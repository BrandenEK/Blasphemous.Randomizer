using System;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows a message indicating that the chosen item has been added to the player inventory. This action DOES NOT adds the item, it will only show a message")]
	public class ItemAdditionMessage : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			Core.Randomizer.Log("ItemAdditionMessage (" + objectIdStting + ")", 2);
			Core.Randomizer.showNotification(objectIdStting);
			return true;
		}

		public override void OnEnter()
		{
			PopUpWidget.OnDialogClose += this.DialogClose;
			string text = (this.objectId == null) ? string.Empty : this.objectId.Value;
			int objType = (this.itemType == null) ? 0 : this.itemType.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Inventory Action - objectId is blank");
				return;
			}
			if (!this.executeAction(text, (InventoryManager.ItemType)objType, 0) && this.onFailure != null)
			{
				base.Fsm.Event(this.onFailure);
				base.Finish();
			}
		}

		public override void OnExit()
		{
			PopUpWidget.OnDialogClose -= this.DialogClose;
		}

		private void DialogClose()
		{
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}
	}
}
