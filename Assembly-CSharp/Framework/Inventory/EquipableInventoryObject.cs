using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public abstract class EquipableInventoryObject : BaseInventoryObject
	{
		public bool IsEquiped { get; private set; }

		public override bool IsEquipable()
		{
			return true;
		}

		public override bool AskForPercentageCompletition()
		{
			return true;
		}

		public override bool AddPercentageCompletition()
		{
			return this.UsePercentageCompletition;
		}

		public void Equip()
		{
			this.IsEquiped = true;
			base.SendMessage("OnEquipInventoryObject", 1);
		}

		public void UnEquip()
		{
			this.IsEquiped = false;
			base.SendMessage("OnUnEquipInventoryObject", 1);
		}

		public void Use()
		{
			Core.Metrics.CustomEvent("ITEM_USED", base.name, -1f);
			Core.Metrics.HeatmapEvent("ITEM_USED", Core.Logic.Penitent.transform.position);
			base.SendMessage("OnUseInventoryObject", 1);
		}

		public bool UsePercentageCompletition = true;
	}
}
