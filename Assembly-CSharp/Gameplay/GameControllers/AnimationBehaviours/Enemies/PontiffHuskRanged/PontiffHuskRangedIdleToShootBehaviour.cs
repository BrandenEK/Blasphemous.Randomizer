using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHuskRanged
{
	public class PontiffHuskRangedIdleToShootBehaviour : StateMachineBehaviour
	{
		public PontiffHuskRanged PontiffHuskRanged { get; set; }

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.PontiffHuskRanged == null)
			{
				this.PontiffHuskRanged = animator.GetComponentInParent<PontiffHuskRanged>();
			}
			this.PontiffHuskRanged.IsAttacking = true;
		}
	}
}
