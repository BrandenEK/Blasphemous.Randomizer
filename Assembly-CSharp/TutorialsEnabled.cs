using System;
using Framework.Managers;
using Framework.Util;
using HutongGames.PlayMaker;
using UnityEngine;

public class TutorialsEnabled : FsmStateAction
{
	public override void OnEnter()
	{
		if (this.tutorialsDisabledEvent != null && this.tutorialsEnabledEvent != null)
		{
			if (Singleton<Core>.Instance != null && Core.TutorialManager != null)
			{
				base.Fsm.Event((!Core.TutorialManager.TutorialsEnabled) ? this.tutorialsDisabledEvent : this.tutorialsEnabledEvent);
			}
			else
			{
				Debug.LogError("Can't find TutorialManager.");
			}
		}
		else
		{
			Debug.LogWarning("IsTutorialEnabled: Events are not defined");
		}
	}

	public FsmEvent tutorialsEnabledEvent;

	public FsmEvent tutorialsDisabledEvent;
}
