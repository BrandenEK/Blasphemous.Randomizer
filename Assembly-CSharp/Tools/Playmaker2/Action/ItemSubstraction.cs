using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Removes the chosen item form player's inventory.")]
	public class ItemSubstraction : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool flag = false;
			string itemName = string.Empty;
			Sprite image = null;
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
			{
				Relic relic = Core.InventoryManager.GetRelic(objectIdStting);
				if (relic)
				{
					itemName = relic.caption;
					image = relic.picture;
					flag = Core.InventoryManager.RemoveRelic(relic);
				}
				break;
			}
			case InventoryManager.ItemType.Prayer:
			{
				Prayer prayer = Core.InventoryManager.GetPrayer(objectIdStting);
				if (prayer)
				{
					itemName = prayer.caption;
					image = prayer.picture;
					flag = Core.InventoryManager.RemovePrayer(prayer);
				}
				break;
			}
			case InventoryManager.ItemType.Bead:
			{
				RosaryBead rosaryBead = Core.InventoryManager.GetRosaryBead(objectIdStting);
				if (rosaryBead)
				{
					itemName = rosaryBead.caption;
					image = rosaryBead.picture;
					flag = Core.InventoryManager.RemoveRosaryBead(rosaryBead);
				}
				break;
			}
			case InventoryManager.ItemType.Quest:
			{
				QuestItem questItem = Core.InventoryManager.GetQuestItem(objectIdStting);
				if (questItem)
				{
					itemName = questItem.caption;
					image = questItem.picture;
					flag = Core.InventoryManager.RemoveQuestItem(questItem);
				}
				break;
			}
			case InventoryManager.ItemType.Collectible:
			{
				Framework.Inventory.CollectibleItem collectibleItem = Core.InventoryManager.GetCollectibleItem(objectIdStting);
				if (collectibleItem)
				{
					itemName = collectibleItem.caption;
					image = collectibleItem.picture;
					flag = Core.InventoryManager.RemoveCollectibleItem(collectibleItem);
				}
				break;
			}
			case InventoryManager.ItemType.Sword:
			{
				Sword sword = Core.InventoryManager.GetSword(objectIdStting);
				if (sword)
				{
					itemName = sword.caption;
					image = sword.picture;
					flag = Core.InventoryManager.RemoveSword(sword);
				}
				break;
			}
			}
			bool flag2 = this.showMessage != null && this.showMessage.Value;
			if (flag && flag2)
			{
				UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GiveObject, itemName, image, objType, 3f, true);
			}
			return flag;
		}

		public FsmBool showMessage;
	}
}
