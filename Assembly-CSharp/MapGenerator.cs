using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Tools.DataContainer;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MapGenerator : SerializedMonoBehaviour
{
	[BoxGroup("Config", true, false, 0)]
	[Button(22)]
	public void Refresh()
	{
		this.hasError = false;
		this.errorDescription = string.Empty;
		this.numDomains = 0;
		this.numZones = 0;
		this.domainZones.Clear();
		this.domains.Clear();
		this.currentCamera = this.orthCamera;
		if (!this.currentCamera)
		{
			this.currentCamera = base.GetComponent<Camera>();
		}
		if (!this.currentCamera)
		{
			this.hasError = true;
			this.errorDescription = "Select a camera or attach to gameobject with camera";
			return;
		}
		Regex regex = new Regex(this.domainRegExp);
		List<Transform> list;
		if (!this.rootMap)
		{
			list = (from gObj in base.gameObject.scene.GetRootGameObjects()
			select gObj.transform).ToList<Transform>();
		}
		else
		{
			list = this.rootMap.Cast<Transform>().ToList<Transform>();
		}
		if (list.Count <= 0)
		{
			this.hasError = true;
			this.errorDescription = "The root object hasn't any child with this regex (use null if you want to use scene root)";
			return;
		}
		foreach (Transform transform in list)
		{
			string name = transform.name;
			Match match = regex.Match(name);
			if (match.Success)
			{
				this.domains.Add(transform);
				this.domainZones[name] = new List<Transform>();
				this.numDomains++;
				IEnumerator enumerator2 = transform.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj = enumerator2.Current;
						Transform item = (Transform)obj;
						this.domainZones[name].Add(item);
						this.numZones++;
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator2 as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}

	private IList<ValueDropdownItem<string>> GetDomains()
	{
		ValueDropdownList<string> valueDropdownList = new ValueDropdownList<string>();
		foreach (string text in this.domainZones.Keys)
		{
			valueDropdownList.Add(text, text);
		}
		return valueDropdownList;
	}

	[HorizontalGroup("Domain", 0f, 0, 0, 0)]
	[ShowIf("CanSave", true)]
	[Button(0)]
	public void SaveDomain()
	{
		this._SaveDomainMaps(this.currentDomain);
	}

	private IList<ValueDropdownItem<string>> GetZones()
	{
		ValueDropdownList<string> valueDropdownList = new ValueDropdownList<string>();
		if (this.currentDomain != null && this.domainZones.ContainsKey(this.currentDomain))
		{
			foreach (Transform transform in this.domainZones[this.currentDomain])
			{
				valueDropdownList.Add(transform.name, transform.name);
			}
		}
		return valueDropdownList;
	}

	[HorizontalGroup("Zone", 0f, 0, 0, 0)]
	[ShowIf("CanSave", true)]
	[Button(0)]
	public void SaveZone()
	{
		this._SaveZoneMap(this.currentDomain, this.currentZone);
	}

	[BoxGroup("Generate", true, false, 0)]
	[ShowIf("CanSave", true)]
	[Button(31)]
	public void SaveAllMaps()
	{
		this.PrepareScene();
		foreach (List<Transform> list in this.domainZones.Values.ToList<List<Transform>>())
		{
			foreach (Transform tr in list)
			{
				this._ExportMap(tr);
			}
		}
		this.RestoreScene();
	}

	[BoxGroup("Generate", true, false, 0)]
	[ShowIf("CanSave", true)]
	[Button(31)]
	public void CheckAllMaps()
	{
		foreach (List<Transform> list in this.domainZones.Values.ToList<List<Transform>>())
		{
			foreach (Transform cameraAndCheckMap in list)
			{
				this.SetCameraAndCheckMap(cameraAndCheckMap);
			}
		}
	}

	private bool CanSave()
	{
		return this.numZones != 0;
	}

	[BoxGroup("Config", true, false, 0)]
	[ShowIf("InCanvas", true)]
	[Button(22)]
	public void TestInUI()
	{
		MapData mapData = Resources.Load<MapData>(this.MAP_RESOURCE_CONFIG);
		GameObject gameObject = new GameObject("MAP", new Type[]
		{
			typeof(RectTransform)
		});
		gameObject.transform.SetParent(base.transform);
		RectTransform rectTransform = (RectTransform)gameObject.transform;
		rectTransform.localRotation = Quaternion.identity;
		rectTransform.localScale = Vector3.one;
		rectTransform.localPosition = Vector3.zero;
		foreach (KeyValuePair<string, MapData.MapItem> keyValuePair in mapData.data)
		{
			float num = (float)keyValuePair.Value.height / (keyValuePair.Value.cameraSize * 2f);
			GameObject gameObject2 = new GameObject(keyValuePair.Key, new Type[]
			{
				typeof(RectTransform)
			});
			gameObject2.transform.SetParent(rectTransform);
			RectTransform rectTransform2 = (RectTransform)gameObject2.transform;
			rectTransform2.localRotation = Quaternion.identity;
			rectTransform2.localScale = Vector3.one;
			rectTransform2.localPosition = new Vector3(keyValuePair.Value.position.x * num, keyValuePair.Value.position.y * num, 0f);
			rectTransform2.sizeDelta = new Vector2((float)keyValuePair.Value.width, (float)keyValuePair.Value.height);
			Image image = gameObject2.AddComponent<Image>();
			Sprite sprite = (Sprite)Resources.Load(keyValuePair.Value.mapImage, typeof(Sprite));
			image.sprite = sprite;
		}
	}

	private bool InCanvas()
	{
		return base.GetComponent<Canvas>() != null;
	}

	private void _SaveDomainMaps(string domain)
	{
		if (this.domainZones.ContainsKey(domain))
		{
			this.PrepareScene();
			foreach (Transform tr in this.domainZones[domain])
			{
				this._ExportMap(tr);
			}
			this.RestoreScene();
		}
	}

	private void _SaveZoneMap(string domain, string zone)
	{
		if (this.domainZones.ContainsKey(domain))
		{
			this.PrepareScene();
			foreach (Transform transform in this.domainZones[domain])
			{
				if (transform.name == zone)
				{
					this._ExportMap(transform);
				}
			}
			this.RestoreScene();
		}
	}

	private bool SetCameraAndCheckMap(Transform tr)
	{
		Bounds maxBounds = this.GetMaxBounds(tr);
		Vector3 localPosition;
		localPosition..ctor(maxBounds.center.x, maxBounds.center.y, -100f);
		this.currentCamera.transform.localPosition = localPosition;
		bool flag = this.IsFullyVisible(maxBounds);
		if (!flag)
		{
			Debug.Log("*** Error in map DOMAIN:" + tr.parent.name + " ZONE: " + tr.name);
		}
		return flag;
	}

	private void _AddToHideAllObjectsByTag(List<GameObject> list, Transform parent, string tag)
	{
		IEnumerator enumerator = parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				if (transform.tag == tag)
				{
					transform.gameObject.SetActive(false);
					list.Add(transform.gameObject);
				}
				else if (transform.childCount > 0)
				{
					this._AddToHideAllObjectsByTag(list, transform, tag);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void _ExportMap(Transform tr)
	{
		this.SetCameraAndCheckMap(tr);
		List<GameObject> list = new List<GameObject>();
		tr.parent.gameObject.SetActive(true);
		IEnumerator enumerator = tr.parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj2 = enumerator.Current;
				Transform transform = (Transform)obj2;
				transform.gameObject.SetActive(transform == tr);
				list.Add(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		Dictionary<SpriteRenderer, Color> dictionary = new Dictionary<SpriteRenderer, Color>();
		foreach (SpriteRenderer spriteRenderer in tr.GetComponentsInChildren<SpriteRenderer>())
		{
			dictionary[spriteRenderer] = spriteRenderer.color;
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
		}
		foreach (TextMesh textMesh in tr.GetComponentsInChildren<TextMesh>())
		{
			if (textMesh.gameObject.activeSelf)
			{
				list.Add(textMesh.gameObject);
				textMesh.gameObject.SetActive(false);
			}
		}
		this._AddToHideAllObjectsByTag(list, tr, "EditorOnly");
		Debug.Log("-- Exporting " + tr.name);
		string domain = tr.parent.name.Substring(0, 3);
		string zone = tr.name.Substring(0, 3);
		string mapImage = this.SaveScreenShot(domain, zone);
		if (this.currentData)
		{
			MapData.MapItem mapItem = new MapData.MapItem();
			mapItem.position = new Vector3(this.currentCamera.transform.position.x, this.currentCamera.transform.position.y, 0f);
			mapItem.domain = domain;
			mapItem.zone = zone;
			mapItem.mapImage = mapImage;
			mapItem.height = 360;
			mapItem.width = 640;
			mapItem.cameraSize = this.currentCamera.orthographicSize;
			mapItem.cells = new List<Bounds>();
			SpriteRenderer[] componentsInChildren3 = tr.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer renderer in componentsInChildren3)
			{
				if (!renderer.gameObject.CompareTag("EditorOnly"))
				{
					mapItem.cells.Add(new Bounds(renderer.bounds.center, renderer.bounds.size));
				}
			}
			this.currentData.data[mapItem.GetKey()] = mapItem;
		}
		foreach (KeyValuePair<SpriteRenderer, Color> keyValuePair in dictionary)
		{
			keyValuePair.Key.color = keyValuePair.Value;
		}
		list.ForEach(delegate(GameObject obj)
		{
			obj.SetActive(true);
		});
		tr.parent.gameObject.SetActive(false);
	}

	private void PrepareScene()
	{
		foreach (Transform transform in this.domains)
		{
			transform.gameObject.SetActive(false);
		}
		this.currentData = Resources.Load<MapData>(this.MAP_RESOURCE_CONFIG);
		if (!this.currentData)
		{
			this.currentData = ScriptableObject.CreateInstance<MapData>();
		}
		this.currentData.data = new Dictionary<string, MapData.MapItem>();
	}

	private void RestoreScene()
	{
		foreach (Transform transform in this.domains)
		{
			transform.gameObject.SetActive(true);
		}
	}

	private Bounds GetMaxBounds(Transform obj)
	{
		bool flag = true;
		Bounds result = default(Bounds);
		SpriteRenderer[] componentsInChildren = obj.GetComponentsInChildren<SpriteRenderer>();
		if (componentsInChildren.Length > 0)
		{
			foreach (SpriteRenderer renderer in componentsInChildren)
			{
				if (!renderer.gameObject.CompareTag("EditorOnly"))
				{
					if (flag)
					{
						result..ctor(componentsInChildren[0].bounds.center, componentsInChildren[0].bounds.size);
						flag = false;
					}
					else
					{
						result.Encapsulate(renderer.bounds);
					}
				}
			}
		}
		else
		{
			result..ctor(obj.position, Vector3.zero);
		}
		return result;
	}

	public void SetImportSetting()
	{
	}

	public bool IsFullyVisible(Bounds bounds)
	{
		Vector3 size = bounds.size;
		Vector3 min = bounds.min;
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(this.currentCamera);
		List<Vector3> list = new List<Vector3>(8)
		{
			min,
			min + new Vector3(0f, 0f, size.z),
			min + new Vector3(size.x, 0f, size.z),
			min + new Vector3(size.x, 0f, 0f)
		};
		for (int i = 0; i < 4; i++)
		{
			list.Add(list[i] + size.y * Vector3.up);
		}
		for (int j = 0; j < array.Length; j++)
		{
			for (int k = 0; k < list.Count; k++)
			{
				if (!array[j].GetSide(list[k]))
				{
					return false;
				}
			}
		}
		return true;
	}

	private string SaveScreenShot(string domain, string zone)
	{
		RenderTexture renderTexture = new RenderTexture(640, 360, 32, 0);
		this.currentCamera.targetTexture = renderTexture;
		Texture2D texture2D = new Texture2D(640, 360, 5, false);
		this.currentCamera.Render();
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, 640f, 360f), 0, 0);
		this.currentCamera.targetTexture = null;
		RenderTexture.active = null;
		Object.DestroyImmediate(renderTexture);
		return this.EditorSaveImageToDisk(texture2D, string.Format("{0}_{1}", domain, zone));
	}

	private string SaveMask(Transform tr, string domain, string zone)
	{
		Texture2D texture2D = new Texture2D(640, 360, 1, false);
		byte element = 0;
		texture2D.LoadRawTextureData(Enumerable.Repeat<byte>(element, 230400).ToArray<byte>());
		texture2D.Apply();
		return this.EditorSaveImageToDisk(texture2D, string.Format("{0}_{1}_Mask", domain, zone));
	}

	private string EditorSaveImageToDisk(Texture2D screenShot, string name)
	{
		byte[] bytes = ImageConversion.EncodeToPNG(screenShot);
		string text = "Maps\\" + name;
		string text2 = Application.dataPath + "\\Resources\\" + text + ".png";
		if (!Directory.Exists(Path.GetDirectoryName(text2)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(text2));
		}
		Debug.Log(text2);
		File.WriteAllBytes(text2, bytes);
		return text;
	}

	private List<Transform> domains = new List<Transform>();

	private Dictionary<string, List<Transform>> domainZones = new Dictionary<string, List<Transform>>();

	private Camera currentCamera;

	private const int resWidth = 640;

	private const int resHeight = 360;

	private string MAP_RESOURCE_CONFIG = "Maps/MapData";

	private MapData currentData;

	[BoxGroup("Config", true, false, 0)]
	public string domainRegExp = "D[0-9][0-9] - (?<name>[a-zA-Z0-9_ ()]*)";

	[BoxGroup("Config", true, false, 0)]
	public Transform rootMap;

	[BoxGroup("Config", true, false, 0)]
	public Camera orthCamera;

	private bool hasError;

	[ShowIf("hasError", true)]
	[BoxGroup("Config", true, false, 0)]
	[ShowInInspector]
	[ReadOnly]
	[LabelText("")]
	[Multiline]
	private string errorDescription;

	[BoxGroup("Generate", true, false, 0)]
	[ShowInInspector]
	[ReadOnly]
	[LabelText("Number of domains")]
	private int numDomains;

	[BoxGroup("Generate", true, false, 0)]
	[ShowInInspector]
	[ReadOnly]
	[LabelText("Number of zones")]
	private int numZones;

	[HorizontalGroup("Domain", 0f, 0, 0, 0)]
	[ShowIf("CanSave", true)]
	[ValueDropdown("GetDomains")]
	private string currentDomain;

	[HorizontalGroup("Zone", 0f, 0, 0, 0)]
	[ShowIf("CanSave", true)]
	[ValueDropdown("GetZones")]
	private string currentZone;
}
