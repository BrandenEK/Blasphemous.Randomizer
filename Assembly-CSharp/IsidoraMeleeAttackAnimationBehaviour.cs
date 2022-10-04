using System;
using Gameplay.GameControllers.Bosses.Isidora;
using Sirenix.OdinInspector;
using UnityEngine;

public class IsidoraMeleeAttackAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.started = false;
		this.finished = false;
		this.flipped = false;
		if (this.isidoraAnimationEventsController == null)
		{
			this.isidoraAnimationEventsController = animator.GetComponent<IsidoraAnimationEventsController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this.started && stateInfo.normalizedTime >= this.percentageStart)
		{
			this.isidoraAnimationEventsController.Animation_SetWeapon(this.weaponToUse);
			this.isidoraAnimationEventsController.Animation_OnMeleeAttackStarts();
			this.started = true;
		}
		if (this.flipCollider && !this.flipped && stateInfo.normalizedTime >= this.percentageToFlipCollider)
		{
			this.isidoraAnimationEventsController.Animation_FlipCollider();
			this.flipped = true;
		}
		if (!this.finished && stateInfo.normalizedTime >= this.percentageEnd && !this.alwaysActive)
		{
			this.isidoraAnimationEventsController.Animation_OnMeleeAttackFinished();
			this.finished = true;
			if (this.flipped)
			{
				this.isidoraAnimationEventsController.Animation_FlipCollider();
			}
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.isidoraAnimationEventsController.Animation_OnMeleeAttackFinished();
		if (this.flipped)
		{
			this.isidoraAnimationEventsController.Animation_FlipCollider();
		}
	}

	public float percentageStart;

	public float percentageEnd;

	public bool alwaysActive;

	public IsidoraBehaviour.ISIDORA_WEAPONS weaponToUse;

	public bool flipCollider;

	[ShowIf("flipCollider", true)]
	public float percentageToFlipCollider;

	private IsidoraAnimationEventsController isidoraAnimationEventsController;

	private bool started;

	private bool finished;

	private bool flipped;
}
