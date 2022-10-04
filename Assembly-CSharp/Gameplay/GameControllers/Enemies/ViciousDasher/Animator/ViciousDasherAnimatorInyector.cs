using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.Animator
{
	public class ViciousDasherAnimatorInyector : EnemyAnimatorInyector
	{
		public void Dash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(ViciousDasherAnimatorInyector.Dash1);
		}

		public void ResetDash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(ViciousDasherAnimatorInyector.Dash1);
		}

		public void Attack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool(ViciousDasherAnimatorInyector.Attack1, true);
		}

		public void StopAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool(ViciousDasherAnimatorInyector.Attack1, false);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			this.IsParried(false);
			base.EntityAnimator.SetTrigger(ViciousDasherAnimatorInyector.Death1);
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void TriggerDash()
		{
			ViciousDasher viciousDasher = (ViciousDasher)this.OwnerEntity;
			viciousDasher.ViciousDasherBehaviour.Dash();
		}

		public void AttackEvent()
		{
			ViciousDasher viciousDasher = (ViciousDasher)this.OwnerEntity;
			viciousDasher.Attack.CurrentWeaponAttack();
		}

		public void IsParried(bool isParried)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool(ViciousDasherAnimatorInyector.Parried, isParried);
		}

		private static readonly int Parried = Animator.StringToHash("IS_PARRIED");

		private static readonly int Dash1 = Animator.StringToHash("DASH");

		private static readonly int Attack1 = Animator.StringToHash("ATTACK");

		private static readonly int Death1 = Animator.StringToHash("DEATH");
	}
}
