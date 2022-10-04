using System;
using UnityEngine;

public class LaudesBowVisibilityAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<AmanecidasAnimationEventsController>().AnimationEvent_ShowWeaponIfBowLaudes();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<AmanecidasAnimationEventsController>().AnimationEvent_HideWeaponIfBowLaudes();
	}
}
