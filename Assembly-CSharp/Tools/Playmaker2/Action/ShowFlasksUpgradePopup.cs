using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows the Flasks Upgrade Popup.")]
	public class ShowFlasksUpgradePopup : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.ShowUpgradeFlasksWidget(this.price.Value, new Action(this.TriggerEventUpgradeFlasks), new Action(this.TriggerEventContinueWithoutUpgrading));
			base.Finish();
		}

		public void TriggerEventUpgradeFlasks()
		{
			base.Fsm.Event(this.upgradeFlasks);
		}

		public void TriggerEventContinueWithoutUpgrading()
		{
			base.Fsm.Event(this.continueWithoutUpgrading);
		}

		[RequiredField]
		public FsmFloat price;

		public FsmEvent upgradeFlasks;

		public FsmEvent continueWithoutUpgrading;
	}
}
