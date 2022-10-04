using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHuskRanged
{
	public class PontiffHuskRangedShootToIdleBehaviour : StateMachineBehaviour
	{
		public PontiffHuskRanged PontiffHuskRanged { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.PontiffHuskRanged == null)
			{
				this.PontiffHuskRanged = animator.GetComponentInParent<PontiffHuskRanged>();
			}
			this.PontiffHuskRanged.IsAttacking = false;
		}
	}
}
