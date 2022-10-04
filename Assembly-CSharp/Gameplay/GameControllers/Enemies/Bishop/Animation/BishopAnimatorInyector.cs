using System;
using Gameplay.GameControllers.Enemies.Bishop.AI;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Bishop.Animation
{
	public class BishopAnimatorInyector : EnemyAnimatorInyector
	{
		public void TurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(BishopAnimatorInyector.Turn);
		}

		public void Chasing(bool isChasing)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.AttackParam);
			base.EntityAnimator.SetBool(BishopAnimatorInyector.ChasingParam, isChasing);
		}

		public void Attack()
		{
			if (base.EntityAnimator == null || this.OwnerEntity.Status.Dead)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(BishopAnimatorInyector.AttackParam);
		}

		public void Damage()
		{
			if (base.EntityAnimator == null || this.OwnerEntity.Status.Dead)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.Turn);
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.AttackParam);
			base.EntityAnimator.SetTrigger(BishopAnimatorInyector.Hurt);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.Hurt);
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.AttackParam);
			base.EntityAnimator.Play("Death");
		}

		public void Idle()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.Hurt);
			base.EntityAnimator.ResetTrigger(BishopAnimatorInyector.AttackParam);
			base.EntityAnimator.SetBool(BishopAnimatorInyector.ChasingParam, false);
		}

		public void SpearAttack()
		{
			Bishop bishop = (Bishop)this.OwnerEntity;
			BishopBehaviour componentInChildren = bishop.GetComponentInChildren<BishopBehaviour>();
			if (!componentInChildren || componentInChildren.IsExecuted)
			{
				return;
			}
			bishop.Attack.CurrentWeaponAttack();
		}

		private static readonly int Turn = Animator.StringToHash("TURN");

		private static readonly int AttackParam = Animator.StringToHash("ATTACK");

		private static readonly int ChasingParam = Animator.StringToHash("CHASING");

		private static readonly int Hurt = Animator.StringToHash("HURT");
	}
}
