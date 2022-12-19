using System.Collections.Generic;
using BlasphemousRandomizer.Structures;
using BlasphemousRandomizer.Fillers;
using Framework.Managers;
using Gameplay.UI;

namespace BlasphemousRandomizer.Shufflers
{
    public class ItemShuffle : IShuffle
    {
        private Dictionary<string, Item> newItems;
        private ItemFiller filler;

        private Item lastItem;

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
                Main.Randomizer.Log($"Location {locationId} has already been collected!");
                return;
            }

            // Get the item
            Item item = getItemAtLocation(locationId);
            if (item == null)
                return;

            // Add the item to inventory
            Main.Randomizer.Log($"Giving item ({item.name})");
            Main.Randomizer.itemsCollected++;
            item.addToInventory();
            Core.Events.SetFlag("Location_" + locationId, true, false);
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

            //Temporary
            if (locationId == "QI110")
                Main.Randomizer.itemsCollected++;
        }

        // Actually trigger the pop up
        public void showItemPopUp(Item item)
        {
            RewardInfo info = item.getRewardInfo(false);
            RewardAchievement achievement = new RewardAchievement(info.name, info.notification, info.sprite);
            UIController.instance.ShowPopupAchievement(achievement);
        }

        // Used by hint filler to generate new hints
        public Dictionary<string, Item> getNewItems()
        {
            return newItems;
        }

        // Shuffle the items - called when loading a game
        public void Shuffle(int seed)
        {
            if (!filler.isValid())
            {
                Main.Randomizer.Log("Error: Item data could not be loaded!");
                return;
            }

            newItems = new Dictionary<string, Item>();
            int attempt = 0, maxAttempts = 30;
            while (!filler.Fill(seed + attempt, Main.Randomizer.gameConfig.items, newItems) && attempt < maxAttempts)
            {
                Main.Randomizer.Log($"Seed {seed + attempt} was invalid! Trying next...");
                attempt++;
            }
            if (attempt >= maxAttempts)
            {
                Main.Randomizer.Log($"Error: Failed to fill items in {maxAttempts} tries!");
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
            string template;
            Dictionary<string, string> itemNames = new Dictionary<string, string>();

            // Ensure data is valid
            if (!FileUtil.read("spoiler_items.dat", true, out template) || !FileUtil.parseFileToDictionary("names_items.dat", itemNames) || newItems == null)
            {
                return spoiler + "Failed to generate item spoiler.\n\n";
            }

            for (int left = template.IndexOf("{"); left > 0; left = template.IndexOf("{"))
            {
                int right = template.IndexOf("}");
                string location = template.Substring(left + 1, right - left - 1);
                string item = "???";
                if (newItems.ContainsKey(location))
                {
                    string desc = newItems[location].name;
                    if (itemNames.ContainsKey(desc))
                        item = itemNames[desc];
                }
                template = template.Substring(0, left) + item + template.Substring(right + 1);
            }

            return spoiler + template + "\n";
        }
    }
}
