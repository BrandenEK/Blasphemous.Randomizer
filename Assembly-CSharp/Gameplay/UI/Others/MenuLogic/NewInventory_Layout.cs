using System;
using Framework.Inventory;
using Sirenix.OdinInspector;

namespace Gameplay.UI.Others.MenuLogic
{
	public abstract class NewInventory_Layout : SerializedMonoBehaviour
	{
		public virtual void ShowLayout(NewInventoryWidget.TabType type, bool editMode)
		{
		}

		public virtual void RestoreFromLore()
		{
		}

		public virtual void RestoreSlotPosition(int slotPosition)
		{
		}

		public virtual int GetLastSlotSelected()
		{
			return 0;
		}

		public abstract void GetSelectedLoreData(out string caption, out string lore);

		public virtual bool CanGoBack()
		{
			return true;
		}

		public virtual bool CanLore()
		{
			return true;
		}

		public abstract int GetItemPosition(BaseInventoryObject item);
	}
}
