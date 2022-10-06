using System;
using Framework.Managers;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InputScheme : MonoBehaviour
{
	private void Start()
	{
		Core.Input.KeyboardPressed += this.Refresh;
		Core.Input.JoystickPressed += this.Refresh;
		LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.Refresh);
	}

	private void Refresh()
	{
		string currentLanguage = LocalizationManager.CurrentLanguage;
		if (currentLanguage.Equals("Spanish") && Core.Input.ActiveControllerType == null)
		{
			this.SetActiveScheme(this.keyboardSpanish);
		}
		else if (currentLanguage.Equals("English") && Core.Input.ActiveControllerType == null)
		{
			this.SetActiveScheme(this.keyboardEnglish);
		}
		else if (currentLanguage.Equals("Spanish") && Core.Input.ActiveControllerType == 2)
		{
			this.SetActiveScheme(this.gamepadSpanish);
		}
		else if (currentLanguage.Equals("English") && Core.Input.ActiveControllerType == 2)
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
		LocalizationManager.OnLocalizeEvent -= new LocalizationManager.OnLocalizeCallback(this.Refresh);
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
