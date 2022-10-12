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
			Core.Randomizer.Log("PurgeAdd(" + num + ")", 2);
			if (num > 0f)
			{
				bool showMessage = this.ShowMessage != null && this.ShowMessage.Value;
				if (num == 18000f)
				{
					Core.Randomizer.giveReward(num + "." + Core.LevelManager.lastLevel.LevelName, showMessage);
				}
				else
				{
					Core.Randomizer.giveReward(num + "." + base.Owner.name, showMessage);
				}
			}
			else
			{
				Core.Logic.Penitent.Stats.Purge.Current += num;
			}
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
