using System;
using Gameplay.GameControllers.Bosses.Crisanta;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Crisanta
{
	public class CrisantaHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Crisanta componentInParent = animator.GetComponentInParent<Crisanta>();
			if (stateInfo.normalizedTime > 0.95f && componentInParent.Behaviour.IsRecovering())
			{
				Debug.Log("CRISANTA HURT BEHAVIOR");
				componentInParent.Behaviour.OnHitReactionAnimationCompleted();
			}
		}
	}
}
