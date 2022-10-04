using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.ElderBrother
{
	public class ElderBrotherAnimatorInyector : EnemyAnimatorInyector
	{
		public void AnimEvent_ActivateBarrier()
		{
			this.blockCollider.SetActive(true);
		}

		public void AnimEvent_DeactivateBarrier()
		{
			this.blockCollider.SetActive(false);
		}

		public void BigSmashPreparation()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("PREPARATION");
		}

		public void Smash()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("SMASH");
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void SetMidAir(bool midAir)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("MID-AIR", midAir);
		}

		public GameObject blockCollider;
	}
}
