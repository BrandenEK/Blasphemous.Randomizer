using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Buttons
{
	public class TLDButton : MenuButton, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
	{
		protected override void InheritedAwake()
		{
			base.InheritedAwake();
			this.buttonImage = base.GetComponent<Image>();
			if (this.buttonImage != null)
			{
				this.buttonImageColor = this.buttonImage.color;
			}
		}

		protected override void InheritedStart()
		{
			base.InheritedStart();
			float num = Color.white.a / 2f;
			this.setButtonColorAlpha(num);
			if (this.buttonText != null)
			{
				this.setButtonTextColorAlpha(num);
			}
			this.enabledButton = false;
		}

		protected override void OnSelectInherited(BaseEventData eventData)
		{
			base.OnSelectInherited(eventData);
			if (!this.enabledButton)
			{
				this.enabledButton = true;
				this.showButtonFocusEffect(true);
			}
		}

		protected override void OnDeselectedInherited(BaseEventData eventData)
		{
			base.OnDeselectedInherited(eventData);
			if (this.enabledButton)
			{
				this.enabledButton = !this.enabledButton;
				this.showButtonFocusEffect(false);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this.enabledButton)
			{
				this.enabledButton = true;
				this.showButtonFocusEffect(true);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.enabledButton)
			{
				this.enabledButton = !this.enabledButton;
				this.showButtonFocusEffect(false);
			}
		}

		private void showButtonFocusEffect(bool show = true)
		{
			if (show)
			{
				float a = Color.white.a;
				this.setButtonColorAlpha(a);
				this.setButtonTextColorAlpha(a);
			}
			else
			{
				float num = Color.white.a / 2f;
				this.setButtonColorAlpha(num);
				this.setButtonTextColorAlpha(num);
			}
		}

		private void setButtonColorAlpha(float colorAlpha)
		{
			this.buttonImageColor.a = colorAlpha;
			this.buttonImage.color = this.buttonImageColor;
		}

		private void setButtonTextColorAlpha(float textAlpha)
		{
			Color color = this.buttonText.color;
			color.a = textAlpha;
			this.buttonText.color = color;
		}

		protected Image buttonImage;

		protected Color buttonImageColor;

		protected bool enabledButton;
	}
}
