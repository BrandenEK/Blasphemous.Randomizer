using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ExplodingEnemy
{
	public class ExplodingEnemyChargeExplosionBehaviour : StateMachineBehaviour
	{
		public ExplodingEnemy ExplodingEnemy { get; private set; }

		public ExplodingEnemyBehaviour ExplodingEnemyBehaviour { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy == null)
			{
				this.ExplodingEnemy = animator.GetComponentInParent<ExplodingEnemy>();
				this.ExplodingEnemyBehaviour = this.ExplodingEnemy.GetComponent<ExplodingEnemyBehaviour>();
			}
			this.ExplodingEnemyBehaviour.IsChargingExplosion = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.ExplodingEnemyBehaviour.IsChargingExplosion = false;
		}
	}
}
