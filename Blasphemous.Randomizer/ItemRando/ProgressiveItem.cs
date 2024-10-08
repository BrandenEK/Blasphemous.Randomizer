﻿using Blasphemous.ModdingAPI;
using Framework.Managers;
using UnityEngine;

namespace Blasphemous.Randomizer.ItemRando
{
    [System.Serializable]
    public class ProgressiveItem : Item
    {
        public string[] items;
        public bool randomOrder; // Unused
        public bool removePrevious;

        // Used for wax beads, thorns, cherubs, rosary knots, bile flasks, quicksilver, sword skills

        public ProgressiveItem(string id, string name, string hint, int type, bool progression, int count, string[] items, bool randomOrder, bool removePrevious) : base(id, name, hint, type, progression, count)
        {
            this.items = items;
            this.randomOrder = randomOrder;
            this.removePrevious = removePrevious;
        }

        public override void addToInventory()
        {
            Item itemToAdd = getItemLevel(true);
            itemToAdd.addToInventory();
            if (removePrevious)
                removeItem();
        }

        public override string GetName(bool upgraded)
        {
            return getItemLevel(upgraded).GetName(false);
        }

        public override string GetDescription(bool upgraded)
        {
            return getItemLevel(upgraded).GetDescription(false);
        }

        public override string GetNotification(bool upgraded)
        {
            return getItemLevel(upgraded).GetNotification(false);
        }

        public override Sprite GetImage(bool upgraded)
        {
            return getItemLevel(upgraded).GetImage(false);
        }

        public Item getItemLevel(bool upgraded)
        {
            int level = getCurrentLevel() + (upgraded ? 1 : 0);
            if (level < 0 || level >= items.Length)
            {
                ModLog.Error("Invalid tier of progressive item!");
                if (level < 0) level = 0;
                else if (level >= items.Length) level = items.Length - 1;
            }

            return new Item(items[level], "", "", type, false, 0); // Change to search in dictionary.  (Would have to make cherubs a regular item)
        }

        private int getCurrentLevel()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (!Core.Events.GetFlag("ITEM_" + items[i]))
                {
                    ModLog.Info("Current progressive tier: " + (i - 1));
                    return i - 1;
                }
            }
            ModLog.Info("Current progressive tier: " + (items.Length - 1));
            return items.Length - 1;
        }

        private void removeItem()
        {
            int level = getCurrentLevel() - 1;
            if (level >= 0 && level < items.Length)
            {
                ModLog.Info("Removing item: " + items[level]);
                if (type == 5)
                    Core.InventoryManager.RemoveQuestItem(items[level]);
                else if (type == 0)
                    Core.InventoryManager.RemoveRosaryBead(items[level]);
                else
                    ModLog.Display($"Item type {type} can not be removed!");
            }
        }
    }
}
