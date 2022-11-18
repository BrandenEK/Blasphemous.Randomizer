using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Structures;
using BlasphemousRandomizer.Fillers;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;

namespace BlasphemousRandomizer.Shufflers
{
    public class ItemShuffle
    {
        private Dictionary<string, Item> newItems;
        private ItemFiller filler;

        private Item lastItem;

        public ItemShuffle()
        {
            filler = new ItemFiller();
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
            giveItem(item);
            lastItem = item;
            if (display)
            {
                displayItem(item);
            }
        }

        // Item has been collected, but display is delayed
        public void displayItem(string locationId)
        {
            string specialItems = "QI78RB17RB18RB19RB24RB25RB26";
            Item item = specialItems.Contains(locationId) ? lastItem : getItemAtLocation(locationId);
            if (item == null)
                return;

            displayItem(item);

            //Temporary
            if (locationId == "QI110")
                Main.Randomizer.itemsCollected++;
        }

        // Add the item to inventory
        private void giveItem(Item item)
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

        // Open a pop up to display the item
        private void displayItem(Item item)
        {
            RewardInfo info = item.getRewardInfo(false);
            RewardAchievement achievement = new RewardAchievement(info.name, info.notification, info.sprite);
            UIController.instance.ShowPopupAchievement(achievement);
        }

        // Shuffle the items - called when loading a game
        public void Shuffle(int seed)
        {
            while (!filler.Fill(seed, Main.Randomizer.gameConfig.items, newItems))
            {
                Main.Randomizer.Log($"Seed {seed} was invalid! Trying next...");
                seed++;
            }

            Main.Randomizer.totalItems = newItems.Count;
            Main.Randomizer.Log(newItems.Count + " items have been shuffled!");
        }
    }
}
