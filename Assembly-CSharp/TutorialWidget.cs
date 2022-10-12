using System;
using System.Collections.Generic;
using Framework.Managers;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWidget : SerializedMonoBehaviour
{
	private IList<ValueDropdownItem<string>> MyLanguages()
	{
		ValueDropdownList<string> valueDropdownList = new ValueDropdownList<string>();
		string[] array = I2.Loc.LocalizationManager.GetAllLanguages(true).ToArray();
		Array.Sort<string>(array);
		foreach (string text in array)
		{
			valueDropdownList.Add(text, text);
		}
		return valueDropdownList;
	}

	[BoxGroup("Debug", true, false, 0)]
	[Button("Check menu", ButtonSizes.Small)]
	private void CheckMenu()
	{
		I2.Loc.LocalizationManager.CurrentLanguage = this.Language;
		this.currentController = this.debugController;
		this.currentJoystick = this.debugJoystick;
		this.ShowInMenu(3, 10);
	}

	[BoxGroup("Debug", true, false, 0)]
	[Button("Check InGame", ButtonSizes.Small)]
	private void CheckIngame()
	{
		I2.Loc.LocalizationManager.CurrentLanguage = this.Language;
		this.currentController = this.debugController;
		this.currentJoystick = this.debugJoystick;
		this.ShowInGame();
	}

	public bool WantToExit { get; private set; }

	public void ShowInMenu(int current, int total)
	{
		this.Show(true, current, total, false);
	}

	public void ShowInGame()
	{
		this.Show(false, 0, 0, true);
	}

	private void Update()
	{
		if (!this.IsSwowing)
		{
			return;
		}
		if (this.rewired == null)
		{
			this.rewired = ReInput.players.GetPlayer(0);
		}
		if (this.rewired.GetButtonDown(51))
		{
			this.Hide();
		}
		else
		{
			JoystickType activeJoystickModel = Core.Input.ActiveJoystickModel;
			ControllerType activeControllerType = Core.Input.ActiveControllerType;
			if (activeJoystickModel != this.currentJoystick || activeControllerType != this.currentController)
			{
				this.UpdateInput();
				this.UpdateTextPro();
			}
		}
	}

	public void Hide()
	{
		this.IsSwowing = false;
		this.WantToExit = true;
	}

	private void Show(bool inMenu, int current, int total, bool catchInput = true)
	{
		this.WantToExit = false;
		this.IsSwowing = true;
		this.rootCanvas.alpha = ((!inMenu) ? this.alphaInGame : this.alphaMenu);
		this.SetInMenu(inMenu);
		if (inMenu)
		{
			this.counter.text = current.ToString() + " / " + total.ToString();
		}
		if (catchInput)
		{
			this.UpdateInput();
		}
		this.UpdateTextPro();
	}

	private void SetInMenu(bool inmenu)
	{
		this.InMenu = inmenu;
		this.buttonBackMenu.SetActive(inmenu);
		this.buttonNavLeft.SetActive(inmenu);
		this.buttonNavRight.SetActive(inmenu);
		this.counter.gameObject.SetActive(inmenu);
		this.buttonBackInGame.SetActive(!inmenu);
	}

	private void UpdateTextPro()
	{
		foreach (TutorialWidget.LocalizationText localizationText in this.texts)
		{
			string localizedText = localizationText.text;
			localizationText.mesh.text = Framework.Managers.LocalizationManager.ParseMeshPro(localizedText, localizationText.text.mTerm, localizationText.mesh);
		}
	}

	private void UpdateInput()
	{
		this.currentJoystick = Core.Input.ActiveJoystickModel;
		this.currentController = Core.Input.ActiveControllerType;
	}

	[BoxGroup("Common", true, false, 0)]
	[SerializeField]
	private CanvasGroup rootCanvas;

	[BoxGroup("Menu", true, false, 0)]
	[SerializeField]
	private GameObject buttonBackMenu;

	[BoxGroup("Menu", true, false, 0)]
	[SerializeField]
	private GameObject buttonNavLeft;

	[BoxGroup("Menu", true, false, 0)]
	[SerializeField]
	private GameObject buttonNavRight;

	[BoxGroup("Menu", true, false, 0)]
	[SerializeField]
	private Text counter;

	[BoxGroup("Menu", true, false, 0)]
	[SerializeField]
	private float alphaMenu = 1f;

	[BoxGroup("InGame", true, false, 0)]
	[SerializeField]
	private GameObject buttonBackInGame;

	[BoxGroup("InGame", true, false, 0)]
	[SerializeField]
	private float alphaInGame = 0.8f;

	[BoxGroup("Text", true, false, 0)]
	[SerializeField]
	private List<TutorialWidget.LocalizationText> texts = new List<TutorialWidget.LocalizationText>();

	[BoxGroup("Debug", true, false, 0)]
	[ValueDropdown("MyLanguages")]
	public string Language;

	[BoxGroup("Debug", true, false, 0)]
	[SerializeField]
	private ControllerType debugController;

	[BoxGroup("Debug", true, false, 0)]
	[SerializeField]
	private JoystickType debugJoystick;

	private bool InMenu;

	private bool IsSwowing;

	private Player rewired;

	private JoystickType currentJoystick;

	private ControllerType currentController;

	[Serializable]
	private class LocalizationText
	{
		public TextMeshProUGUI mesh;

		public LocalizedString text;
	}
}
