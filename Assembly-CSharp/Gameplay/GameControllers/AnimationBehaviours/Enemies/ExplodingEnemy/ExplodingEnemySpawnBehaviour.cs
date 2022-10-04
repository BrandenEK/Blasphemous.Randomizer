using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ExplodingEnemy
{
	public class ExplodingEnemySpawnBehaviour : StateMachineBehaviour
	{
		public ExplodingEnemy ExplodingEnemy { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy == null)
			{
				this.ExplodingEnemy = animator.GetComponentInParent<ExplodingEnemy>();
			}
			ExplodingEnemyBehaviour explodingEnemyBehaviour = (ExplodingEnemyBehaviour)this.ExplodingEnemy.EnemyBehaviour;
			explodingEnemyBehaviour.IsMelting = true;
			this.ExplodingEnemy.EntityAttack.transform.gameObject.SetActive(false);
			this.ExplodingEnemy.Audio.Appear();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			ExplodingEnemyBehaviour explodingEnemyBehaviour = (ExplodingEnemyBehaviour)this.ExplodingEnemy.EnemyBehaviour;
			explodingEnemyBehaviour.IsMelting = false;
			this.ExplodingEnemy.Status.CastShadow = true;
			this.ExplodingEnemy.EntityAttack.transform.gameObject.SetActive(true);
		}
	}
}
