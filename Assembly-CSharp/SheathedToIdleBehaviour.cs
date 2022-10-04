using System;
using Framework.Managers;
using UnityEngine;

public class SheathedToIdleBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		Core.Input.SetBlocker("PLAYER_LOGIC", true);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		Core.Input.SetBlocker("PLAYER_LOGIC", false);
	}
}
