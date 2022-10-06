using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Execution
{
	public class ExecutionAwareness : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Object.Destroy(animator.gameObject);
		}
	}
}
