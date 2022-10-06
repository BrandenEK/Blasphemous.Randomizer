using System;
using Framework.Managers;
using Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Inventory
{
	public class InteractableInventoryAddQuestItem : MonoBehaviour
	{
		private void OnUsePost()
		{
			BaseInventoryObject baseInventoryObject = Core.InventoryManager.GetQuestItem(this.questItem);
			if (baseInventoryObject)
			{
				baseInventoryObject = Core.InventoryManager.AddBaseObjectOrTears(baseInventoryObject);
				if (baseInventoryObject)
				{
					Core.Persistence.SaveGame(true);
					if (this.showMessage)
					{
						UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GetObejct, baseInventoryObject.caption, baseInventoryObject.picture, InventoryManager.ItemType.Quest, 3f, false);
					}
				}
			}
		}

		[InventoryId(InventoryManager.ItemType.Quest)]
		public string questItem;

		[InfoBox("Este componente está deprecado!! Usa InteractableInvAdd.", 2, null)]
		public bool showMessage = true;

		public string sound = "event:/Key Event/Quest Item";
	}
}
