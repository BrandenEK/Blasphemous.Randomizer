using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Inventory
{
	public class InventoryIdAttribute : PropertyAttribute
	{
		public InventoryIdAttribute(InventoryManager.ItemType type)
		{
			this.type = type;
		}

		public InventoryManager.ItemType type;
	}
}
