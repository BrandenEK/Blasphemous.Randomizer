using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add or remove purge points.")]
	public class TearsAddition : FsmStateAction
	{
		public override void OnEnter()
		{
			float num = (this.Tears == null) ? 0f : this.Tears.Value;
			bool flag = this.ShowMessage != null && this.ShowMessage.Value;
			float num2 = Core.Logic.Penitent.Stats.Purge.Current + num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Core.Logic.Penitent.Stats.Purge.Current = num2;
			Core.Randomizer.Log("TearsAddition", 2);
			if (flag)
			{
				PopUpWidget.OnDialogClose += this.DialogClose;
				TearsObject tearsGenericObject = Core.InventoryManager.TearsGenericObject;
				UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, tearsGenericObject.caption, tearsGenericObject.picture, tearsGenericObject.GetItemType(), 3f, true);
				return;
			}
			if (this.onSuccess != null)
			{
				base.Fsm.Event(this.onSuccess);
			}
			base.Finish();
		}

		private void DialogClose()
		{
			PopUpWidget.OnDialogClose -= this.DialogClose;
			if (this.onSuccess != null)
			{
				base.Fsm.Event(this.onSuccess);
			}
			base.Finish();
		}

		public FsmFloat Tears;

		public FsmBool ShowMessage;

		public FsmEvent onSuccess;
	}
}
