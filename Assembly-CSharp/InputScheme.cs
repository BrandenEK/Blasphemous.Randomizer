using System;
using Framework.Managers;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InputScheme : MonoBehaviour
{
	private void Start()
	{
		Core.Input.KeyboardPressed += this.Refresh;
		Core.Input.JoystickPressed += this.Refresh;
		I2.Loc.LocalizationManager.OnLocalizeEvent += this.Refresh;
	}

	private void Refresh()
	{
		string currentLanguage = I2.Loc.LocalizationManager.CurrentLanguage;
		if (currentLanguage.Equals("Spanish") && Core.Input.ActiveControllerType == ControllerType.Keyboard)
		{
			this.SetActiveScheme(this.keyboardSpanish);
		}
		else if (currentLanguage.Equals("English") && Core.Input.ActiveControllerType == ControllerType.Keyboard)
		{
			this.SetActiveScheme(this.keyboardEnglish);
		}
		else if (currentLanguage.Equals("Spanish") && Core.Input.ActiveControllerType == ControllerType.Joystick)
		{
			this.SetActiveScheme(this.gamepadSpanish);
		}
		else if (currentLanguage.Equals("English") && Core.Input.ActiveControllerType == ControllerType.Joystick)
		{
			this.SetActiveScheme(this.gamepadEnglish);
		}
	}

	private void SetActiveScheme(Image image)
	{
		try
		{
			this.keyboardSpanish.enabled = false;
			this.keyboardEnglish.enabled = false;
			this.gamepadSpanish.enabled = false;
			this.gamepadEnglish.enabled = false;
			image.enabled = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("Main menu input scheme has no image." + ex.ToString());
		}
	}

	private void OnDestroy()
	{
		Core.Input.KeyboardPressed -= this.Refresh;
		Core.Input.JoystickPressed -= this.Refresh;
		I2.Loc.LocalizationManager.OnLocalizeEvent -= this.Refresh;
	}

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	private Image keyboardSpanish;

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	private Image keyboardEnglish;

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	private Image gamepadSpanish;

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	private Image gamepadEnglish;
}
