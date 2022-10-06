using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
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
			bool flag = this.ShowMessage != null && this.ShowMessage.Value;
			float num2 = Core.Logic.Penitent.Stats.Purge.Current + num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Core.Logic.Penitent.Stats.Purge.Current = num2;
			if (flag)
			{
				PopUpWidget.OnDialogClose += this.DialogClose;
				TearsObject tearsGenericObject = Core.InventoryManager.TearsGenericObject;
				UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, tearsGenericObject.caption, tearsGenericObject.picture, tearsGenericObject.GetItemType(), 3f, true);
			}
			else
			{
				base.Finish();
			}
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
