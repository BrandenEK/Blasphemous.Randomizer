using System;
using Gameplay.GameControllers.Enemies.Roller.AI;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.Animator
{
	public class AxeRollerAnimatorInjector : EnemyAnimatorInyector
	{
		public void Attack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void StopAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("STOP_ATTACK");
		}

		public void Damage()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("ATTACK");
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Rolling(bool isRolling)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ROLLING", isRolling);
			base.EntityAnimator.ResetTrigger("ATTACK");
			base.EntityAnimator.ResetTrigger("HURT");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
			this.StopMoving();
		}

		public void DestroyEnemy()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.OwnerEntity.gameObject);
			base.EntityAnimator.ResetTrigger("ATTACK");
		}

		public void StartMoving()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			AxeRoller axeRoller = (AxeRoller)this.OwnerEntity;
			AxeRollerBehaviour axeRollerBehaviour = (AxeRollerBehaviour)axeRoller.EnemyBehaviour;
			axeRollerBehaviour.Roll();
		}

		public void StopMoving()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			AxeRoller axeRoller = (AxeRoller)this.OwnerEntity;
			axeRoller.EnemyBehaviour.StopMovement();
		}

		public void StopAttackIfPenitentIsTooFar()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			AxeRoller axeRoller = (AxeRoller)this.OwnerEntity;
			axeRoller.EnemyBehaviour.StopMovement();
		}

		public void LaunchProjectile()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			AxeRoller axeRoller = (AxeRoller)this.OwnerEntity;
			axeRoller.AxeAttack.FireProjectile();
			axeRoller.IsAttacking = false;
		}
	}
}
