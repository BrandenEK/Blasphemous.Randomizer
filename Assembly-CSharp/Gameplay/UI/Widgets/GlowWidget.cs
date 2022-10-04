using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	[RequireComponent(typeof(Image))]
	public class GlowWidget : UIWidget
	{
		private void Start()
		{
			this.image = base.GetComponent<Image>();
			this.image.color = this.OFF_COLOR;
		}

		public void Show(float duration = 1f, int loops = 1)
		{
			DOTween.Sequence().Append(this.image.DOColor(this.color, duration)).Append(this.image.DOColor(this.OFF_COLOR, duration)).SetLoops(loops).OnComplete(delegate
			{
				this.color = Color.white;
			}).Play<Sequence>();
		}

		private Image image;

		private Color OFF_COLOR = new Color(0f, 0f, 0f, 0f);

		public Color color = new Color(1f, 1f, 1f, 1f);
	}
}
