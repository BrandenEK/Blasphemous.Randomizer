using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Managers;

namespace BlasphemousRandomizer.Structures
{
    [Serializable]
    public class NumerousItem : Item
    {
        public int count;
        public string[] itemIds;

        // Used for cherubs, quicksilver, rosary knots, and bile flasks, as these items are all the same

        public NumerousItem(string name, int type, int id, bool progression, int count, string[] itemIds) : base(name, type, id, progression)
        {
            this.count = count;
            this.itemIds = itemIds;
        }

        public override void addToInventory()
        {
            string nextItem = getNextItem();
            if (nextItem == "") return;

            Item newItem = new Item(nextItem, type, id, false);
            newItem.addToInventory();
        }

        public override RewardInfo getRewardInfo(bool upgraded)
        {
            return base.getRewardInfo(upgraded);
        }

        private string getNextItem()
        {
            for (int i = 0; i < itemIds.Length; i++)
            {
                string flag = "Item_" + itemIds[i];
                if (!Core.Events.GetFlag(flag))
                {
                    Main.Randomizer.Log("Giving item " + itemIds[i] + " from the set!");
                    Core.Events.SetFlag(flag, true, false);
                    return itemIds[i];
                }
            }
            Main.Randomizer.Log("You already own all of item " + name + "!");
            return "";
        }
    }
}
