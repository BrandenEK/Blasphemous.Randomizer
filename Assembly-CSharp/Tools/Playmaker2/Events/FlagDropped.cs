using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when a flag has been set to false.")]
	public class FlagDropped : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.OnFlagChanged += this.OnFlagChanged;
		}

		private void OnFlagChanged(string flag, bool flagactive)
		{
			if (this.flagName.Value == flag && !flagactive)
			{
				base.Fsm.Event(this.onSuccess);
				base.Finish();
			}
		}

		public override void OnExit()
		{
			Core.Events.OnFlagChanged += this.OnFlagChanged;
		}

		public FsmString category;

		public FsmString flagName;

		public bool runtimeFlag;

		public FsmEvent onSuccess;
	}
}
