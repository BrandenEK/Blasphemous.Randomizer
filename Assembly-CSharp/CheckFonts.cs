using System;
using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckFonts : MonoBehaviour
{
	[Button(ButtonSizes.Medium)]
	public void CheckChild()
	{
		Text[] componentsInChildren = base.GetComponentsInChildren<Text>(true);
		this.CheckInternal(componentsInChildren);
	}

	[Button(ButtonSizes.Medium)]
	public void CheckAll()
	{
		Text[] foundObjects = Resources.FindObjectsOfTypeAll<Text>();
		this.CheckInternal(foundObjects);
	}

	[Button(ButtonSizes.Medium)]
	public void CheckChildTextMesh()
	{
		TextMeshProUGUI[] componentsInChildren = base.GetComponentsInChildren<TextMeshProUGUI>(true);
		this.CheckInternalPro(componentsInChildren);
	}

	[Button(ButtonSizes.Medium)]
	public void CheckAllTextMesh()
	{
		TextMeshProUGUI[] foundObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
		this.CheckInternalPro(foundObjects);
	}

	[Button(ButtonSizes.Medium)]
	public void CheckLocalizationChild()
	{
		Text[] componentsInChildren = base.GetComponentsInChildren<Text>(true);
		this.CheckLocalizationInternal(componentsInChildren);
	}

	[Button(ButtonSizes.Medium)]
	public void CheckLocalizationAll()
	{
		Text[] foundObjects = Resources.FindObjectsOfTypeAll<Text>();
		this.CheckLocalizationInternal(foundObjects);
	}

	public void CheckInternalPro(TextMeshProUGUI[] foundObjects)
	{
		Dictionary<string, CheckFonts.data> dictionary = new Dictionary<string, CheckFonts.data>();
		int num = 0;
		foreach (TextMeshProUGUI textMeshProUGUI in foundObjects)
		{
			if (!(textMeshProUGUI.font == this.goodFontPro))
			{
				if (this.changePro)
				{
					textMeshProUGUI.font = this.goodFontPro;
					textMeshProUGUI.material = this.goodFontPro.material;
					num++;
				}
				else
				{
					if (!dictionary.ContainsKey(textMeshProUGUI.font.name))
					{
						dictionary[textMeshProUGUI.font.name] = new CheckFonts.data();
					}
					dictionary[textMeshProUGUI.font.name].total++;
					dictionary[textMeshProUGUI.font.name].names.Add(this.GetGameObjectPath(textMeshProUGUI.transform));
				}
			}
		}
	}

	public void CheckInternal(Text[] foundObjects)
	{
		Dictionary<string, CheckFonts.data> dictionary = new Dictionary<string, CheckFonts.data>();
		int num = 0;
		foreach (Text text in foundObjects)
		{
			if (!(text.font == this.goodFont))
			{
				if (this.fontsToChange.Contains(text.font))
				{
					text.font = this.goodFont;
					text.material = this.goodFont.material;
					num++;
				}
				else
				{
					if (!dictionary.ContainsKey(text.font.name))
					{
						dictionary[text.font.name] = new CheckFonts.data();
					}
					dictionary[text.font.name].total++;
					dictionary[text.font.name].names.Add(this.GetGameObjectPath(text.transform));
				}
			}
		}
		Debug.Log("********************************************");
		Debug.Log("***  FONTS CHANGEG:" + num.ToString());
		Debug.Log("***  UNKNOW");
		foreach (KeyValuePair<string, CheckFonts.data> keyValuePair in dictionary)
		{
			Debug.Log(keyValuePair.Key + ": " + keyValuePair.Value.total.ToString());
			if (this.showNames && keyValuePair.Value.names.Count > 0)
			{
				foreach (string str in keyValuePair.Value.names)
				{
					Debug.Log("    -" + str);
				}
			}
			if (keyValuePair.Value.prefabs.Count > 0)
			{
				Debug.Log("--- Prefabs");
				foreach (string str2 in keyValuePair.Value.prefabs)
				{
					Debug.Log("    -" + str2);
				}
			}
		}
	}

	public void CheckLocalizationInternal(Text[] foundObjects)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (Text text in foundObjects)
		{
			string gameObjectPath = this.GetGameObjectPath(text.transform);
			Localize component = text.gameObject.GetComponent<Localize>();
			if (!component)
			{
				list.Add(gameObjectPath);
			}
			else if (component.SecondaryTerm == null || !component.SecondaryTerm.StartsWith("UI/FONT"))
			{
				list2.Add(string.Concat(new string[]
				{
					gameObjectPath,
					" -- TEXT: ",
					text.text,
					"  SECOND:",
					component.SecondaryTerm
				}));
			}
		}
		Debug.Log("********************************************");
		Debug.Log("***  NO LOCALIZATION:" + list.Count.ToString());
		foreach (string message in list)
		{
			Debug.Log(message);
		}
		Debug.Log("********************************************");
		Debug.Log("***  ERROR SECONDARY:" + list2.Count.ToString());
		foreach (string message2 in list2)
		{
			Debug.Log(message2);
		}
	}

	private string GetGameObjectPath(Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	public Font goodFont;

	public TMP_FontAsset goodFontPro;

	public List<Font> fontsToChange;

	public bool showNames;

	public bool changePro;

	private class data
	{
		public int total;

		public List<string> names = new List<string>();

		public List<string> prefabs = new List<string>();
	}
}
