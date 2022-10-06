using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class CustomScrollBar : MonoBehaviour
	{
		private void OnEditorChanged()
		{
			this.CurrentValue = this.initialValue;
		}

		public float CurrentValue
		{
			get
			{
				return this._currentValue;
			}
			set
			{
				this._currentValue = Mathf.Clamp01(value);
				if (this.filledImage != null && this.pointer != null && this.slidingdBar != null)
				{
					this.filledImage.fillAmount = this._currentValue;
					float num = this.slidingdBar.sizeDelta.x * (this._currentValue - 0.5f);
					this.pointer.localPosition = new Vector3(num, 0f, 0f);
				}
			}
		}

		private void Awake()
		{
			this.CurrentValue = this.initialValue;
		}

		[SerializeField]
		private Image filledImage;

		[SerializeField]
		private RectTransform slidingdBar;

		[SerializeField]
		private RectTransform pointer;

		[SerializeField]
		[OnValueChanged("OnEditorChanged", false)]
		private float initialValue;

		private float _currentValue;
	}
}
