using Blasphemous.Randomizer.Doors;
using Blasphemous.Randomizer.Notifications;
using Blasphemous.Randomizer.Shuffle;
using Framework.Managers;
using Gameplay.UI;
using System.Collections.Generic;
using System.Text;

namespace Blasphemous.Randomizer.Items;

public class ItemHandler
{
    private Dictionary<string, string> _mappedItems;
    private Dictionary<string, string> _mappedDoors;
    private readonly ItemDoorShuffler _shuffler = new();

    private Item lastItem;

    public bool ValidSeed => _mappedItems != null && _mappedItems.Count > 0;

    public DoorData LastDoor { get; set; }

    // Manage mapped items
    public Dictionary<string, string> SaveMappedItems() => _mappedItems;
    public void LoadMappedItems(Dictionary<string, string> mappedItems) => _mappedItems = mappedItems;
    public void ClearMappedItems() => _mappedItems = null;

    // Manage mapped doors
    public Dictionary<string, string> SaveMappedDoors() => _mappedDoors;
    public void LoadMappedDoors(Dictionary<string, string> mappedDoors) => _mappedDoors = mappedDoors;
    public void ClearMappedDoors() => _mappedDoors = null;

    // Returns the target door given a door id - Also sets this as the last door entered
    public DoorData GetTargetDoor(string doorId)
    {
        if (_mappedDoors == null || !_mappedDoors.ContainsKey(doorId))
            return null;

        LastDoor = Main.Randomizer.DataHandler.Doors[_mappedDoors[doorId]];
        return LastDoor;
    }

    // Gets the item held at the specified location
    public Item GetItemAtLocation(string locationId)
    {
        if (_mappedItems == null)
        {
            UIController.instance.ShowPopUp(Main.Randomizer.LocalizationHandler.Localize("itmerr"), "", 0f, false);
            return null;
        }
        if (!_mappedItems.ContainsKey(locationId))
        {
            Main.Randomizer.LogError("Location " + locationId + " was not loaded!");
            return null;
        }
        return Main.Randomizer.DataHandler.Items[_mappedItems[locationId]];
    }

    // Item has been collected from a location
    public void GiveItem(string locationId, bool display)
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
        Item item = GetItemAtLocation(locationId);
        if (item == null)
            return;

        // Add the item to inventory
        Main.Randomizer.Log($"Giving item ({item.Id})");
        item.AddToInventory();
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
        Item item = specialItems.Contains(locationId) ? lastItem : GetItemAtLocation(locationId);
        if (item == null)
            return;

        // Call pop up method
        showItemPopUp(item);
    }

    // Actually trigger the pop up
    public void showItemPopUp(Item item)
    {
        RewardInfo info = item.GetRewardInfo(false);
        RewardAchievement achievement = new RewardAchievement(info.Name, info.Notification, info.Image);
        UIController.instance.ShowPopupAchievement(achievement);
    }

    // Shuffle the items - called when loading a game
    public void Shuffle(int seed)
    {
        _mappedItems = new Dictionary<string, string>();
        _mappedDoors = new Dictionary<string, string>();
        int attempt = 0, maxAttempts = 30;
        while (!_shuffler.Fill(seed + attempt, _mappedDoors, _mappedItems) && attempt < maxAttempts)
        {
            Main.Randomizer.LogError($"Seed {seed + attempt} was invalid! Trying next...");
            attempt++;
        }
        if (attempt >= maxAttempts)
        {
            Main.Randomizer.LogError($"Error: Failed to fill items in {maxAttempts} tries!");
            _mappedItems.Clear();
            _mappedDoors.Clear();
            return;
        }

        Main.Randomizer.Log(_mappedItems.Count + " items have been shuffled!");
    }

    // The locations_items.json file must be in order sorted by area for this to work!
    public string GetSpoiler()
    {
        StringBuilder spoiler = new StringBuilder();
        spoiler.AppendLine($"Seed: {Main.Randomizer.GameSeed}");

        string currentArea = string.Empty;
        foreach (ItemLocation location in Main.Randomizer.DataHandler.ItemLocations.Values)
        {
            Item item = GetItemAtLocation(location.Id);
            if (item == null) continue;

            string locationArea = location.Room.Substring(0, 6);
            if (locationArea != currentArea && Main.Randomizer.DataHandler.LocationNames.TryGetValue(locationArea, out string locationName))
            {
                // Reached new area
                spoiler.AppendLine($"\n - {locationName} -\n");
                currentArea = locationArea;
            }

            spoiler.AppendLine($"{location.Name}: {item.Name}");
        }

        return spoiler.ToString();
    }
}
