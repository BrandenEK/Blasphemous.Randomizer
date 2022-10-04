using System;
using Gameplay.GameControllers.Enemies.Firethrower.Attack;
using Gameplay.GameControllers.Enemies.Firethrower.IA;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Firethrower.Animator
{
	public class FirethrowerAnimatorInyector : EnemyAnimatorInyector
	{
		public void TurnAround()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN");
		}

		public void Walk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", true);
		}

		public void Stop()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
		}

		public void Charge()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("CHARGE");
		}

		public void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", true);
		}

		public void StopAttack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", false);
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void AttackAnimationEvent()
		{
			Firethrower firethrower = (Firethrower)this.OwnerEntity;
			firethrower.Attack.CurrentWeaponAttack();
		}

		public void FireStartAnimationEvent()
		{
			this.attack.SetFireLevel(FirethrowerAttack.FIRE_LEVEL.START);
		}

		public void FireGrowingtAnimationEvent()
		{
			this.attack.SetFireLevel(FirethrowerAttack.FIRE_LEVEL.GROWING);
		}

		public void FireMainAnimationEvent()
		{
			this.attack.SetFireLevel(FirethrowerAttack.FIRE_LEVEL.LOOP);
		}

		public void AttackEndAnimationEvent()
		{
			this.attack.SetFireLevel(FirethrowerAttack.FIRE_LEVEL.NONE);
		}

		public void ResetCoolDownAttack()
		{
			FirethrowerBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<FirethrowerBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public void Dispose()
		{
			this.attack.SetFireLevel(FirethrowerAttack.FIRE_LEVEL.NONE);
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public FirethrowerAttack attack;
	}
}
