using System;
using UnityEngine;

public class IsidoraResetSpeedBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.isidoraAnimationEventsController == null)
		{
			this.isidoraAnimationEventsController = animator.GetComponent<IsidoraAnimationEventsController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.isidoraAnimationEventsController.Animation_CheckFlagAndResetSpeed();
	}

	private IsidoraAnimationEventsController isidoraAnimationEventsController;
}
