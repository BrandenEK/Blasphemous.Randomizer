using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ExplodingEnemy
{
	public class ExplodingEnemyVanishBehaviour : StateMachineBehaviour
	{
		private ExplodingEnemy ExplodingEnemy { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy == null)
			{
				this.ExplodingEnemy = animator.GetComponentInParent<ExplodingEnemy>();
			}
			ExplodingEnemyBehaviour explodingEnemyBehaviour = (ExplodingEnemyBehaviour)this.ExplodingEnemy.EnemyBehaviour;
			explodingEnemyBehaviour.IsMelting = true;
			this.ExplodingEnemy.Status.CastShadow = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy.IsSummoned && !this.ExplodingEnemy.ReekLeader.Status.Dead)
			{
				this.ExplodingEnemy.ReekLeader.Behaviour.ReekSpawner.DisposeReek(this.ExplodingEnemy.gameObject);
			}
			Object.Destroy(this.ExplodingEnemy.gameObject);
		}
	}
}
