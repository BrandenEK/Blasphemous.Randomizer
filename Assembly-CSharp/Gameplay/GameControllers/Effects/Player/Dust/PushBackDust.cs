using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class PushBackDust : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.PushBackDustAnimator == null)
			{
				Debug.LogError("An animator is needed!");
				return;
			}
			this._spriteRenderer = this.PushBackDustAnimator.GetComponent<SpriteRenderer>();
		}

		public void TriggerPushBackDust()
		{
			bool flag = Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			if (!this.PushBackDustAnimator || flag)
			{
				return;
			}
			this.FlipSpriteRenderer();
			this.PushBackDustAnimator.Play(this._pushBackDustAnimHash, 0, 0f);
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

		public Animator PushBackDustAnimator;

		private readonly int _pushBackDustAnimHash = Animator.StringToHash("PushBackDust");

		private SpriteRenderer _spriteRenderer;
	}
}
