using System;
using Gameplay.GameControllers.Enemies.Nun.Attack;
using Gameplay.GameControllers.Enemies.Nun.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun.Animator
{
	public class NunAnimatorInyector : EnemyAnimatorInyector
	{
		public void TurnAround()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(NunAnimatorInyector.Turn);
		}

		public void Walk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(NunAnimatorInyector.WalkParam, true);
		}

		public void Stop()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(NunAnimatorInyector.WalkParam, false);
		}

		public void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(NunAnimatorInyector.AttackParam);
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(NunAnimatorInyector.DeathParam);
		}

		public void ResetCoolDownAttack()
		{
			NunBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<NunBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public void SetCurrentDamageType(DamageArea.DamageType damageType)
		{
			NunAttack componentInChildren = this.OwnerEntity.GetComponentInChildren<NunAttack>();
			if (componentInChildren != null)
			{
				componentInChildren.CurrentDamageType = damageType;
			}
		}

		public void SpawnOilPuddle()
		{
		}

		private static readonly int Turn = Animator.StringToHash("TURN");

		private static readonly int WalkParam = Animator.StringToHash("WALK");

		private static readonly int AttackParam = Animator.StringToHash("ATTACK");

		private static readonly int DeathParam = Animator.StringToHash("DEATH");
	}
}
