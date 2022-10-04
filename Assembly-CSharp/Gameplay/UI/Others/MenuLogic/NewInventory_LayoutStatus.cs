using System;
using Framework.Inventory;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_LayoutStatus : NewInventory_Layout
	{
		public override void GetSelectedLoreData(out string caption, out string lore)
		{
			caption = string.Empty;
			lore = string.Empty;
		}

		public override int GetItemPosition(BaseInventoryObject item)
		{
			return 0;
		}
	}
}
