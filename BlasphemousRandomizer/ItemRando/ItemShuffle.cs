﻿using BlasphemousRandomizer.Notifications;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI;

namespace BlasphemousRandomizer.ItemRando
{
    public class ItemShuffle : IShuffle
    {
        private Dictionary<string, string> newItems;
        private Dictionary<string, string> newDoors;
        private ItemFiller filler;

        private Item lastItem;

        public bool validSeed { get { return newItems != null && newItems.Count > 0; } }

        // Returns the target door given a door id
        public bool getNewDoor(string doorId, out string targetScene, out string targetId)
        {
            if (newDoors != null && newDoors.ContainsKey(doorId))
            {
                Main.Randomizer.Log("Processing door " + doorId);
                string targetDoorId = newDoors[doorId];
                int bracket = targetDoorId.IndexOf('[');

                targetScene = targetDoorId.Substring(0, bracket);
                targetId = targetDoorId.Substring(bracket + 1, targetDoorId.Length - bracket - 2);
                return true;
            }
            targetScene = "";
            targetId = "";
            return false;
        }

        // Gets the item held at the specified location
        public Item getItemAtLocation(string locationId)
        {
            if (newItems == null)
            {
                UIController.instance.ShowPopUp(Main.Randomizer.Localize("itmerr") + "!", "", 0f, false);
                return null;
            }
            if (!newItems.ContainsKey(locationId))
            {
                Main.Randomizer.LogError("Location " + locationId + " was not loaded!");
                return null;
            }
            return Main.Randomizer.data.items[newItems[locationId]];
        }

        // Item has been collected from a location
        public void giveItem(string locationId, bool display)
        {
            // Make sure this location hasn't already been collected
            if (Core.Events.GetFlag("LOCATION_" + locationId))
            {
                Main.Randomizer.Log($"Location {locationId} has already been collected!");
                return;
            }

            // If the location was a sword shrine, upgrade the mea culpa stat
            // This has to be done in here to prevent duplication in multiplayer
            if (locationId.StartsWith("Sword["))
                Core.Logic.Penitent.Stats.MeaCulpa.Upgrade();
            // If picking up one of the custom items in place of Holy Visage altar, also set altar flag
            else if (locationId == "QI38") Core.Events.SetFlag("ATTRITION_ALTAR_DONE", true, false);
            else if (locationId == "QI39") Core.Events.SetFlag("CONTRITION_ALTAR_DONE", true, false);
            else if (locationId == "QI40") Core.Events.SetFlag("COMPUNCTION_ALTAR_DONE", true, false);

            // Get the item
            Item item = getItemAtLocation(locationId);
            if (item == null)
                return;

            // Add the item to inventory
            Main.Randomizer.Log($"Giving item ({item.id})");
            Main.Randomizer.itemsCollected++;
            item.addToInventory();
            Core.Events.SetFlag("LOCATION_" + locationId, true, false);
            Main.Randomizer.updateShops();
            lastItem = item;

            // Possibly display the item
            if (display)
                showItemPopUp(item);
        }

        // Display the item in a pop up
        public void displayItem(string locationId)
        {
            // Get item
            string specialItems = "QI78RB17RB18RB19RB24RB25RB26";
            Item item = specialItems.Contains(locationId) ? lastItem : getItemAtLocation(locationId);
            if (item == null)
                return;

            // Call pop up method
            showItemPopUp(item);
        }

        // Actually trigger the pop up
        public void showItemPopUp(Item item)
        {
            RewardInfo info = item.getRewardInfo(false);
            RewardAchievement achievement = new RewardAchievement(info.name, info.notification, info.sprite);
            UIController.instance.ShowPopupAchievement(achievement);
        }

        // Shuffle the items - called when loading a game
        public void Shuffle(int seed)
        {
            if (!Main.Randomizer.data.isValid)
                return;

            newItems = new Dictionary<string, string>();
            newDoors = new Dictionary<string, string>();
            int attempt = 0, maxAttempts = 30;
            while (!filler.Fill(seed + attempt, newDoors, newItems) && attempt < maxAttempts)
            {
                Main.Randomizer.LogError($"Seed {seed + attempt} was invalid! Trying next...");
                attempt++;
            }
            if (attempt >= maxAttempts)
            {
                Main.Randomizer.LogError($"Error: Failed to fill items in {maxAttempts} tries!");
                return;
            }

            Main.Randomizer.totalItems = newItems.Count;
            Main.Randomizer.Log(newItems.Count + " items have been shuffled!");
        }

        public void Init()
        {
            filler = new ItemFiller();
        }

        public void Reset()
        {
            newItems = null;
        }

        public string GetSpoiler()
        {
            string spoiler = "================\nItems\n================\n\n";
            if (!Main.Randomizer.data.isValid)
                return spoiler + "Failed to generate item spoiler.\n\n";

            string template = Main.Randomizer.data.spoilerTemplate;

            for (int left = template.IndexOf("{"); left > 0; left = template.IndexOf("{"))
            {
                int right = template.IndexOf("}");
                string location = template.Substring(left + 1, right - left - 1);
                string item = "???";
                if (newItems.ContainsKey(location))
                {
                    item = Main.Randomizer.data.items[newItems[location]].name;
                }
                template = template.Substring(0, left) + item + template.Substring(right + 1);
            }

            return spoiler + template + "\n";
        }
    }
}