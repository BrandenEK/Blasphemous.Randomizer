using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows the Quote Popup.")]
	public class ShowQuotePopup : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.ShowQuoteWidget(this.fadeInTime.Value, this.timeActive.Value, this.fadeOutTime.Value, new Action(this.TriggerEventContinueAfterFadeOut));
			base.Finish();
		}

		public void TriggerEventContinueAfterFadeOut()
		{
			base.Fsm.Event(this.continueAfterFadeOut);
		}

		[RequiredField]
		public FsmFloat fadeInTime;

		[RequiredField]
		public FsmFloat timeActive;

		[RequiredField]
		public FsmFloat fadeOutTime;

		public FsmEvent continueAfterFadeOut;
	}
}
