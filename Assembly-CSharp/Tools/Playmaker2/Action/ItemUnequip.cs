using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Unequip the chosen item.")]
	public class ItemUnequip : InventoryBase
	{
		public override bool UseSlot
		{
			get
			{
				return true;
			}
		}

		public override bool UseObject
		{
			get
			{
				return false;
			}
		}

		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool result = false;
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.SetRelicInSlot(slot, null);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.SetPrayerInSlot(slot, null);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.SetRosaryBeadInSlot(slot, null);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.SetSwordInSlot(slot, null);
				break;
			}
			return result;
		}
	}
}
