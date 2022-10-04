using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class InteractionSignal : MonoBehaviour
	{
		private void Awake()
		{
		}

		private void Update()
		{
			Penitent penitent = Core.Logic.Penitent;
			if (penitent != null)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(penitent.transform.position);
				vector += Vector3.up * this.yOffset;
				base.gameObject.transform.position = vector;
			}
		}

		public void SetText(string text)
		{
			this.label.text = text;
		}

		public void Show(PadButton btn)
		{
			if (btn == PadButton.Y)
			{
				this.yIcon.enabled = true;
				this.downIcon.enabled = false;
			}
			else if (btn == PadButton.Down)
			{
				this.yIcon.enabled = false;
				this.downIcon.enabled = true;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}

		public void Hide()
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}

		public Text label;

		public Image yIcon;

		public Image downIcon;

		public float yOffset = 5f;
	}
}
