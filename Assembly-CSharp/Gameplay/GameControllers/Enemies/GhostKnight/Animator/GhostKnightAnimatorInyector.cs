using System;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GhostKnight.Animator
{
	public class GhostKnightAnimatorInyector : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._colorFlash = this.OwnerEntity.Animator.GetComponent<ColorFlash>();
		}

		public void Damage()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(GhostKnightAnimatorInyector.Hurt);
		}

		public void ParryReaction()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("ParryReaction");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(GhostKnightAnimatorInyector.DeathTrigger);
		}

		public void AttackClue()
		{
			if (base.EntityAnimator == null || (this.OwnerEntity != null && this.OwnerEntity.Status.Dead))
			{
				return;
			}
			base.EntityAnimator.Play(this._attackClueAnim);
		}

		public void Attack()
		{
			if (base.EntityAnimator == null || (this.OwnerEntity != null && this.OwnerEntity.Status.Dead))
			{
				return;
			}
			base.EntityAnimator.Play(this._attackAnim);
		}

		public void Appear()
		{
			if (base.EntityAnimator == null || (this.OwnerEntity != null && this.OwnerEntity.Status.Dead))
			{
				return;
			}
			base.EntityAnimator.Play(this._appearAnim);
		}

		public void AttackToIdle()
		{
			if (base.EntityAnimator == null || (this.OwnerEntity != null && this.OwnerEntity.Status.Dead))
			{
				return;
			}
			base.EntityAnimator.Play(this._attackToIdleAnim);
		}

		public void ColorFlash(Color color)
		{
			this._colorFlash.FlashColor = color;
			this._colorFlash.TriggerColorFlash();
		}

		private readonly int _appearAnim = Animator.StringToHash("Appear");

		private readonly int _attackAnim = Animator.StringToHash("Attack");

		private readonly int _attackClueAnim = Animator.StringToHash("AttackClue");

		private readonly int _attackToIdleAnim = Animator.StringToHash("AttackToIdle");

		private ColorFlash _colorFlash;

		private static readonly int DeathTrigger = Animator.StringToHash("DEATH");

		private static readonly int Hurt = Animator.StringToHash("HURT");
	}
}
