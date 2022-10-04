using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireAnimationInyector : EnemyAnimatorInyector
	{
		public void EventAnimation_Attack()
		{
			HomingBonfire homingBonfire = (HomingBonfire)this.OwnerEntity;
			HomingBonfireBehaviour homingBonfireBehaviour = homingBonfire.EnemyBehaviour as HomingBonfireBehaviour;
			if (homingBonfireBehaviour != null)
			{
				homingBonfireBehaviour.BonfireAttack.FireProjectile();
			}
		}

		public void Dispose()
		{
			if (!this.OwnerEntity)
			{
				return;
			}
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void SetParamHalf(bool paramValue)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(HomingBonfireAnimationInyector.HalfParam, paramValue);
		}

		public void SetParamFull(bool paramValue)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(HomingBonfireAnimationInyector.FullParam, paramValue);
		}

		public void SetParamExplode()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(HomingBonfireAnimationInyector.ExplodeParam);
		}

		private static readonly int HalfParam = Animator.StringToHash("HALF");

		private static readonly int FullParam = Animator.StringToHash("FULL");

		private static readonly int ExplodeParam = Animator.StringToHash("EXPLODE");
	}
}
