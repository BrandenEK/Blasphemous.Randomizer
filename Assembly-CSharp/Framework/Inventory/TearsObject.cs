using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class TearsObject : BaseInventoryObject
	{
		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Quest;
		}

		public float TearsForDuplicatedObject = 1200f;
	}
}
