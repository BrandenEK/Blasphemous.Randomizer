using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class ThrowbackDust : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.ThrowbackDustAnimator == null)
			{
				Debug.LogError("An animator is needed!");
				return;
			}
			this._spriteRenderer = this.ThrowbackDustAnimator.GetComponent<SpriteRenderer>();
		}

		public void TriggerThrowbackDust()
		{
			if (this.ThrowbackDustAnimator == null)
			{
				return;
			}
			this.FlipSpriteRenderer();
			this.ThrowbackDustAnimator.Play(this._throwbackDustAnim, 0, 0f);
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

		public Animator ThrowbackDustAnimator;

		private readonly int _throwbackDustAnim = Animator.StringToHash("ThrowbackDust");

		private SpriteRenderer _spriteRenderer;
	}
}
