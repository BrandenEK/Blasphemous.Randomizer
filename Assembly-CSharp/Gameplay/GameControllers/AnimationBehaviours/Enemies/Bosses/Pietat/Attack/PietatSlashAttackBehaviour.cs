using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Attack
{
	public class PietatSlashAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			if (!this._pietyMonster.IsAttacking)
			{
				this._pietyMonster.IsAttacking = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._pietyMonster.PietyBehaviour.Attacking)
			{
				this._pietyMonster.PietyBehaviour.Attacking = false;
			}
			if (this._pietyMonster.IsAttacking)
			{
				this._pietyMonster.IsAttacking = false;
			}
			animator.ResetTrigger("SLASH_ATTACK");
		}

		private PietyMonster _pietyMonster;
	}
}
