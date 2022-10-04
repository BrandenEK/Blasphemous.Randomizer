using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Generic
{
	public class GenericEnemyAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.GenericEnemy == null)
			{
				this.GenericEnemy = animator.GetComponentInParent<Enemy>();
			}
			this.GenericEnemy.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.GenericEnemy.IsAttacking = false;
		}

		private Enemy GenericEnemy;
	}
}
