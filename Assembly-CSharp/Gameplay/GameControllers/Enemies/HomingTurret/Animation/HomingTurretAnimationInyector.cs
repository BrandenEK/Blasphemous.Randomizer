using System;
using Gameplay.GameControllers.Enemies.HomingTurret.AI;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.Animation
{
	public class HomingTurretAnimationInyector : EnemyAnimatorInyector
	{
		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(HomingTurretAnimationInyector.DeathParam, true);
		}

		public void Damage()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(HomingTurretAnimationInyector.DamageParam);
		}

		public void ChargeAttack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(HomingTurretAnimationInyector.HoldParam, true);
			base.EntityAnimator.SetTrigger(HomingTurretAnimationInyector.Charge);
		}

		public void ReleaseAttack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(HomingTurretAnimationInyector.HoldParam, false);
		}

		public void EventAnimation_Attack()
		{
			HomingTurret homingTurret = (HomingTurret)this.OwnerEntity;
			HomingTurretBehaviour homingTurretBehaviour = homingTurret.EnemyBehaviour as HomingTurretBehaviour;
			if (homingTurretBehaviour != null)
			{
				homingTurretBehaviour.TurretAttack.FireProjectileToPenitent();
			}
		}

		public void Spawn()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(HomingTurretAnimationInyector.SpawnParam);
		}

		public void Dispose()
		{
			if (!this.OwnerEntity)
			{
				return;
			}
			this.OwnerEntity.gameObject.SetActive(false);
		}

		private static readonly int DeathParam = Animator.StringToHash("DEAD");

		private static readonly int Charge = Animator.StringToHash("CHARGE");

		private static readonly int DamageParam = Animator.StringToHash("DAMAGE");

		private static readonly int SpawnParam = Animator.StringToHash("SPAWN");

		private static readonly int HoldParam = Animator.StringToHash("HOLD");
	}
}
