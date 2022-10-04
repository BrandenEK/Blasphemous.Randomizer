using System;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class CustomScrollView : MonoBehaviour
	{
		public bool InputEnabled { get; set; }

		public bool ScrollBarNeeded { get; private set; }

		public void NewContentSetted()
		{
			if (this.scrollRect.content != null)
			{
				VerticalLayoutGroup component = this.scrollRect.content.GetComponent<VerticalLayoutGroup>();
				LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)component.transform);
			}
			RectTransform rectTransform = (RectTransform)this.scrollRect.transform;
			RectTransform rectTransform2 = (RectTransform)this.scrollRect.content.transform;
			this.ScrollBarNeeded = (rectTransform2.rect.height + 6f >= rectTransform.rect.height);
			float num = 1f;
			this.scrollRect.verticalNormalizedPosition = num;
			if (this.scrollBar != null)
			{
				this.scrollBar.SetScrollbar(1f - num);
				this.scrollBar.SetEnabled(this.ScrollBarNeeded);
			}
		}

		private void Awake()
		{
			this.InputEnabled = true;
		}

		private void Update()
		{
			if (this.scrollRect == null || !this.InputEnabled || !this.ScrollBarNeeded)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			float num = (!player.GetButton(43)) ? ((!player.GetButton(45)) ? 0f : -1f) : 1f;
			float num2 = player.GetAxis(49);
			if (this.axis == CustomScrollView.AxisEnum.Movement || (this.axis == CustomScrollView.AxisEnum.Both && Mathf.Abs(num2) > this.inputThreshold))
			{
				num = num2;
			}
			if (num > this.inputThreshold || num < -this.inputThreshold)
			{
				RectTransform rectTransform = (RectTransform)this.scrollRect.content.transform;
				float num3 = 1f / rectTransform.rect.height;
				float num4 = this.scrollRect.verticalNormalizedPosition + num * this.scrollBarSpeed * Time.unscaledDeltaTime * num3;
				num4 = Mathf.Clamp01(num4);
				this.scrollRect.verticalNormalizedPosition = num4;
			}
			if (this.scrollBar != null)
			{
				this.scrollBar.SetScrollbar(1f - this.scrollRect.verticalNormalizedPosition);
			}
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		public ScrollRect scrollRect;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		public FixedScrollBar scrollBar;

		[SerializeField]
		[BoxGroup("Params", true, false, 0)]
		private float scrollBarSpeed = 200f;

		[SerializeField]
		[BoxGroup("Params", true, false, 0)]
		private float inputThreshold = 0.1f;

		[SerializeField]
		[BoxGroup("Params", true, false, 0)]
		private CustomScrollView.AxisEnum axis;

		private const int SIZE_EPSILON = 6;

		public enum AxisEnum
		{
			InventoryScroll,
			Movement,
			Both
		}
	}
}
