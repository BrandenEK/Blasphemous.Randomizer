using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost
{
	public class BellGhostFloatingMotion : Trait
	{
		public bool IsFloating { get; set; }

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.IsFloating && base.EntityOwner.SpriteRenderer.isVisible)
			{
				this.Floating();
			}
		}

		private void Floating()
		{
			this.index += Time.deltaTime;
			float x = this.amplitudeX * Mathf.Cos(this.speedX * this.index);
			float y = Mathf.Sin(this.speedY * this.index) * this.amplitudeY;
			Vector3 vector = new Vector3(x, y, 0f) + this.Offset;
			base.transform.localPosition = ((!this.UseSlerp) ? vector : Vector3.Slerp(base.transform.localPosition, vector, Time.deltaTime));
		}

		public float amplitudeX = 10f;

		public float amplitudeY = 5f;

		private float index;

		public float speedX = 1f;

		public float speedY = 2f;

		public bool UseSlerp;

		public Vector2 Offset;
	}
}
