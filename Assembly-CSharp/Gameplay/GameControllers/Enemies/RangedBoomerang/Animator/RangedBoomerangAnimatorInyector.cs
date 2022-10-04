using System;
using Gameplay.GameControllers.Enemies.RangedBoomerang.Attack;
using Gameplay.GameControllers.Enemies.RangedBoomerang.IA;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang.Animator
{
	public class RangedBoomerangAnimatorInyector : EnemyAnimatorInyector
	{
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

		public void Recover()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("RECOVER");
		}

		public void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("DEATH", true);
		}

		public void AttackAnimationEvent()
		{
			RangedBoomerang rangedBoomerang = (RangedBoomerang)this.OwnerEntity;
			rangedBoomerang.Attack.CurrentWeaponAttack();
		}

		public void DisposeEnemy()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void ResetCoolDownAttack()
		{
			RangedBoomerangBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<RangedBoomerangBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public RangedBoomerangAttack attack;
	}
}
