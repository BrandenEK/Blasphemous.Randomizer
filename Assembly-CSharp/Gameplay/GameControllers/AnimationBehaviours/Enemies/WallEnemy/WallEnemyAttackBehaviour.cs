using System;
using Gameplay.GameControllers.Enemies.WallEnemy;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WallEnemy
{
	public class WallEnemyAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._wallEnemy == null)
			{
				this._wallEnemy = animator.GetComponentInParent<WallEnemy>();
			}
			if (!this._wallEnemy.IsAttacking)
			{
				this._wallEnemy.IsAttacking = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._wallEnemy.IsAttacking)
			{
				this._wallEnemy.IsAttacking = false;
			}
		}

		private WallEnemy _wallEnemy;
	}
}
