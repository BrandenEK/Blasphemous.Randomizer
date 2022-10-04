using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen item is equipped.")]
	public class ItemIsEquiped : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool result = false;
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.IsRelicEquipped(objectIdStting);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.IsPrayerEquipped(objectIdStting);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.IsRosaryBeadEquipped(objectIdStting);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.IsSwordEquipped(objectIdStting);
				break;
			}
			return result;
		}
	}
}
