using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Equip the chosen item form player's inventory.")]
	public class ItemEquip : InventoryBase
	{
		public override bool UseSlot
		{
			get
			{
				return true;
			}
		}

		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool result = false;
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.SetRelicInSlot(slot, objectIdStting);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.SetPrayerInSlot(slot, objectIdStting);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.SetRosaryBeadInSlot(slot, objectIdStting);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.SetSwordInSlot(slot, objectIdStting);
				break;
			}
			return result;
		}
	}
}
