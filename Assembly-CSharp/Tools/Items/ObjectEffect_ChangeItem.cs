using System;
using Framework.Inventory;
using Framework.Managers;
using Sirenix.OdinInspector;

namespace Tools.Items
{
	public class ObjectEffect_ChangeItem : ObjectEffect
	{
		private void OnResetInventoryObject()
		{
		}

		protected override bool OnApplyEffect()
		{
			bool flag = true;
			if (this.addObject)
			{
				flag = Core.InventoryManager.AddBaseObject(this.NewItem.GetInvObject());
			}
			int baseObjectEquippedSlot = Core.InventoryManager.GetBaseObjectEquippedSlot(this.InvObj);
			flag = (flag && Core.InventoryManager.RemoveBaseObject(this.InvObj));
			if (this.equip && baseObjectEquippedSlot != -1)
			{
				Core.InventoryManager.EquipBaseObject(this.NewItem.GetInvObject(), baseObjectEquippedSlot);
			}
			return flag;
		}

		public bool addObject;

		[ShowIf("addObject", true)]
		public InventoryObjectInspector NewItem;

		[ShowIf("addObject", true)]
		public bool equip;
	}
}
