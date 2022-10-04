using System;
using Gameplay.GameControllers.Enemies.WallEnemy;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WallEnemy
{
	public class WallEnemyDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._wallEnemy == null)
			{
				this._wallEnemy = animator.GetComponentInParent<WallEnemy>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			UnityEngine.Object.Destroy(this._wallEnemy.gameObject);
		}

		private WallEnemy _wallEnemy;
	}
}
