using System;
using Framework.Managers;

namespace Framework.Randomizer
{
	public class ProgressiveReward : Reward
	{
		public ProgressiveReward(int type, string[] items, Reward[] rewards, bool remove) : base(99, type)
		{
			this.items = items;
			this.rewards = rewards;
			this.removePrevious = remove;
		}

		public Reward getLevel(bool upgraded)
		{
			int num = this.currentLevel();
			if (upgraded)
			{
				num++;
			}
			if (num >= 0 && num < this.items.Length)
			{
				return this.rewards[num];
			}
			Core.Randomizer.Log("Trying to get invalid tier of progressive reward!", 0);
			return null;
		}

		public void removeItem()
		{
			if (!this.removePrevious)
			{
				return;
			}
			int num = this.currentLevel() - 1;
			if (num >= 0 && num < this.items.Length)
			{
				Core.Randomizer.Log("Removing item: " + this.items[num], 0);
				if (this.id == 0)
				{
					Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(this.items[num], InventoryManager.ItemType.Bead));
					return;
				}
				if (this.id == 5)
				{
					Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(this.items[num], InventoryManager.ItemType.Quest));
					return;
				}
			}
			Core.Randomizer.Log("Can't remove item because none are owned!", 0);
		}

		private int currentLevel()
		{
			for (int i = this.items.Length - 1; i >= 0; i--)
			{
				if ((this.id == 5 && Core.InventoryManager.IsQuestItemOwned(this.items[i])) || (this.id == 0 && Core.InventoryManager.IsRosaryBeadOwned(this.items[i])))
				{
					Core.Randomizer.Log("Current progressive tier is level " + i, 0);
					return i;
				}
			}
			Core.Randomizer.Log("Current progressive tier is level -1", 0);
			return -1;
		}

		public string[] items;

		public Reward[] rewards;

		public bool removePrevious;
	}
}
