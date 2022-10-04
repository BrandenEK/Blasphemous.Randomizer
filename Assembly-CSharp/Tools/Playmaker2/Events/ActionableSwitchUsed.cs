using System;
using HutongGames.PlayMaker;
using Tools.Level.Actionables;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an interactable is interacted.")]
	public class ActionableSwitchUsed : FsmStateAction
	{
		public override void OnEnter()
		{
			base.Owner.GetComponent<ActionableSwitch>().OnSwitchUsed += this.OnSwitchUsed;
		}

		private void OnSwitchUsed(ActionableSwitch go)
		{
			base.Finish();
		}

		public override void OnExit()
		{
			base.Owner.GetComponent<ActionableSwitch>().OnSwitchUsed -= this.OnSwitchUsed;
		}
	}
}
