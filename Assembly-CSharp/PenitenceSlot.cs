using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Penitences;
using Gameplay.UI.Others.MenuLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PenitenceSlot : MonoBehaviour
{
	private void Awake()
	{
		if (this.childElement != null)
		{
			this.childElement.gameObject.SetActive(false);
		}
	}

	public void UpdateFromSavegameData(PenitenceManager.PenitencePersistenceData data)
	{
		this.DeleteElements();
		if (data == null || data.allPenitences == null)
		{
			return;
		}
		foreach (IPenitence penitence in data.allPenitences)
		{
			if (penitence != null)
			{
				if (this.allPenitences.ContainsKey(penitence.Id))
				{
					bool flag = data.currentPenitence != null && data.currentPenitence.Id == penitence.Id;
					if (penitence.Completed || flag || penitence.Abandoned)
					{
						SelectSaveSlots.PenitenceData penitenceData = this.allPenitences[penitence.Id];
						Sprite sprite;
						if (penitence.Completed)
						{
							sprite = penitenceData.Completed;
						}
						else if (flag)
						{
							sprite = penitenceData.InProgress;
						}
						else
						{
							sprite = penitenceData.Missing;
						}
						this.CreateElement(penitence.Id, sprite);
					}
				}
				else
				{
					Debug.LogError("PenitenceSlot: Error penitence " + penitence.Id + " not found in config");
				}
			}
		}
	}

	public void SetPenitenceConfig(List<SelectSaveSlots.PenitenceData> data)
	{
		this.allPenitences = new Dictionary<string, SelectSaveSlots.PenitenceData>();
		foreach (SelectSaveSlots.PenitenceData penitenceData in data)
		{
			this.allPenitences[penitenceData.id] = penitenceData;
		}
	}

	private void DeleteElements()
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = this.childElement.transform.parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				if (transform.gameObject.activeSelf)
				{
					list.Add(transform.gameObject);
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
		foreach (GameObject obj2 in list)
		{
			UnityEngine.Object.Destroy(obj2);
		}
	}

	private void CreateElement(string name, Sprite sprite)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childElement, this.childElement.transform.parent);
		gameObject.name = "Penitence_" + name;
		gameObject.SetActive(true);
		Image component = gameObject.GetComponent<Image>();
		component.sprite = sprite;
	}

	[SerializeField]
	[BoxGroup("Elements", true, false, 0)]
	private GameObject childElement;

	private Dictionary<string, SelectSaveSlots.PenitenceData> allPenitences = new Dictionary<string, SelectSaveSlots.PenitenceData>();
}
