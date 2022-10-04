using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class CollectibleItem : BaseInventoryObject
	{
		public override bool AskForPercentageCompletition()
		{
			return true;
		}

		public override bool AddPercentageCompletition()
		{
			return this.UsePercentageCompletition;
		}

		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Collectible;
		}

		public bool ClaimedInOssuary
		{
			get
			{
				string id = string.Format("CLAIMED_{0}", this.id);
				return Core.Events.GetFlag(id);
			}
			set
			{
				string id = string.Format("CLAIMED_{0}", this.id);
				Core.Events.SetFlag(id, value, false);
			}
		}

		public override bool HasLore()
		{
			return false;
		}

		public bool UsePercentageCompletition = true;
	}
}
