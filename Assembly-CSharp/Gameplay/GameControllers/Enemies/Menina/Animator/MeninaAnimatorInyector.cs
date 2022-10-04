using System;
using System.Diagnostics;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Menina.Animator
{
	public class MeninaAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnStepFinished;

		public void NotifyOnStepFinished()
		{
			if (this.OnStepFinished != null)
			{
				this.OnStepFinished();
			}
		}

		public void Attack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void ResetAttackTrigger()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("ATTACK");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Step(bool forward = true)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool((!forward) ? "STEP_BCK" : "STEP_FWD", true);
			if (base.EntityAnimator.GetBool("STEP_FWD"))
			{
				base.EntityAnimator.SetBool("STEP_BCK", false);
			}
			if (base.EntityAnimator.GetBool("STEP_BCK"))
			{
				base.EntityAnimator.SetBool("STEP_FWD", false);
			}
		}

		public void Stop()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("STEP_FWD", false);
			base.EntityAnimator.SetBool("STEP_BCK", false);
		}

		public void AttackEvent()
		{
			Menina menina = (Menina)this.OwnerEntity;
			menina.Attack.CurrentWeaponAttack();
			this.ResetAttackTrigger();
		}

		public void AnimatorEvent_Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}
	}
}
