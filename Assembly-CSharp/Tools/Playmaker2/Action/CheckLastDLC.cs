using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Checks the last DLC installed")]
	public class CheckLastDLC : FsmStateAction
	{
		public override void OnEnter()
		{
			if (Core.DLCManager.IsThirdDLCInstalled())
			{
				base.Fsm.Event(this.lastDLCIsThree);
			}
			else if (Core.DLCManager.IsSecondDLCInstalled())
			{
				base.Fsm.Event(this.lastDLCIsTwo);
			}
			else if (Core.DLCManager.IsFirstDLCInstalled())
			{
				base.Fsm.Event(this.lastDLCIsOne);
			}
			base.Finish();
		}

		public FsmEvent lastDLCIsOne;

		public FsmEvent lastDLCIsTwo;

		public FsmEvent lastDLCIsThree;
	}
}
