using System;
using UnityEngine;

public class AmanecidaMeleeAttackAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.started = false;
		this.finished = false;
		if (this.amanecidasAnimationEventsController == null)
		{
			this.amanecidasAnimationEventsController = animator.GetComponent<AmanecidasAnimationEventsController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this.started && stateInfo.normalizedTime >= this.percentageStart)
		{
			this.amanecidasAnimationEventsController.AnimationEvent_MeleeAttackStart();
			this.started = true;
		}
		if (!this.finished && stateInfo.normalizedTime >= this.percentageEnd && !this.alwaysActive)
		{
			this.amanecidasAnimationEventsController.AnimationEvent_MeleeAttackFinished();
			this.finished = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.amanecidasAnimationEventsController.AnimationEvent_MeleeAttackFinished();
	}

	public float percentageStart;

	public float percentageEnd;

	public bool alwaysActive;

	private AmanecidasAnimationEventsController amanecidasAnimationEventsController;

	private bool started;

	private bool finished;
}
