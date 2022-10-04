using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Animation
{
	public class BejeweledSaintAnimatorInyector : EnemyAnimatorInyector
	{
		public void HeadDamage()
		{
			if (this.OwnerEntity.Animator == null)
			{
				return;
			}
			this.OwnerEntity.Animator.SetTrigger("HURT");
		}

		public void BasicStaffAttack()
		{
			if (this.ArmAnimator == null)
			{
				return;
			}
			this.ArmAnimator.SetTrigger("BASIC_ATTACK");
		}

		public void DoSweep(bool isSweep)
		{
			if (this.ArmAnimator == null)
			{
				return;
			}
			this.ArmAnimator.SetBool("IS_SWEEPING", isSweep);
		}

		public void SetCackle(bool cackle)
		{
			this.OwnerEntity.Animator.SetBool("CACKLE", cackle);
		}

		public void ResetAttack()
		{
			this.ArmAnimator.ResetTrigger("BASIC_ATTACK");
			this.ArmAnimator.SetBool("IS_SWEEPING", false);
		}

		public Animator ArmAnimator;
	}
}
