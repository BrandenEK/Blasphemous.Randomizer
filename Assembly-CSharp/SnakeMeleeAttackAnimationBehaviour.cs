using System;
using Gameplay.GameControllers.Bosses.Snake;
using UnityEngine;

public class SnakeMeleeAttackAnimationBehaviour : StateMachineBehaviour
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
		this.started = false;
		this.finished = false;
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
		if (!this.started && stateInfo.normalizedTime >= this.percentageStart)
		{
			this.eventsController.Animation_SetWeapon(this.weaponToUse);
			this.eventsController.Animation_OnMeleeAttackStarts();
			this.started = true;
		}
		if (!this.finished && stateInfo.normalizedTime >= this.percentageEnd && !this.alwaysActive)
		{
			this.eventsController.Animation_OnMeleeAttackFinished();
			this.finished = true;
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
		this.eventsController.Animation_OnMeleeAttackFinished();
	}

	public float percentageStart;

	public float percentageEnd;

	public bool alwaysActive;

	public SnakeBehaviour.SNAKE_WEAPONS weaponToUse;

	private SnakeAnimationEventsController eventsController;

	private bool started;

	private bool finished;
}
