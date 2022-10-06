using System;
using Framework.Managers;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Inventory
{
	public class InteractableInventoryAdd : MonoBehaviour
	{
		private void OnUsePost()
		{
			BaseInventoryObject baseInventoryObject = Core.InventoryManager.GetBaseObject(this.item, this.itemType);
			baseInventoryObject = Core.InventoryManager.AddBaseObjectOrTears(baseInventoryObject);
			if (baseInventoryObject)
			{
				Core.Persistence.SaveGame(true);
				if (this.showMessage)
				{
					UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, baseInventoryObject.caption, baseInventoryObject.picture, this.itemType, 3f, false);
				}
			}
		}

		public InventoryManager.ItemType itemType;

		public string item = string.Empty;

		public bool showMessage = true;
	}
}
