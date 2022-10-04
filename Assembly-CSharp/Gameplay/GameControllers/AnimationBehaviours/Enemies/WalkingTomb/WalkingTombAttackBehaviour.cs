using System;
using Gameplay.GameControllers.Enemies.WalkingTomb;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WalkingTomb
{
	public class WalkingTombAttackBehaviour : StateMachineBehaviour
	{
		protected WalkingTomb WalkingTomb { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.WalkingTomb == null)
			{
				this.WalkingTomb = animator.GetComponentInParent<WalkingTomb>();
			}
			this.WalkingTomb.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.WalkingTomb.IsAttacking = false;
		}
	}
}
