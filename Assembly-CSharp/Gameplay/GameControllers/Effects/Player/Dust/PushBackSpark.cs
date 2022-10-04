using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class PushBackSpark : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.PushBackSparkAnimator == null)
			{
				Debug.LogError("An animator is needed!");
				return;
			}
			this._spriteRenderer = this.PushBackSparkAnimator.GetComponent<SpriteRenderer>();
		}

		public void TriggerPushBackSparks()
		{
			if (this.PushBackSparkAnimator == null)
			{
				return;
			}
			this.FlipSpriteRenderer();
			this.PushBackSparkAnimator.Play(this._pushBackSparkAnimHash, 0, 0f);
		}

		public void FlipSpriteRenderer()
		{
			if (base.EntityOwner == null)
			{
				return;
			}
			if (base.EntityOwner.Status.Orientation == EntityOrientation.Left && !this._spriteRenderer.flipX)
			{
				this._spriteRenderer.flipX = true;
			}
			else if (base.EntityOwner.Status.Orientation == EntityOrientation.Right && this._spriteRenderer.flipX)
			{
				this._spriteRenderer.flipX = false;
			}
		}

		public Animator PushBackSparkAnimator;

		private readonly int _pushBackSparkAnimHash = Animator.StringToHash("PushBackSparks");

		private SpriteRenderer _spriteRenderer;
	}
}
