using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PontiffHuskBossActivateCollisionAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<PontiffHuskBossAnimationEventsController>();
		}
		this.collisionOnPercentageUsed = false;
		this.eventsController.Animation_DoActivateCollisions(this.collisionOnEnter);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<PontiffHuskBossAnimationEventsController>();
		}
		if (this.useAnimationPercentage && !this.collisionOnPercentageUsed && stateInfo.normalizedTime >= this.percentageToUse)
		{
			this.eventsController.Animation_DoActivateCollisions(this.collisionOnExit);
			this.collisionOnPercentageUsed = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.eventsController.Animation_DoActivateCollisions(this.collisionOnExit);
	}

	public bool collisionOnEnter;

	public bool collisionOnExit;

	public bool useAnimationPercentage;

	[ShowIf("useAnimationPercentage", true)]
	[Range(0f, 1f)]
	public float percentageToUse;

	private bool collisionOnPercentageUsed;

	private PontiffHuskBossAnimationEventsController eventsController;
}
