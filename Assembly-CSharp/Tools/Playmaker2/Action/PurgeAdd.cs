using System;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add or remove purge points.")]
	public class PurgeAdd : FsmStateAction
	{
		public override void OnEnter()
		{
			float num = (this.value == null) ? 0f : this.value.Value;
			bool showMessage = this.ShowMessage != null && this.ShowMessage.Value;
			Core.Randomizer.Log("PurgeAdd", 2);
			Core.Randomizer.giveReward(num + "." + base.Owner.name, showMessage);
			base.Finish();
		}

		private void DialogClose()
		{
			PopUpWidget.OnDialogClose -= this.DialogClose;
			base.Finish();
		}

		public FsmFloat value;

		public FsmBool ShowMessage;
	}
}
