using System;
using System.Collections.Generic;
using Framework.Managers;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockWidget : SerializedMonoBehaviour
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
	[Button("Check InGame", ButtonSizes.Small)]
	private void CheckIngame()
	{
		I2.Loc.LocalizationManager.CurrentLanguage = this.Language;
		this.currentController = this.debugController;
		this.currentJoystick = this.debugJoystick;
		this.ShowInGame();
	}

	public bool WantToExit { get; private set; }

	public void ShowInGame()
	{
		this.Show(0, 0, true);
	}

	private void Update()
	{
		if (!this.IsShowing)
		{
			return;
		}
		if (this.rewired == null)
		{
			this.rewired = ReInput.players.GetPlayer(0);
		}
		if (this.rewired.GetButtonDown(51))
		{
			this.IsShowing = false;
			this.WantToExit = true;
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

	private void Show(int current, int total, bool catchInput = true)
	{
		this.WantToExit = false;
		this.IsShowing = true;
		this.rootCanvas.alpha = this.alphaInGame;
		if (catchInput)
		{
			this.UpdateInput();
		}
		this.UpdateTextPro();
	}

	private void UpdateTextPro()
	{
		string text = "UI_Extras/UNLOCK_" + this.unlockId;
		foreach (UnlockWidget.LocalizationText localizationText in this.texts)
		{
			if (localizationText.text.mTerm.Equals(text))
			{
				string localizedText = localizationText.text;
				localizationText.mesh.text = Framework.Managers.LocalizationManager.ParseMeshPro(localizedText, text, null);
			}
		}
	}

	private void UpdateInput()
	{
		this.currentJoystick = Core.Input.ActiveJoystickModel;
		this.currentController = Core.Input.ActiveControllerType;
	}

	public void Configurate(string unlockId)
	{
		Sprite paletteSpritePreview = Core.ColorPaletteManager.GetPaletteSpritePreview(unlockId);
		this.skinPreviewImage.sprite = paletteSpritePreview;
		this.unlockId = unlockId;
	}

	[BoxGroup("References", true, false, 0)]
	[SerializeField]
	private CanvasGroup rootCanvas;

	[BoxGroup("References", true, false, 0)]
	[SerializeField]
	private GameObject buttonBackInGame;

	[BoxGroup("References", true, false, 0)]
	[SerializeField]
	private float alphaInGame = 0.8f;

	[BoxGroup("Skin info", true, false, 0)]
	[SerializeField]
	private Image skinPreviewImage;

	[BoxGroup("Text", true, false, 0)]
	[SerializeField]
	private List<UnlockWidget.LocalizationText> texts = new List<UnlockWidget.LocalizationText>();

	[BoxGroup("Debug", true, false, 0)]
	[ValueDropdown("MyLanguages")]
	public string Language;

	[BoxGroup("Debug", true, false, 0)]
	[SerializeField]
	private ControllerType debugController;

	[BoxGroup("Debug", true, false, 0)]
	[SerializeField]
	private JoystickType debugJoystick;

	private bool IsShowing;

	private Player rewired;

	private JoystickType currentJoystick;

	private ControllerType currentController;

	private string unlockId;

	[Serializable]
	private class LocalizationText
	{
		public TextMeshProUGUI mesh;

		public LocalizedString text;
	}
}
