using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.Notifications;
using Framework.Managers;
using Gameplay.UI;
using System.Collections.Generic;
using System.Text;

namespace Blasphemous.Randomizer.ItemRando
{
    public class ItemShuffle : IShuffle
    {
        private Dictionary<string, string> newItems;
        private Dictionary<string, string> newDoors;
        private ItemFiller filler;

        public bool ValidSeed => newItems != null && newItems.Count > 0;

        public DoorLocation LastDoor { get; set; }
        private Item LastItem { get; set; }

        // Manage mapped items
        public Dictionary<string, string> SaveMappedItems() => newItems;
        public void LoadMappedItems(Dictionary<string, string> mappedItems) => newItems = mappedItems;
        public void ClearMappedItems() => newItems = null;

        // Manage mapped doors
        public Dictionary<string, string> SaveMappedDoors() => newDoors;
        public void LoadMappedDoors(Dictionary<string, string> mappedDoors) => newDoors = mappedDoors;
        public void ClearMappedDoors() => newDoors = null;

        // Returns the target door given a door id - Also sets this as the last door entered
        public DoorLocation GetTargetDoor(string doorId)
        {
            if (newDoors == null || !newDoors.ContainsKey(doorId))
                return null;

            LastDoor = Main.Randomizer.data.doorLocations[newDoors[doorId]];
            return LastDoor;
        }

        // Gets the item held at the specified location
        public Item getItemAtLocation(string locationId)
        {
            if (newItems == null)
            {
                UIController.instance.ShowPopUp(Main.Randomizer.LocalizationHandler.Localize("itmerr"), "", 0f, false);
                return null;
            }
            if (!newItems.ContainsKey(locationId))
            {
                ModLog.Error("Location " + locationId + " was not loaded!");
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
                ModLog.Info($"Location {locationId} has already been collected!");
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

            // Queue item display info for when the item popup appears
            //_previousItemInfo = new RewardAchievement(item.GetName(true), item.GetNotification(true), item.GetImage(true));

            // Add the item to inventory
            ModLog.Info($"Giving item ({item.id})");
            Core.Events.SetFlag("LOCATION_" + locationId, true, false);
            LastItem = item;

            try
            {
                item.addToInventory();
            }
            catch (System.Exception e)
            {
                ModLog.Error($"Failed to add item to inventory ({item.id}): {e}");
            }

            // Possibly display the item
            if (display)
                ShowItemPopUp(item);
            Main.Randomizer.updateShops();
        }

        // Display the item in a pop up
        public void displayItem(string locationId)
        {
            // Get item
            string specialItems = "QI78RB17RB18RB19RB24RB25RB26";
            Item item = specialItems.Contains(locationId) ? LastItem : getItemAtLocation(locationId);
            if (item == null)
                return;

            // Call pop up method
            ShowItemPopUp(item);
        }

        /// <summary>
        /// Displays the last item picked up, used when the item popup should happen after the item is given
        /// </summary>
        //public void DisplayPreviousItem()
        //{
        //    if (_previousItemInfo == null)
        //        return;

        //    ModLog.Info("Displaying previous acquired item: " + _previousItemInfo.Id);
        //    UIController.instance.ShowPopupAchievement(_previousItemInfo);
        //}

        // Actually trigger the pop up
        public void ShowItemPopUp(Item item)
        {
            RewardAchievement achievement = new(item.GetName(false), item.GetNotification(false), item.GetImage(false));
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
                ModLog.Error($"Seed {seed + attempt} was invalid! Trying next...");
                attempt++;
            }
            if (attempt >= maxAttempts)
            {
                ModLog.Error($"Error: Failed to fill items in {maxAttempts} tries!");
                newItems.Clear();
                newDoors.Clear();
                return;
            }

            ModLog.Info(newItems.Count + " items have been shuffled!");
        }

        public void Init()
        {
            filler = new ItemFiller();
        }

        // The locations_items.json file must be in order sorted by area for this to work!
        public string GetSpoiler()
        {
            StringBuilder spoiler = new StringBuilder();
            spoiler.AppendLine($"Seed: {Main.Randomizer.GameSettings.Seed}");

            string currentArea = string.Empty;
            foreach (ItemLocation location in Main.Randomizer.data.itemLocations.Values)
            {
                Item item = getItemAtLocation(location.Id);
                if (item == null) continue;

                string locationArea = location.Room.Substring(0, 6);
                if (locationArea != currentArea && Main.Randomizer.data.LocationNames.TryGetValue(locationArea, out string locationName))
                {
                    // Reached new area
                    spoiler.AppendLine($"\n - {locationName} -\n");
                    currentArea = locationArea;
                }

                spoiler.AppendLine($"{location.Name}: {item.name}");
            }

            return spoiler.ToString();
        }
    }
}
