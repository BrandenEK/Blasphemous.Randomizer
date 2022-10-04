using System;
using Gameplay.GameControllers.Bosses.Quirce;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Bosses
{
	public class QuirceTeleportOutBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			QuirceBehaviour componentInParent = animator.GetComponentInParent<QuirceBehaviour>();
			if (componentInParent != null)
			{
				componentInParent.OnTeleportOutAnimationStarts();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			QuirceBehaviour componentInParent = animator.GetComponentInParent<QuirceBehaviour>();
			if (componentInParent != null)
			{
				componentInParent.OnTeleportOutAnimationFinished();
			}
		}
	}
}
