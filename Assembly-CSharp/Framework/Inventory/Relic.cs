using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class Relic : EquipableInventoryObject
	{
		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Relic;
		}
	}
}
