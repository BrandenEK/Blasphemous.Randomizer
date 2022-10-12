using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Shows a message indicating that the chosen item has been removed from the player inventory. This action DOES NOT removes the item, it will only show a message")]
	public class ItemSubstractionMessage : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
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
				}
				break;
			}
			}
			UIController.instance.ShowObjectPopUp(UIController.PopupItemAction.GiveObject, itemName, image, objType, 3f, true);
			return true;
		}
	}
}
