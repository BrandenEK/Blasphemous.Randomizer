using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnakePoisonAreaAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.timeWaited = 0f;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.timeWaited += Time.deltaTime;
		if (this.timeWaited > this.WaitTimeBeforeSettingTrigger)
		{
			if (this.TriggerToSet == SnakePoisonAreaAnimationBehaviour.POSION_AREA_TRIGGER.SHOW)
			{
				animator.SetTrigger(SnakePoisonAreaAnimationBehaviour.T_SHOW);
			}
			else
			{
				animator.SetTrigger(SnakePoisonAreaAnimationBehaviour.T_HIDE);
			}
		}
	}

	private static readonly int T_SHOW = Animator.StringToHash("SHOW");

	private static readonly int T_HIDE = Animator.StringToHash("HIDE");

	[EnumToggleButtons]
	public SnakePoisonAreaAnimationBehaviour.POSION_AREA_TRIGGER TriggerToSet;

	public float WaitTimeBeforeSettingTrigger;

	private float timeWaited;

	public enum POSION_AREA_TRIGGER
	{
		SHOW,
		HIDE
	}
}
