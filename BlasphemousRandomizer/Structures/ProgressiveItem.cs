using Framework.Managers;

namespace BlasphemousRandomizer.Structures
{
    public class ProgressiveItem : Item
    {
        public Item[] items;
        public bool removePrevious;

        public ProgressiveItem(string name, int type, int id, bool progression, Item[] items, bool removePrevious) : base(name, type, id, progression)
        {
            this.items = items;
            this.removePrevious = removePrevious;
        }

        /*public override string getDescriptor()
        {
            switch (id)
            {
                case 17: return "RW";
                case 24: return "BW";
                case 31: return "TH";
                default: return "X";
            }
        }*/

        public override void addToInventory()
        {
            getLevel(true).addToInventory();
            if (removePrevious)
                removeItem();
        }

        public override RewardInfo getRewardInfo(bool upgraded)
        {
            return getLevel(upgraded).getRewardInfo(false);
        }

        // Get the item at the current or next level
        public Item getLevel(bool upgraded)
        {
            int level = currentLevel() + (upgraded ? 1 : 0);
            if (level >= 0 && level < items.Length)
            {
                return items[level];
            }
            Main.Randomizer.Log("Invalid tier of progressive item!");
            return null;
        }

        // Gets the current tier of reward the player has
        private int currentLevel()
        {
            for (int i = items.Length - 1; i >= 0; i--)
            {
                if (type == 5 && Core.InventoryManager.IsQuestItemOwned(items[i].name) || type == 0 && Core.InventoryManager.IsRosaryBeadOwned(items[i].name))
                {
                    Main.Randomizer.Log("Current progressive tier: " + i);
                    return i;
                }
            }
            Main.Randomizer.Log("Current progressive tier: -1");
            return -1;
        }

        // Removes the previous item from the inventory
        public void removeItem()
        {
            int level = currentLevel() - 1;
            if (level >= 0 && level < items.Length)
            {
                Main.Randomizer.Log("Removing item: " + items[level].name);
                if (type == 5)
                    Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(items[level].name, InventoryManager.ItemType.Quest));
                else if (type == 0)
                    Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(items[level].name, InventoryManager.ItemType.Bead));
            }
        }
    }
}
