using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen item is available on player inventory.")]
	public class ItemIsOwned : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool result = false;
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.IsRelicOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.IsPrayerOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.IsRosaryBeadOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Quest:
				result = Core.InventoryManager.IsQuestItemOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Collectible:
				result = Core.InventoryManager.IsCollectibleItemOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.IsSwordOwned(objectIdStting);
				break;
			}
			return result;
		}
	}
}
