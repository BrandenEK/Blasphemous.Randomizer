using Framework.Managers;

namespace BlasphemousRandomizer.Structures
{
    public class ProgressiveItem : Item
    {
        public string[] items;
        public bool randomOrder;
        public bool removePrevious;

        // Used for wax beads, thorns, cherubs, rosary knots, bile flasks, quicksilver, collectibles

        public ProgressiveItem(string name, int type, bool progression, int count, string[] items, bool randomOrder, bool removePrevious) : base(name, type, progression, count)
        {
            this.items = items;
            this.randomOrder = randomOrder;
            this.removePrevious = removePrevious;
        }

        public override void addToInventory()
        {
            Item itemToAdd = getItemLevel(true);
            Core.Events.SetFlag("Item_" + itemToAdd.name, true, false);
            itemToAdd.addToInventory();
            if (removePrevious)
                removeItem();
        }

        public override RewardInfo getRewardInfo(bool upgraded)
        {
            return getItemLevel(upgraded).getRewardInfo(false);
        }

        public Item getItemLevel(bool upgraded)
        {
            int level = getCurrentLevel() + (upgraded ? 1 : 0);
            if (level >= 0 && level < items.Length)
            {
                return new Item(items[level], type, false, 0); // Change to search in dictionary.  maybe not tho
            }
            Main.Randomizer.Log("Invalid tier of progressive item!");
            return null;
        }

        private int getCurrentLevel()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (!Core.Events.GetFlag("Item_" + items[i]))
                {
                    Main.Randomizer.Log("Current progressive tier: " + (i - 1));
                    return i - 1;
                }
            }
            Main.Randomizer.Log("Current progressive tier: " + (items.Length - 1));
            return items.Length - 1;
        }

        private void removeItem()
        {
            int level = getCurrentLevel() - 1;
            if (level >= 0 && level < items.Length)
            {
                Main.Randomizer.Log("Removing item: " + items[level]);
                if (type == 5)
                    Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(items[level], InventoryManager.ItemType.Quest));
                else if (type == 0)
                    Core.InventoryManager.RemoveBaseObject(Core.InventoryManager.GetBaseObject(items[level], InventoryManager.ItemType.Bead));
            }
        }
    }
}
