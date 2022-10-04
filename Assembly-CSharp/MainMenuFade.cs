using System;
using Gameplay.UI;
using UnityEngine;

public class MainMenuFade : MonoBehaviour
{
	public void Start()
	{
		UIController.instance.fade.CrossFadeAlpha(0f, 0f, false);
		UIController.instance.fade.CrossFadeAlpha(1f, 1f, false);
	}
}
