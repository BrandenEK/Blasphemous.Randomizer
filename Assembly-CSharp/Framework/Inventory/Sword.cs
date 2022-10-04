using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class Sword : EquipableInventoryObject
	{
		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Sword;
		}

		public static class Id
		{
			public const string SteamingIncenseHeart = "HE01";
		}
	}
}
