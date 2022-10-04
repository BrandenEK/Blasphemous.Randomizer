using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class ParryDust : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.ParryDustAnimator == null)
			{
				Debug.LogError("An animator is needed!");
				return;
			}
			this._spriteRenderer = this.ParryDustAnimator.GetComponent<SpriteRenderer>();
		}

		public void TriggerParryDust()
		{
			if (this.ParryDustAnimator == null)
			{
				return;
			}
			this.FlipSpriteRenderer();
			this.ParryDustAnimator.Play(this._parryDustAnim, 0, 0f);
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

		public Animator ParryDustAnimator;

		private readonly int _parryDustAnim = Animator.StringToHash("ParryDust");

		private SpriteRenderer _spriteRenderer;
	}
}
