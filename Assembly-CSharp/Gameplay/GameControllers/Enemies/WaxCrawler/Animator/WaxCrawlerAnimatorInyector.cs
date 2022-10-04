using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.Animator
{
	public class WaxCrawlerAnimatorInyector : EnemyAnimatorInyector
	{
		public Animator Animator
		{
			get
			{
				return base.EntityAnimator;
			}
		}

		public bool EnableSpriteRenderer
		{
			get
			{
				return this._spriteRenderer.enabled;
			}
			set
			{
				this._spriteRenderer.enabled = value;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._waxCrawler = (WaxCrawler)this.OwnerEntity;
			base.EntityAnimator = this.OwnerEntity.Animator;
			this._spriteRenderer = this._waxCrawler.SpriteRenderer;
		}

		public void AnimatorSpeed(float speed)
		{
			if (base.EntityAnimator != null)
			{
				base.EntityAnimator.speed = Mathf.Clamp01(speed);
			}
		}

		public void Dead()
		{
			if (base.EntityAnimator != null)
			{
				base.EntityAnimator.SetTrigger("DEATH");
			}
		}

		public void Hide()
		{
			if (base.EntityAnimator != null)
			{
				base.EntityAnimator.SetTrigger("HIDE");
			}
		}

		public void Appear()
		{
			if (base.EntityAnimator != null)
			{
				base.EntityAnimator.Play("Appear", 0, 0f);
			}
		}

		public void Hurt()
		{
			if (base.EntityAnimator != null)
			{
				base.EntityAnimator.SetTrigger("HURT");
			}
		}

		private SpriteRenderer _spriteRenderer;

		private WaxCrawler _waxCrawler;
	}
}
