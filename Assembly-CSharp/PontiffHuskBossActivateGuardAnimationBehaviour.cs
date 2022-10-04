using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PontiffHuskBossActivateGuardAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.activated = false;
		this.deactivated = false;
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<PontiffHuskBossAnimationEventsController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = Mathf.FloorToInt(stateInfo.normalizedTime);
		if (num > this.lastLoop)
		{
			this.lastLoop = num;
			this.activated = false;
			this.deactivated = false;
		}
		else
		{
			float num2 = stateInfo.normalizedTime - (float)num;
			if (this.activatesGuard && !this.activated && num2 >= this.percentageToActivateGuard)
			{
				this.eventsController.Animation_DoActivateGuard(true);
				this.activated = true;
			}
			if (this.deactivatesGuard && !this.deactivated && num2 >= this.percentageToDeactivateGuard)
			{
				this.eventsController.Animation_DoActivateGuard(false);
				this.deactivated = true;
			}
		}
	}

	public bool activatesGuard;

	public bool deactivatesGuard;

	[ShowIf("activatesGuard", true)]
	[Range(0f, 1f)]
	public float percentageToActivateGuard;

	[ShowIf("deactivatesGuard", true)]
	[Range(0f, 1f)]
	public float percentageToDeactivateGuard;

	private PontiffHuskBossAnimationEventsController eventsController;

	private bool activated;

	private bool deactivated;

	private int lastLoop = -1;
}
