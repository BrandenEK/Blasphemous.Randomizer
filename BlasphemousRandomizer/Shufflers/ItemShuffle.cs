using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Structures;
using Framework.Managers;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;

namespace BlasphemousRandomizer.Shufflers
{
    public class ItemShuffle
    {
        private Dictionary<string, Item> newItems;

        private Item lastItem;

        // Item has been collected from a location
        public void giveItem(string locationId, bool display)
        {
            // Make sure this location hasn't already been collected
            if (Core.Events.GetFlag("Location_" + locationId))
            {
                Main.Randomizer.Log("This location has already been collected!");
                return;
            }
            Core.Events.SetFlag("Location_" + locationId, true, false);

            // Get the item
            Item item = getItemAtLocation(locationId);
            if (item == null)
                return;

            // Logic for if its a progressive item

            // Add & maybe display the item
            addItem(item);
            if (display)
            {
                displayItem(item);
            }
        }

        // Gets the item held at the specified location
        public Item getItemAtLocation(string locationId)
        {
            if (newItems == null)
            {
                UIController.instance.ShowPopUp("Error: Rewards list not initialized!", "", 0f, false);
                return null;
            }
            if (!newItems.ContainsKey(locationId))
            {
                Main.Randomizer.Log("Location " + locationId + " was not loaded!");
                return null;
            }
            return newItems[locationId];
        }

        // Add the item to inventory
        private void addItem(Item item)
        {
            Main.Randomizer.Log($"Giving item [{item.type}]({item.id})");
            Main.Randomizer.itemsCollected++;

            InventoryManager inv = Core.InventoryManager;
            EntityStats stats = Core.Logic.Penitent.Stats;
            switch (item.type)
            {
                case 0:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("RB" + item.id.ToString("00"), InventoryManager.ItemType.Bead));
                    return;
                case 1:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("PR" + item.id.ToString("00"), InventoryManager.ItemType.Prayer));
                    return;
                case 2:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("RE" + item.id.ToString("00"), InventoryManager.ItemType.Relic));
                    return;
                case 3:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("HE" + item.id.ToString("00"), InventoryManager.ItemType.Sword));
                    return;
                case 4:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("CO" + item.id.ToString("00"), InventoryManager.ItemType.Collectible));
                    return;
                case 5:
                    inv.AddBaseObjectOrTears(inv.GetBaseObject("QI" + item.id.ToString("00"), InventoryManager.ItemType.Quest));
                    return;
                case 6:
                    Core.Events.SetFlag("RESCUED_CHERUB_" + item.id.ToString("00"), true, false);
                    return;
                case 7:
                    stats.Life.Upgrade(); stats.Life.SetToCurrentMax();
                    return;
                case 8:
                    stats.Fervour.Upgrade(); stats.Fervour.SetToCurrentMax();
                    return;
                case 9:
                    stats.MeaCulpa.Upgrade(); stats.Strength.Upgrade();
                    return;
                case 10:
                    stats.Purge.Current += item.id;
                    return;
                default:
                    return;
            }
        }

        private void displayItem(Item item)
        {

        }
    }
}
