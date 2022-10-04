using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class IsidoraActivateCollisionAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<IsidoraAnimationEventsController>();
		}
		this.collisionOnPercentageUsed = false;
		this.eventsController.DoActivateCollisions(this.collisionOnEnter);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<IsidoraAnimationEventsController>();
		}
		if (this.useAnimationPercentage && !this.collisionOnPercentageUsed && stateInfo.normalizedTime >= this.percentageToUse)
		{
			this.eventsController.DoActivateCollisions(this.collisionOnPercentage);
			this.collisionOnPercentageUsed = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.eventsController.DoActivateCollisions(this.collisionOnExit);
	}

	public bool collisionOnEnter;

	public bool collisionOnExit;

	public bool useAnimationPercentage;

	[ShowIf("useAnimationPercentage", true)]
	[Range(0f, 1f)]
	public float percentageToUse;

	[ShowIf("useAnimationPercentage", true)]
	public bool collisionOnPercentage;

	private bool collisionOnPercentageUsed;

	private IsidoraAnimationEventsController eventsController;
}
