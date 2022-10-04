using System;
using System.Collections;
using Framework.Managers;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Framework.Util
{
	public class FinalDoor : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.layer == LayerMask.NameToLayer("Penitent"))
			{
				this.inside = true;
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if (col.gameObject.layer == LayerMask.NameToLayer("Penitent"))
			{
				this.inside = false;
			}
		}

		private void Update()
		{
			if (this.inside && Core.Logic.Penitent.PlatformCharacterInput.isJoystickUp && !this.used)
			{
				base.StartCoroutine(this.action());
				this.used = true;
			}
		}

		private IEnumerator action()
		{
			FadeWidget.instance.Fade(false, 1.5f, 0f, null);
			yield return new WaitForSeconds(1f);
			Core.Logic.LoadAttrackScene();
			yield break;
		}

		private bool used;

		private bool inside;
	}
}
