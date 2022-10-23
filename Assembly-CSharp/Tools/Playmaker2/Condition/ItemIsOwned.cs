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
			Core.Randomizer.Log("Checking if item is owned: " + objectIdStting, 0);
			string levelName = Core.LevelManager.currentLevel.LevelName;
			if (levelName == "D01Z04S18" && objectIdStting == "QI38")
			{
				return true;
			}
			if (levelName == "D03Z03S15" && objectIdStting == "QI39")
			{
				return true;
			}
			if (levelName == "D02Z03S20" && objectIdStting == "QI40")
			{
				return true;
			}
			if ((levelName == "D04BZ03S01" && objectIdStting == "QI60") || (levelName == "D05Z01S15" && objectIdStting == "QI62") || (levelName == "D02Z03S19" && objectIdStting == "QI61"))
			{
				return Core.Events.GetFlag("LOCATION_" + objectIdStting);
			}
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
