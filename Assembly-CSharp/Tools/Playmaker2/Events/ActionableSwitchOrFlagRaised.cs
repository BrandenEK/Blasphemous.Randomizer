using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.Level.Actionables;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when a flag has been set to true or an interactable is interacted.")]
	public class ActionableSwitchOrFlagRaised : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.OnFlagChanged += this.OnFlagChanged;
			base.Owner.GetComponent<ActionableSwitch>().OnSwitchUsed += this.OnSwitchUsed;
		}

		private void OnFlagChanged(string flag, bool flagactive)
		{
			if (this.flagName.Value == flag && flagactive)
			{
				base.Fsm.Event(this.onFlagChanged);
				base.Finish();
			}
		}

		private void OnSwitchUsed(ActionableSwitch go)
		{
			base.Fsm.Event(this.onSwitchUsed);
			base.Finish();
		}

		public override void OnExit()
		{
			Core.Events.OnFlagChanged += this.OnFlagChanged;
			base.Owner.GetComponent<ActionableSwitch>().OnSwitchUsed -= this.OnSwitchUsed;
		}

		public FsmString category;

		public FsmString flagName;

		public bool runtimeFlag;

		public FsmEvent onFlagChanged;

		public FsmEvent onSwitchUsed;
	}
}
