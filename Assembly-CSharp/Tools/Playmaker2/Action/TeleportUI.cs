using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Show a Unlock skills UI")]
	public class TeleportUI : FsmStateAction
	{
		public override void OnEnter()
		{
			TeleportWidget.OnTeleportCancelled += this.OnTeleportCancelled;
			UIController.instance.ShowTeleportUI();
		}

		public override void OnExit()
		{
			TeleportWidget.OnTeleportCancelled -= this.OnTeleportCancelled;
		}

		public void OnTeleportCancelled()
		{
			base.Fsm.Event(this.TeleportCancelled);
			base.Finish();
		}

		public FsmEvent TeleportCancelled;
	}
}
