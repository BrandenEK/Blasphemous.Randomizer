using System;
using Gameplay.GameControllers.Bosses.PontiffOldman;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.PontiffOldman
{
	public class PontiffOldmanHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			PontiffOldman componentInParent = animator.GetComponentInParent<PontiffOldman>();
			if (stateInfo.normalizedTime > 0.95f && componentInParent.Behaviour.IsRecovering())
			{
				componentInParent.Behaviour.OnHitReactionAnimationCompleted();
			}
		}
	}
}
