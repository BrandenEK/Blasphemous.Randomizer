using System;
using System.Collections;
using Framework.Managers;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class Escape : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(27) && !this.pressed)
			{
				base.StartCoroutine(this.Action());
			}
		}

		private IEnumerator Action()
		{
			this.pressed = true;
			FadeWidget.instance.Fade(true, 1.5f, 0f, null);
			yield return new WaitForSeconds(1f);
			Core.Logic.LoadMenuScene(true);
			this.pressed = false;
			yield break;
		}

		private bool pressed;
	}
}
