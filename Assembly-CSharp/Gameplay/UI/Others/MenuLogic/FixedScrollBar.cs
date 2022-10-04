using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class FixedScrollBar : MonoBehaviour
	{
		public void SetEnabled(bool enabled)
		{
			CanvasGroup component = base.GetComponent<CanvasGroup>();
			if (component)
			{
				component.alpha = ((!enabled) ? this.alphaWhenDisabled : 1f);
			}
		}

		public void SetScrollbar(float percent)
		{
			float num = percent * (this.barElement.rect.height - this.handleElement.rect.height);
			num -= this.barElement.rect.height / 2f;
			this.handleElement.localPosition = new Vector3(this.handleElement.localPosition.x, -num, this.handleElement.localPosition.z);
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private RectTransform barElement;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private RectTransform handleElement;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private float alphaWhenDisabled = 0.7f;
	}
}
