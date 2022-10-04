using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows the Penitence Abandonment Popup.")]
	public class ShowPenitenceAbandonmentPopup : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.ShowAbandonPenitenceWidget(new Action(this.TriggerEventContinueAfterAbandoningPenitence), new Action(this.TriggerEventContinueWithoutAbandoningPenitence));
			base.Finish();
		}

		public void TriggerEventContinueAfterAbandoningPenitence()
		{
			base.Fsm.Event(this.continueAfterAbandoningPenitence);
		}

		public void TriggerEventContinueWithoutAbandoningPenitence()
		{
			base.Fsm.Event(this.continueWithoutAbandoningPenitence);
		}

		public FsmEvent continueAfterAbandoningPenitence;

		public FsmEvent continueWithoutAbandoningPenitence;
	}
}
