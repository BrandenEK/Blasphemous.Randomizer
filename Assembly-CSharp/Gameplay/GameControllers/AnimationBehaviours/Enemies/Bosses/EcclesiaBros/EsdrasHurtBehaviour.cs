using System;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.EcclesiaBros
{
	public class EsdrasHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Esdras componentInParent = animator.GetComponentInParent<Esdras>();
			if (stateInfo.normalizedTime > 0.95f && componentInParent.Behaviour.IsRecovering())
			{
				Debug.Log("ESDRAS HURT BEHAVIOR");
				componentInParent.Behaviour.OnHitReactionAnimationCompleted();
			}
		}
	}
}
