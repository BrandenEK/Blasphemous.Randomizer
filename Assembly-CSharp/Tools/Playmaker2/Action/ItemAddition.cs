using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Adds the chosen item to the player inventory.")]
	public class ItemAddition : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			BaseInventoryObject baseInventoryObject = Core.InventoryManager.GetBaseObject(objectIdStting, objType);
			if (baseInventoryObject)
			{
				baseInventoryObject = Core.InventoryManager.AddBaseObjectOrTears(baseInventoryObject);
				bool flag = this.showMessage != null && this.showMessage.Value;
				if (flag)
				{
					UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, baseInventoryObject.caption, baseInventoryObject.picture, objType, 3f, true);
				}
				return true;
			}
			Debug.LogError(string.Concat(new string[]
			{
				"Playmaker ItemAdition Error. object ",
				objectIdStting,
				" with type ",
				objType.ToString(),
				" not found"
			}));
			base.Fsm.Event(this.onSuccess);
			base.Finish();
			return false;
		}

		public override void OnEnter()
		{
			PopUpWidget.OnDialogClose += this.DialogClose;
			if (!this.showMessage.Value)
			{
				base.OnEnter();
				return;
			}
			string text = (this.objectId == null) ? string.Empty : this.objectId.Value;
			int objType = (this.itemType == null) ? 0 : this.itemType.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Inventory Action - objectId is blank");
			}
			else if (!this.executeAction(text, (InventoryManager.ItemType)objType, 0) && this.onFailure != null)
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

		public FsmBool showMessage;
	}
}
