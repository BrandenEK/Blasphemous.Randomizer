using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Checks if a given Game Mode is the one that is currently active.")]
	public class CheckGameModeActive : FsmStateAction
	{
		public override void OnEnter()
		{
			bool flag = Core.GameModeManager.CheckGameModeActive(this.mode.Value);
			if (flag)
			{
				base.Fsm.Event(this.modeIsActive);
			}
			else
			{
				base.Fsm.Event(this.modeIsInactive);
			}
			base.Finish();
		}

		[RequiredField]
		public FsmString mode;

		public FsmEvent modeIsActive;

		public FsmEvent modeIsInactive;
	}
}
