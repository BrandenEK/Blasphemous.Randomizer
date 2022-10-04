using System;
using UnityEngine;

namespace Gameplay.UI.Others.Buttons
{
	public class ButtonColor : MonoBehaviour
	{
		public Color GetColor(bool highlighted, bool interactable)
		{
			Color result = (!highlighted) ? this.textColorDefault : this.textColorHighlighted;
			if (!interactable)
			{
				result = this.textColorDisabled;
			}
			return result;
		}

		public Color textColorDefault;

		public Color textColorHighlighted;

		public Color textColorDisabled;
	}
}
