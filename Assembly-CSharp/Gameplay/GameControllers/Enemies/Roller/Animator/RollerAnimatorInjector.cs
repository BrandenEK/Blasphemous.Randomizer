using System;
using Gameplay.GameControllers.Enemies.Roller.Attack;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.Animator
{
	public class RollerAnimatorInjector : EnemyAnimatorInyector
	{
		public void Attack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void Rolling(bool isRolling)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ROLLING", isRolling);
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

		public void DestroyEnemy()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			Object.Destroy(this.OwnerEntity.gameObject);
			base.EntityAnimator.ResetTrigger("ATTACK");
		}

		public void LaunchProjectile()
		{
			RollerAttack rollerAttack = (RollerAttack)this.OwnerEntity.EntityAttack;
			rollerAttack.FireProjectile();
		}
	}
}
