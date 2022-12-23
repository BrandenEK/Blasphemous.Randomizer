using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class ItemFiller : Filler
    {
		private ItemConfig config;
		private string[] lowestItems;

        public ItemFiller()
        {
			lowestItems = new string[] { "Tears[250]", "Tears[300]", "Tears[500]" };
        }

        public bool Fill(int seed, ItemConfig config, Dictionary<string, Item> output)
        {
            initialize(seed);
			this.config = config;

            // Create lists of locations
            List<ItemLocation> locations = new List<ItemLocation>(Main.Randomizer.data.itemLocations.Values);
            List<ItemLocation> vanillaLocations = new List<ItemLocation>();

            // Create lists of items
			List<Item> items = new List<Item>();
			foreach (Item item in Main.Randomizer.data.items.Values)
            {
                if (item is ProgressiveItem)
                    shuffleItemOrder(item as ProgressiveItem);

				if (item.count == 1)
					items.Add(item);
				else
					for (int i = 0; i < item.count; i++)
						items.Add(item);
            }
            List<Item> progressionItems = new List<Item>();

			// Remove low tears rewards and replace them with new items
			replaceNewItems(items);
			if (locations.Count != items.Count)
				return false;

			// Place vanilla items & get list of locations that are vanilla
			fillVanillaItems(locations, vanillaLocations, items);
            // Get list of items that are progression
            getProgressionItems(items, progressionItems);

			// Fill all progression items
			fillProgressionItems(locations, vanillaLocations, progressionItems);
			if (progressionItems.Count > 0)
				return false;

			// Fill all extra items
			fillExtraItems(locations, items);
			if (items.Count > 0)
				return false;

			// Fill output dictionary
			output.Clear();
			for (int i = 0; i < locations.Count; i++)
            {
				output.Add(locations[i].id, locations[i].item);
            }

			// Remove temporary location data
			for (int i = 0; i < locations.Count; i++)
            {
				locations[i].item = null;
            }

			// Seed is filled & validated
			return true;
        }

		// For each new item that is to be added, remove the lowest tear reward
		private void replaceNewItems(List<Item> items)
        {
			// For more items, need to add more to lowest array
			// Also won't work if some of these are set to vanilla
			int removed = 0;
			
			// Reliquaries
			if (config.shuffleReliquaries)
            {
				for (int i = 0; i < 3; i++)
                {
					items.RemoveAt(getItemIdx(items, lowestItems[removed]));
					removed++;
                }
            }
            else
            {
				items.RemoveAt(getItemIdx(items, "RB101"));
				items.RemoveAt(getItemIdx(items, "RB102"));
				items.RemoveAt(getItemIdx(items, "RB103"));
			}

			// Dash, Wall climb
        }

        // Assign vanilla items & fill vanillaLocations list
        private void fillVanillaItems(List<ItemLocation> locations, List<ItemLocation> vanillaLocations, List<Item> items)
        {
			string[] randomLocations;
			if (config.type > 0)
				randomLocations = new string[] { "item", "cherub", "lady", "oil", "sword", "blessing", "guiltArena", "tirso", "miriam", "redento", "jocinero", "altasgracias", "tentudia", "gemino", "guiltBead", "ossuary", "boss", "visage", "mask", "herb", "church", "shop", "thorn", "candle", "viridiana", "cleofas", "crisanta", "skill" };
			else
				randomLocations = new string[0];

            for (int i = 0; i < locations.Count; i++)
            {
				if (!FileUtil.arrayContains(randomLocations, locations[i].type))
                {
					// If this location is vanilla, set its item to original game item
					int vanillaIdx = getItemIdx(items, locations[i].originalItem);
					if (vanillaIdx >= 0)
                    {
						locations[i].item = items[vanillaIdx];
						if (items[vanillaIdx].progression)
							vanillaLocations.Add(locations[i]);
						items.RemoveAt(vanillaIdx);
					}
                }
                else
                {
					// If starting gift location is random & start with wheel is enabled
					if (config.startWithWheel && locations[i].id == "QI106")
                    {
						int wheelIdx = getItemIdx(items, "RB203");
						if (wheelIdx >= 0)
                        {
							locations[i].item = items[wheelIdx];
							vanillaLocations.Add(locations[i]);
							items.RemoveAt(wheelIdx);
                        }
                    }
                }
            }
        }

		// Really slow, but works for now
		private int getItemIdx(List<Item> items, string itemName)
        {
			for (int i = 0; i < items.Count; i++)
            {
				if (items[i].id == itemName)
					return i;
            }
			return -1;
        }
        
        // Fill progression items list while removing them from the items list
        private void getProgressionItems(List<Item> items, List<Item> progressionItems)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].progression)
                {
                    progressionItems.Add(items[i]);
                    items.RemoveAt(i);
                    i--;
                }
            }
        }

        // Place all progression items into a random reachable location
        private void fillProgressionItems(List<ItemLocation> locations, List<ItemLocation> vanillaLocations, List<Item> progressionItems)
        {
            List<ItemLocation> reachableLocations = new List<ItemLocation>();
            List<Item> itemsOwned = new List<Item>();
            shuffleList(progressionItems);

            // Find holy wounds & move them to the back
            prioritizeRewards(progressionItems);

            findReachable(locations, vanillaLocations, reachableLocations, itemsOwned);
            while (reachableLocations.Count > 0 && progressionItems.Count > 0)
            {
                int idx = rand(reachableLocations.Count);
                ItemLocation loc = reachableLocations[idx];
                Item item = progressionItems[progressionItems.Count - 1];
                loc.item = item;
                itemsOwned.Add(item);
                progressionItems.RemoveAt(progressionItems.Count - 1);
                findReachable(locations, vanillaLocations, reachableLocations, itemsOwned);
            }
        }

        // Place all remaining items into random locations
        private void fillExtraItems(List<ItemLocation> locations, List<Item> items)
        {
            shuffleList(locations);
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].item != null) continue;
                locations[i].item = items[items.Count - 1];
				items.RemoveAt(items.Count - 1);
            }
        }

        // Fill reachableLocations list with locations that are reachable with the current items owned
        private void findReachable(List<ItemLocation> locations, List<ItemLocation> vanillaLocations, List<ItemLocation> reachableLocations, List<Item> itemsOwned)
        {
            List<Item> newItems = new List<Item>();
            InventoryData data;

            while (true)
            {
                data = calculateItemData(itemsOwned);
                checkVanillaLocations(vanillaLocations, newItems, data);
                if (newItems.Count == 0)
                    break;
                itemsOwned.AddRange(newItems);
            }

            reachableLocations.Clear();
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].item == null && locations[i].isReachable(data))
                    reachableLocations.Add(locations[i]);
            }
        }

        // Fill out the item data based on the items owned
        private InventoryData calculateItemData(List<Item> itemsOwned)
        {
            InventoryData data = new InventoryData();

            for (int i = 0; i < itemsOwned.Count; i++)
            {
                data.addItem(itemsOwned[i]);
            }
            data.lungDamage = config.lungDamage || data.lung;
            if (data.fervourLevel < 2)
                data.cherubBits &= ~0x12000;

            return data;
        }

        // Place certain items at the back of the list so they are seeded first
        private void prioritizeRewards(List<Item> progressionItems)
        {
            int count = 0;
            for (int i = 0; i < progressionItems.Count - count; i++)
            {
                Item item = progressionItems[i];
                if (item.type == 5 && (item.id == "QI38" || item.id == "QI39" || item.id == "QI40"))
                {
                    progressionItems.RemoveAt(i);
                    progressionItems.Add(item);
                    count++;
                    i--;
                }
            }
        }

        // Check if any new vanilla locations are reachable and remove them
        private void checkVanillaLocations(List<ItemLocation> vanillaLocations, List<Item> newItems, InventoryData data)
        {
            newItems.Clear();
            for (int i = 0; i < vanillaLocations.Count; i++)
            {
                if (vanillaLocations[i].isReachable(data))
                {
                    newItems.Add(vanillaLocations[i].item);
                    vanillaLocations.RemoveAt(i);
                    i--;
                }
            }
        }

		// Randomizes the order of certain progressive items' rewards (Not used anymore)
		private void shuffleItemOrder(ProgressiveItem item)
        {
            if (item.randomOrder)
            {
                List<string> order = new List<string>(item.items);
                order.Sort(); // Sort
                shuffleList(order); // Randomize
                item.items = order.ToArray();
            }
        }
	}
}
