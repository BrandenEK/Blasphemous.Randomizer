using System;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Sirenix.OdinInspector;
using Tools.Level.Interactables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationWidget : UIWidget
{
	private void Awake()
	{
		this.Show(false);
	}

	public void Show(bool b)
	{
		if (b)
		{
			this.GenerateList();
		}
		if (Core.Logic.CurrentLevelConfig)
		{
			Core.Logic.CurrentLevelConfig.TimeScale = (float)((!b) ? 1 : 0);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(b);
		}
	}

	private void GenerateList()
	{
		PrieDieu[] array = UnityEngine.Object.FindObjectsOfType<PrieDieu>();
		Door[] array2 = UnityEngine.Object.FindObjectsOfType<Door>();
		this.firstButton = null;
		this.previousButton = null;
		if (array.Length > 0 || array2.Length > 0)
		{
			this.DestroyList();
		}
		for (int i = 0; i < array.Length; i++)
		{
			this.CreateButton(array[i].name, array[i].transform.position);
		}
		for (int j = 0; j < array2.Length; j++)
		{
			this.CreateButton(array2[j].name, array2[j].spawnPoint.position);
		}
		if (this.firstButton)
		{
			EventSystem.current.SetSelectedGameObject(this.firstButton);
			this.firstButton.GetComponent<Button>().OnSelect(new BaseEventData(EventSystem.current));
		}
	}

	private void DestroyList()
	{
		this.firstButton = null;
		for (int i = 0; i < this.list.childCount; i++)
		{
			UnityEngine.Object.Destroy(this.list.GetChild(i).gameObject);
		}
	}

	private void CreateButton(string identificativeName, Vector3 position)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab);
		gameObject.transform.SetParent(this.list);
		gameObject.transform.localScale = Vector3.one;
		Text componentInChildren = gameObject.GetComponentInChildren<Text>();
		NavigationButton componentInChildren2 = gameObject.GetComponentInChildren<NavigationButton>();
		componentInChildren2.destination = position;
		componentInChildren.text = identificativeName;
		componentInChildren.fontSize = this.ButtonFontSize;
		Button componentInChildren3 = gameObject.GetComponentInChildren<Button>();
		if (!this.firstButton)
		{
			this.firstButton = gameObject;
		}
		if (this.previousButton)
		{
			Navigation navigation = this.previousButton.navigation;
			navigation.selectOnDown = componentInChildren3;
			this.previousButton.navigation = navigation;
			navigation = componentInChildren3.navigation;
			navigation.selectOnUp = this.previousButton;
			componentInChildren3.navigation = navigation;
		}
		this.previousButton = componentInChildren3;
	}

	[SerializeField]
	[BoxGroup("Attached References", true, false, 0)]
	private GameObject buttonPrefab;

	[SerializeField]
	[BoxGroup("Attached References", true, false, 0)]
	private Transform list;

	public int ButtonFontSize = 10;

	private GameObject firstButton;

	private Button previousButton;
}
