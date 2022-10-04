using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows the Penitence Selection Popup.")]
	public class ShowPenitenceSelectionPopup : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.ShowChoosePenitenceWidget(new Action(this.TriggerEventContinueAfterActivatingPenitence), new Action(this.TriggerEventContinueWithoutChoosingPenitence));
			base.Finish();
		}

		public void TriggerEventContinueAfterActivatingPenitence()
		{
			base.Fsm.Event(this.continueAfterActivatingPenitence);
		}

		public void TriggerEventContinueWithoutChoosingPenitence()
		{
			base.Fsm.Event(this.continueWithoutChoosingPenitence);
		}

		public FsmEvent continueAfterActivatingPenitence;

		public FsmEvent continueWithoutChoosingPenitence;
	}
}
