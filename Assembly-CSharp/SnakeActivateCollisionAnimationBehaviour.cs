using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnakeActivateCollisionAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<SnakeAnimationEventsController>();
		}
		if (this.eventsController == null)
		{
			return;
		}
		this.collisionOnPercentageUsed = false;
		if (this.colliderType == SnakeActivateCollisionAnimationBehaviour.TONGUE_COLLIDER.OPEN_MOUTH)
		{
			this.eventsController.DoActivateCollisionsOpenMouth(this.collisionOnEnter);
		}
		else
		{
			this.eventsController.DoActivateCollisionsIdle(this.collisionOnEnter);
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<SnakeAnimationEventsController>();
		}
		if (this.eventsController == null)
		{
			return;
		}
		if (this.useAnimationPercentage && !this.collisionOnPercentageUsed && stateInfo.normalizedTime >= this.percentageToUse)
		{
			if (this.colliderType == SnakeActivateCollisionAnimationBehaviour.TONGUE_COLLIDER.OPEN_MOUTH)
			{
				this.eventsController.DoActivateCollisionsOpenMouth(this.collisionOnPercentage);
			}
			else
			{
				this.eventsController.DoActivateCollisionsIdle(this.collisionOnPercentage);
			}
			this.collisionOnPercentageUsed = true;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<SnakeAnimationEventsController>();
		}
		if (this.eventsController == null)
		{
			return;
		}
		if (this.colliderType == SnakeActivateCollisionAnimationBehaviour.TONGUE_COLLIDER.OPEN_MOUTH)
		{
			this.eventsController.DoActivateCollisionsOpenMouth(this.collisionOnExit);
		}
		else
		{
			this.eventsController.DoActivateCollisionsIdle(this.collisionOnExit);
		}
	}

	[EnumToggleButtons]
	public SnakeActivateCollisionAnimationBehaviour.TONGUE_COLLIDER colliderType;

	public bool collisionOnEnter;

	public bool collisionOnExit;

	public bool useAnimationPercentage;

	[ShowIf("useAnimationPercentage", true)]
	[Range(0f, 1f)]
	public float percentageToUse;

	[ShowIf("useAnimationPercentage", true)]
	public bool collisionOnPercentage;

	private bool collisionOnPercentageUsed;

	private SnakeAnimationEventsController eventsController;

	public enum TONGUE_COLLIDER
	{
		IDLE,
		OPEN_MOUTH
	}
}
