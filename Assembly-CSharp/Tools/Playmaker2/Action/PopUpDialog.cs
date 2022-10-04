using System;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Show a PopUp.")]
	public class PopUpDialog : FsmStateAction
	{
		public override void OnEnter()
		{
			string message = Core.Localization.Get(this.textId.Value);
			this.IsModal = this.blockplayer.Value;
			if (this.IsModal)
			{
				PopUpWidget.OnDialogClose += this.DialogClose;
			}
			UIController.instance.ShowPopUp(message, this.soundId.Value, this.timeToWait.Value, this.blockplayer.Value);
			if (!this.IsModal)
			{
				base.Finish();
			}
		}

		public override void OnExit()
		{
			if (!this.IsModal)
			{
				PopUpWidget.OnDialogClose -= this.DialogClose;
			}
		}

		private void DialogClose()
		{
			base.Finish();
		}

		[Tooltip("String ID with message")]
		public FsmString textId;

		public FsmBool blockplayer;

		public FsmFloat timeToWait;

		public FsmString soundId;

		private bool IsModal;
	}
}
