using System.Collections.Generic;
using BlasphemousRandomizer.DoorRando;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class ItemFiller : Filler
    {
        private const string FIRST_DOOR = "D17Z01S01[E]";

        public bool Fill(int seed, Dictionary<string, string> mappedDoors, Dictionary<string, string> mappedItems)
        {
            // Initialize seed with empty lists
            initialize(seed);
            BlasphemousInventory inventory = new BlasphemousInventory();
            Dictionary<string, ItemLocation> allItemLocations = Main.Randomizer.data.itemLocations;
            Dictionary<string, DoorLocation> allDoorLocations = Main.Randomizer.data.doorLocations;
            Dictionary<string, Item> allItems = Main.Randomizer.data.items;

            foreach (ItemLocation location in allItemLocations.Values)
            {
                mappedItems.Add(location.Id, "Tears[500]");
            }

            // Seed is filled & validated
            return true;
        }
	}
}
