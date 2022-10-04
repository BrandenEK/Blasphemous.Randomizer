using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ExplodingEnemy
{
	public class ExplodingEnemyTurnAround : StateMachineBehaviour
	{
		public ExplodingEnemy ExplodingEnemy { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy == null)
			{
				this.ExplodingEnemy = animator.GetComponentInParent<ExplodingEnemy>();
			}
			this.ExplodingEnemy.EnemyBehaviour.TurningAround = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.ExplodingEnemy.SetOrientation(this.ExplodingEnemy.Status.Orientation, true, false);
			this.ExplodingEnemy.EnemyBehaviour.TurningAround = false;
		}
	}
}
