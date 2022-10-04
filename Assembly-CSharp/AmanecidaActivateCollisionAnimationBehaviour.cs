using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmanecidaActivateCollisionAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<AmanecidasAnimationEventsController>().DoActivateCollisions(this.collisionOnEnter);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.useAnimationPercentage && !this.collisionOnPercentageUsed && stateInfo.normalizedTime >= this.percentageToUse)
		{
			animator.GetComponent<AmanecidasAnimationEventsController>().DoActivateCollisions(this.collisionOnPercentage);
			this.collisionOnPercentageUsed = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<AmanecidasAnimationEventsController>().DoActivateCollisions(this.collisionOnExit);
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
}
