using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory(3)]
	[Tooltip("Checks if the flag is raised.")]
	public class CheckFlag : FsmStateAction
	{
		public override void OnEnter()
		{
			string id = this.flagName.Value.ToUpper().Replace(' ', '_');
			if (Core.Events.GetFlag(id))
			{
				base.Fsm.Event(this.flagAvailable);
			}
			else
			{
				base.Fsm.Event(this.flagUnavailable);
			}
			base.Finish();
		}

		public FsmString flagName;

		public FsmEvent flagAvailable;

		public FsmEvent flagUnavailable;
	}
}
