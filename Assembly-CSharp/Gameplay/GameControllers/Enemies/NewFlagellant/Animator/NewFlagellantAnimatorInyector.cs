using System;
using System.Diagnostics;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.Animator
{
	public class NewFlagellantAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<NewFlagellantAnimatorInyector> OnAttackAnimationFinished;

		public void AttackAnimationFinished()
		{
			if (this.OnAttackAnimationFinished != null)
			{
				this.OnAttackAnimationFinished(this);
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		public void Falling()
		{
			base.EntityAnimator.SetTrigger("FALL");
		}

		public void Landing()
		{
			base.EntityAnimator.SetTrigger("LAND");
		}

		public void Dash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DASH");
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("HURT", 0, 0f);
		}

		public void ResetDash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("DASH");
		}

		public void Run(bool run)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("RUN", run);
		}

		public void Walk(bool walk)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", walk);
		}

		public void Attack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", true);
		}

		public void FastAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("FASTATTACK");
		}

		public void StopAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", false);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			this.IsParried(false);
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void TriggerDash()
		{
			NewFlagellant newFlagellant = (NewFlagellant)this.OwnerEntity;
			newFlagellant.NewFlagellantBehaviour.Dash();
		}

		public void AnimationEvent_HeavyAttack()
		{
			NewFlagellant newFlagellant = (NewFlagellant)this.OwnerEntity;
			newFlagellant.Attack.CurrentWeaponAttack();
		}

		public void AnimationEvent_FastAttack()
		{
			NewFlagellant newFlagellant = (NewFlagellant)this.OwnerEntity;
			newFlagellant.FastAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_AttackDisplacement()
		{
			NewFlagellant newFlagellant = (NewFlagellant)this.OwnerEntity;
			newFlagellant.NewFlagellantBehaviour.AttackDisplacement();
		}

		public void IsParried(bool isParried)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("IS_PARRIED", isParried);
		}
	}
}
