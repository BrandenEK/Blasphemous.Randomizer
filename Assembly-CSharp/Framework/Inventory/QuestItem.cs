using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class QuestItem : BaseInventoryObject
	{
		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Quest;
		}
	}
}
