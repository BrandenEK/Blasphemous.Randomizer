using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Sparks
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class SwordSpark : MonoBehaviour
	{
		private void Awake()
		{
			this.swordSparkSpriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		private void Start()
		{
			this._penitent = Core.Logic.Penitent;
		}

		private void Update()
		{
			this.setOrientation(this._penitent.Status.Orientation);
		}

		protected void setOrientation(EntityOrientation orientation)
		{
			if (orientation == EntityOrientation.Left && !this.swordSparkSpriteRenderer.flipX)
			{
				this.swordSparkSpriteRenderer.flipX = true;
			}
			if (orientation == EntityOrientation.Right && this.swordSparkSpriteRenderer.flipX)
			{
				this.swordSparkSpriteRenderer.flipX = false;
			}
		}

		public SwordSpark.SwordSparkType sparkType;

		private SpriteRenderer swordSparkSpriteRenderer;

		private Penitent _penitent;

		public enum SwordSparkType
		{
			swordSpark_1,
			swordSpark_2,
			swordSpark_3
		}
	}
}
