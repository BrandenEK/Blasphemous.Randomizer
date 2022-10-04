using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen flags exists.")]
	public class FlagExists : FsmStateAction
	{
		public override void Reset()
		{
			this.outValue = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			string id = this.flagName.Value.ToUpper().Replace(' ', '_');
			bool flag = Core.Events.GetFlag(id);
			if (this.outValue != null)
			{
				this.outValue.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(this.flagAvailable);
			}
			else
			{
				base.Fsm.Event(this.flagUnavailable);
			}
			base.Finish();
		}

		public FsmString category;

		public FsmString flagName;

		public bool runtimeFlag;

		public FsmEvent flagAvailable;

		public FsmEvent flagUnavailable;

		public FsmBool outValue;
	}
}
