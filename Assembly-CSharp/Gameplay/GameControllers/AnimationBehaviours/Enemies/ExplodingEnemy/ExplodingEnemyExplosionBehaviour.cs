using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ExplodingEnemy
{
	public class ExplodingEnemyExplosionBehaviour : StateMachineBehaviour
	{
		private ExplodingEnemy ExplodingEnemy { get; set; }

		private ExplodingEnemyBehaviour ExplodingEnemyBehaviour { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ExplodingEnemy == null)
			{
				this.ExplodingEnemy = animator.GetComponentInParent<ExplodingEnemy>();
				this.ExplodingEnemyBehaviour = this.ExplodingEnemy.GetComponent<ExplodingEnemyBehaviour>();
			}
			this.ExplodingEnemy.EntityAttack.CurrentWeaponAttack();
			this.ExplodingEnemyBehaviour.IsExploding = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.ExplodingEnemyBehaviour.IsExploding = false;
			if (this.ExplodingEnemy.IsSummoned)
			{
				this.ExplodingEnemy.ReekLeader.Behaviour.ReekSpawner.DisposeReek(this.ExplodingEnemy.gameObject);
			}
			Object.Destroy(this.ExplodingEnemy.gameObject);
		}
	}
}
