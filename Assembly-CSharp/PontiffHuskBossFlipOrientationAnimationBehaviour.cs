using System;
using UnityEngine;

public class PontiffHuskBossFlipOrientationAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<PontiffHuskBossAnimationEventsController>();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.eventsController.Animation_FlipOrientation();
	}

	private PontiffHuskBossAnimationEventsController eventsController;
}
