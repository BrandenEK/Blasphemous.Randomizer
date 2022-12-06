using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class ItemFiller : Filler
    {
        private List<ItemLocation> allLocations;
        private List<Item> allItems;

		private ItemConfig config;

        public ItemFiller()
        {
			allLocations = new List<ItemLocation>();
			fillLocations(allLocations); // Change to load from json
            FileUtil.loadJson("items.json", out allItems);
			addSpecialItems(allItems);
        }

        public override bool isValid()
        {
            return allItems.Count > 0 && allLocations.Count > 0 && allItems.Count == allLocations.Count;
        }

        public bool Fill(int seed, ItemConfig config, Dictionary<string, Item> output)
        {
            initialize(seed);
			this.config = config;

            // Create lists
            List<ItemLocation> locations = new List<ItemLocation>(allLocations);
            List<ItemLocation> vanillaLocations = new List<ItemLocation>();
            List<Item> items = new List<Item>(allItems);
            List<Item> progressionItems = new List<Item>();

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

        // Assign vanilla items & fill vanillaLocations list
        private void fillVanillaItems(List<ItemLocation> locations, List<ItemLocation> vanillaLocations, List<Item> items)
        {
            List<Item> rewardsToRemove = new List<Item>();
			string[] randomLocations;
			if (config.type > 0)
				randomLocations = new string[] { "item", "cherub", "lady", "oil", "sword", "blessing", "guiltArena", "tirso", "miriam", "redento", "jocinero", "altasgracias", "tentudia", "gemino", "guiltBead", "ossuary", "boss", "visage", "mask", "herb", "church", "shop", "thorn", "candle", "viridiana", "cleofas" };
			else
				randomLocations = new string[0];

            for (int i = 0; i < locations.Count; i++)
            {
				if (!FileUtil.arrayContains(randomLocations, locations[i].type))
                {
					// If this location is vanilla, set its item to original game item
					Item vanillaItem = getItem(items, locations[i].originalItem);
					if (vanillaItem != null)
                    {
						locations[i].item = vanillaItem;
						if (vanillaItem.progression)
							vanillaLocations.Add(locations[i]);
						rewardsToRemove.Add(vanillaItem);
					}
                }
                else
                {
					// If starting gift location is random & start with wheel is enabled
					if (config.startWithWheel && locations[i].id == "QI106")
                    {
						Item wheel = getItem(items, "RB203");
						if (wheel != null)
                        {
							locations[i].item = wheel;
							vanillaLocations.Add(locations[i]);
							rewardsToRemove.Add(wheel);
                        }
                    }
                }
            }
            for (int i = rewardsToRemove.Count - 1; i >= 0; i--)
                items.Remove(rewardsToRemove[i]);
        }

		// Really slow, but works for now
		private Item getItem(List<Item> items, string itemName)
        {
			for (int i = 0; i < items.Count; i++)
            {
				if (items[i].name == itemName)
					return items[i];
            }
			return null;
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

            //Find blood & holy wounds & move them to the back
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
            if (data.swordLevel > 1)
                data.cherubBits |= 262144;
            if (data.fervourLevel < 2)
                data.cherubBits &= ~73728;

            return data;
        }

        // Place certain items at the back of the list so they are seeded first
        private void prioritizeRewards(List<Item> progressionItems)
        {
            int count = 0;
            for (int i = 0; i < progressionItems.Count - count; i++)
            {
                Item item = progressionItems[i];
                if (item.type == 2 && item.id == 1 || item.type == 5 && (item.id == 38 || item.id == 39 || item.id == 40))
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

		private void addSpecialItems(List<Item> items)
        {
			// Create progression items
			ProgressiveItem[] progressiveItems = new ProgressiveItem[]
			{
				new ProgressiveItem("RW", 0, 17, true, new string[] { "RB17", "RB18", "RB19" }, true),
				new ProgressiveItem("BW", 0, 24, true, new string[] { "RB24", "RB25", "RB26" }, true),
				new ProgressiveItem("TH", 5, 31, true, new string[] { "QI31", "QI32", "QI33", "QI34", "QI35", "QI79", "QI80", "QI81" }, true),
				new ProgressiveItem("RK", 5, 44, true, new string[] { "QI44", "QI52", "QI53", "QI54", "QI55", "QI56" }, false),
				new ProgressiveItem("BV", 5, 45, true, new string[] { "QI41", "QI45", "QI46", "QI47", "QI48", "QI49", "QI50", "QI51" }, false),
				new ProgressiveItem("QS", 5, 101, true, new string[] { "QI101", "QI102", "QI103", "QI104", "QI105" }, false),
				new ProgressiveItem("CH", 6, 0, true, new string[38], false),
				new ProgressiveItem("CO", 4, 0, true, new string[44], false)
			};
			for (int i = 1; i <= 38; i++)
				progressiveItems[6].items[i - 1] = "CH" + i.ToString("00");
			for (int i = 1; i <= 44; i++)
				progressiveItems[7].items[i - 1] = "CO" + i.ToString("00");
			// Shuffle list of collectibles

			// Add them to list
			for (int i = 0; i < progressiveItems.Length; i++)
            {
				for (int j = 0; j < progressiveItems[i].items.Length; j++)
                {
					items.Add(progressiveItems[i]);
                }
            }
        }

        // Temporary until they can be loaded from json
        private void fillLocations(List<ItemLocation> locations)
        {
			locations.Clear();

			//Rosaries
			locations.Add(new ItemLocation("RB01", "RB01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB02", "RB02", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB03", "RB03", "item", (InventoryData d) => d.shroud));
			locations.Add(new ItemLocation("RB04", "RB04", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB05", "RB05", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB06", "RB06", "altasgracias", (InventoryData d) => d.ceremonyItems >= 3 && d.hatchedEgg));
			locations.Add(new ItemLocation("RB07", "RB07", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("RB08", "RB08", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB09", "RB09", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB10", "RB10", "gemino1", (InventoryData d) => d.fullThimble));
			locations.Add(new ItemLocation("RB11", "RB11", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("RB12", "RB12", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("RB13", "RB13", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB14", "RB14", "item", (InventoryData d) => d.bridgeAccess && (d.wheel && d.swordLevel > 1 || d.dawnHeart || d.root)));
			locations.Add(new ItemLocation("RB15", "RB15", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB16", "RB16", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RB17", "RW", "candle", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB18", "RW", "candle", (InventoryData d) => d.redWax > 0));
			locations.Add(new ItemLocation("RB19", "RW", "candle", (InventoryData d) => d.bridgeAccess && d.redWax > 0 && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("RB20", "RB20", "redento", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB21", "RB21", "redento", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB22", "RB22", "redento", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB24", "BW", "candle", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB25", "BW", "candle", (InventoryData d) => d.blueWax > 0));
			locations.Add(new ItemLocation("RB26", "BW", "candle", (InventoryData d) => d.bridgeAccess && d.blueWax > 0));
			locations.Add(new ItemLocation("RB28", "RB28", "item", (InventoryData d) => d.root));
			locations.Add(new ItemLocation("RB30", "RB30", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB31", "RB31", "item", (InventoryData d) => d.bridgeAccess && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("RB32", "RB32", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB33", "RB33", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB34", "RB34", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("RB35", "RB35", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && (d.blood || d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce"))));
			locations.Add(new ItemLocation("RB36", "RB36", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("RB37", "RB37", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB38", "RB38", "guiltBead", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB101", "RB101", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB102", "RB102", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB103", "RB103", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB104", "RB104", "church", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB105", "RB105", "church", (InventoryData d) => d.tears >= 145000));
			locations.Add(new ItemLocation("RB106", "RB106", "item", (InventoryData d) => d.blood && d.root));
			locations.Add(new ItemLocation("RB107", "RB107", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB108", "RB108", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB201", "RB201", "item", (InventoryData d) => d.bridgeAccess && (d.root || d.dawnHeart && d.swordLevel > 1)));
			locations.Add(new ItemLocation("RB202", "RB202", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB203", "RB203", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB204", "RB204", "item", (InventoryData d) => d.blood && d.linen));
			locations.Add(new ItemLocation("RB301", "RB301", "crisanta", (InventoryData d) => d.bridgeAccess && d.woodKey));

			//Prayers
			locations.Add(new ItemLocation("PR01", "PR01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("PR03", "PR03", "tentudia", (InventoryData d) => d.tentudiaRemains >= 3));
			locations.Add(new ItemLocation("PR04", "PR04", "gemino", (InventoryData d) => (d.fullThimble || d.linen) && d.driedFlowers));
			locations.Add(new ItemLocation("PR05", "PR05", "jocinero", (InventoryData d) => d.bridgeAccess && d.cherubs >= 38));
			locations.Add(new ItemLocation("PR07", "PR07", "item", (InventoryData d) => d.bridgeAccess && d.blood));
			locations.Add(new ItemLocation("PR08", "PR08", "viridiana", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.canBeatBoss("crisanta")));
			locations.Add(new ItemLocation("PR09", "PR09", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("PR10", "PR10", "item", (InventoryData d) => d.root || d.linen));
			locations.Add(new ItemLocation("PR11", "PR11", "cleofas", (InventoryData d) => d.bridgeAccess && d.marksOfRefuge >= 3 && d.cord));
			locations.Add(new ItemLocation("PR12", "PR12", "item", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.linen && (d.root || d.swordLevel > 1)));
			locations.Add(new ItemLocation("PR14", "PR14", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("PR15", "PR15", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("PR16", "PR16", "item", (InventoryData d) => d.nail || d.linen));
			locations.Add(new ItemLocation("PR101", "PR101", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("PR201", "PR201", "miriam", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.linen && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("PR202", "PR202", "item", (InventoryData d) => d.blood || d.bridgeAccess));
			locations.Add(new ItemLocation("PR203", "PR203", "item", (InventoryData d) => d.blood));

			//Relics
			locations.Add(new ItemLocation("RE01", "RE01", "item", (InventoryData d) => d.elderKey));
			locations.Add(new ItemLocation("RE02", "RE02", "blessing", (InventoryData d) => d.hand));
			locations.Add(new ItemLocation("RE03", "RE03", "redento", (InventoryData d) => d.bridgeAccess && d.limestones >= 3 && d.knots >= 3));
			locations.Add(new ItemLocation("RE04", "RE04", "blessing", (InventoryData d) => d.cloth));
			locations.Add(new ItemLocation("RE05", "RE05", "jocinero", (InventoryData d) => d.bridgeAccess && d.cherubs >= 20));
			locations.Add(new ItemLocation("RE07", "RE07", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RE10", "RE10", "blessing", (InventoryData d) => d.hatchedEgg));

			//Hearts
			locations.Add(new ItemLocation("HE01", "HE01", "item", (InventoryData d) => d.bridgeAccess && d.blood));
			locations.Add(new ItemLocation("HE02", "HE02", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE03", "HE03", "item", (InventoryData d) => d.lung));
			locations.Add(new ItemLocation("HE04", "HE04", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("HE05", "HE05", "item", (InventoryData d) => d.root));
			locations.Add(new ItemLocation("HE06", "HE06", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE07", "HE07", "item", (InventoryData d) => d.bridgeAccess && d.redWax >= 3 && d.blueWax >= 3));
			locations.Add(new ItemLocation("HE10", "HE10", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("HE11", "HE11", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE101", "HE101", "item", (InventoryData d) => d.bridgeAccess && d.verses >= 4 && d.canBeatBoss("laudes")));
			locations.Add(new ItemLocation("HE201", "HE201", "crisanta", (InventoryData d) => d.bridgeAccess && d.woodKey && d.traitorEyes >= 2));

			//Collectibles
			locations.Add(new ItemLocation("CO01", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO02", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.goldKey && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("CO03", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO04", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO05", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO06", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO07", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO08", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO09", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO10", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("CO11", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO12", "CO", "item", (InventoryData d) => d.lungDamage));
			locations.Add(new ItemLocation("CO13", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO14", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO15", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO16", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO17", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO18", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO19", "CO", "item", (InventoryData d) => (d.fullThimble || d.linen) && d.blood));
			locations.Add(new ItemLocation("CO20", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO21", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO22", "CO", "item", (InventoryData d) => d.bridgeAccess && d.root));
			locations.Add(new ItemLocation("CO23", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO24", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.silverKey));
			locations.Add(new ItemLocation("CO25", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO26", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.goldKey));
			locations.Add(new ItemLocation("CO27", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("CO28", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO29", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO30", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO31", "CO", "item", (InventoryData d) => d.bridgeAccess && d.linen && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("CO32", "CO", "item", (InventoryData d) => d.nail && d.lung));
			locations.Add(new ItemLocation("CO33", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO34", "CO", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO35", "CO", "item", (InventoryData d) => d.bridgeAccess && (d.blood || d.dawnHeart || d.wheel && d.swordLevel > 1)));
			locations.Add(new ItemLocation("CO36", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO37", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("CO38", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO39", "CO", "item", (InventoryData d) => d.bridgeAccess && (d.wheel && d.swordLevel > 1 || d.dawnHeart || d.root)));
			locations.Add(new ItemLocation("CO40", "CO", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.lung && d.root && d.blood));
			locations.Add(new ItemLocation("CO41", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO42", "CO", "item", (InventoryData d) => d.blood || d.canBreakHoles));
			locations.Add(new ItemLocation("CO43", "CO", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO44", "CO", "item", (InventoryData d) => d.linen));

			//Quest items
			locations.Add(new ItemLocation("QI01", "QI01", "cleofas", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI02", "QI02", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI03", "QI03", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("QI04", "QI04", "item", (InventoryData d) => d.bridgeAccess && d.masks > 1));
			locations.Add(new ItemLocation("QI06", "QI06", "item", (InventoryData d) => d.blood || d.dawnHeart || (d.wheel && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI07", "QI07", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI08", "QI08", "item", (InventoryData d) => d.blood && d.root));
			locations.Add(new ItemLocation("QI10", "QI10", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI11", "QI11", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("QI12", "QI12", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI13", "QI13", "altasgracias", (InventoryData d) => d.ceremonyItems >= 3));
			locations.Add(new ItemLocation("QI14", "QI14", "hatching", (InventoryData d) => d.egg));
			locations.Add(new ItemLocation("QI19", "QI19", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI20", "QI20", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI31", "TH", "thorn", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI32", "TH", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI33", "TH", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI34", "TH", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI35", "TH", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI37", "QI37", "herb", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI38", "QI38", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI39", "QI39", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI40", "QI40", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI41", "BV", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI44", "RK", "item", (InventoryData d) => d.lungDamage));
			locations.Add(new ItemLocation("QI45", "BV", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI46", "BV", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI47", "BV", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI48", "BV", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI49", "BV", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("QI50", "BV", "item", (InventoryData d) => d.bridgeAccess && d.canBreakHoles));
			locations.Add(new ItemLocation("QI51", "BV", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.goldKey));
			locations.Add(new ItemLocation("QI52", "RK", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI53", "RK", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI54", "RK", "redento", (InventoryData d) => d.bridgeAccess && d.limestones >= 3 && d.knots >= 3));
			locations.Add(new ItemLocation("QI55", "RK", "item", (InventoryData d) => d.nail && d.blood));
			locations.Add(new ItemLocation("QI56", "RK", "tirso", (InventoryData d) => d.herbs >= 6 && d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI57", "QI57", "fountain", (InventoryData d) => d.emptyThimble));
			locations.Add(new ItemLocation("QI58", "QI58", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("QI59", "QI59", "gemino", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI60", "QI60", "mask", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("melquiades")));
			locations.Add(new ItemLocation("QI61", "QI61", "mask", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new ItemLocation("QI62", "QI62", "mask", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("QI63", "QI63", "herb", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI64", "QI64", "herb", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI65", "QI65", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI66", "QI66", "tirso", (InventoryData d) => d.herbs > 0));
			locations.Add(new ItemLocation("QI67", "QI67", "item", (InventoryData d) => d.nail));
			locations.Add(new ItemLocation("QI68", "QI68", "gemino", (InventoryData d) => d.fullThimble));
			locations.Add(new ItemLocation("QI69", "QI69", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("QI70", "QI70", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("QI71", "QI71", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("QI72", "QI72", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI75", "QI75", "chalice", (InventoryData d) => (d.lung && d.nail && (d.root || d.cherubAttack(786432))) || (d.linen && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI79", "TH", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess));
			locations.Add(new ItemLocation("QI80", "TH", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess && (d.blood || d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce"))));
			locations.Add(new ItemLocation("QI81", "TH", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI101", "QS", "item", (InventoryData d) => d.canBreakHoles));
			locations.Add(new ItemLocation("QI102", "QS", "item", (InventoryData d) => d.bridgeAccess && (d.blood && d.root || d.masks > 0 && d.bronzeKey)));
			locations.Add(new ItemLocation("QI103", "QS", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI104", "QS", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI105", "QS", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI106", "QI106", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI107", "QI107", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles));
			locations.Add(new ItemLocation("QI108", "QI108", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && (d.blood && d.root || d.bridgeAccess)));
			locations.Add(new ItemLocation("QI109", "QI109", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && (d.blood && d.root || d.masks > 0 && d.bronzeKey && d.silverKey)));
			locations.Add(new ItemLocation("QI110", "QI110", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("QI201", "QI201", "crisanta", (InventoryData d) => d.bones >= 30 && d.canBeatBoss("isidora")));
			locations.Add(new ItemLocation("QI202", "QI202", "crisanta", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("sierpes") && (d.root || d.nail || d.cherubAttack(786432) && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI203", "QI203", "crisanta", (InventoryData d) => d.blood || d.bridgeAccess));
			locations.Add(new ItemLocation("QI204", "QI204", "crisanta", (InventoryData d) => d.bridgeAccess && d.blood && d.scapular));
			locations.Add(new ItemLocation("QI301", "QI301", "crisanta", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.trueHeart && d.blood && d.canBeatBoss("crisanta")));

			//Cherubs
			locations.Add(new ItemLocation("RESCUED_CHERUB_01", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_02", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_03", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_04", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_05", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_06", "CH", "cherub", (InventoryData d) => d.cherubAttack(66) || (d.root && d.cherubAttack(197632)) || (d.blood && (d.linen || d.root))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_07", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_08", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_09", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_10", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_11", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_12", "CH", "cherub", (InventoryData d) => d.lung && d.nail));
			locations.Add(new ItemLocation("RESCUED_CHERUB_13", "CH", "cherub", (InventoryData d) => d.linen || d.nail || d.cherubAttack(467522)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_14", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_15", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_16", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_17", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_18", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_19", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_20", "CH", "cherub", (InventoryData d) => d.root || d.cherubAttack(205426)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_21", "CH", "cherub", (InventoryData d) => d.blood && (d.cherubAttack(8192) && d.swordLevel > 1 || d.cherubAttack(198210))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_22", "CH", "cherub", (InventoryData d) => d.linen || (d.lung && d.nail && d.cherubAttack(131138))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_23", "CH", "cherub", (InventoryData d) => d.root || d.cherubAttack(131138)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_24", "CH", "cherub", (InventoryData d) => d.linen || d.cherubAttack(197186)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_25", "CH", "cherub", (InventoryData d) => d.blood || d.cherubAttack(157266)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_26", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_27", "CH", "cherub", (InventoryData d) => (d.fullThimble || d.linen) && d.cherubAttack(418386)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_28", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_29", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_30", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_31", "CH", "cherub", (InventoryData d) => d.blood || d.cherubAttack(131138)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_32", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.blood && d.canBeatBoss("exposito") && ((d.dawnHeart && d.swordLevel > 1 && d.cherubAttack(131138)) || d.root)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_33", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.cherubAttack(197186)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_34", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_35", "CH", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_36", "CH", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("RESCUED_CHERUB_37", "CH", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_38", "CH", "cherub", (InventoryData d) => d.cherubAttack(942608)));

			//Lady of the Six Sorrows
			locations.Add(new ItemLocation("Lady[D01Z05S22]", "LU", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D02Z02S12]", "LU", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D02Z03S15]", "LU", "lady", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new ItemLocation("Lady[D01Z05S26]", "LU", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D06Z01S24]", "LU", "lady", (InventoryData d) => d.bridgeAccess && d.lung && d.root && d.blood));
			locations.Add(new ItemLocation("Lady[D05Z01S14]", "LU", "lady", (InventoryData d) => d.bridgeAccess));

			//Oil of Pilgrims
			locations.Add(new ItemLocation("Oil[D03Z03S13]", "FU", "oil", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("Oil[D01Z05S07]", "FU", "oil", (InventoryData d) => true));
			locations.Add(new ItemLocation("Oil[D02Z02S10]", "FU", "oil", (InventoryData d) => true));
			locations.Add(new ItemLocation("Oil[D04Z02S14]", "FU", "oil", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("Oil[D09Z01S12]", "FU", "oil", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("Oil[D05Z01S19]", "FU", "oil", (InventoryData d) => d.bridgeAccess));

			//Mea Culpa Shrines
			locations.Add(new ItemLocation("Sword[D01Z05S24]", "SU", "sword", (InventoryData d) => d.bridgeAccess && d.chalice && d.masks > 0 && d.bronzeKey && ((d.lung && d.nail && (d.root || d.cherubAttack(786432))) || (d.linen && d.swordLevel > 1))));
			locations.Add(new ItemLocation("Sword[D17Z01S08]", "SU", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D02Z03S13]", "SU", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D01Z02S06]", "SU", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D05Z01S13]", "SU", "sword", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("Sword[D04Z02S12]", "SU", "sword", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("Sword[D06Z01S11]", "SU", "sword", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.blood && d.lung));

			//Tears
			locations.Add(new ItemLocation("Tirso[500]", "Tears[500]", "tirso", (InventoryData d) => d.herbs >= 2));
			locations.Add(new ItemLocation("Tirso[1000]", "Tears[1000]", "tirso", (InventoryData d) => d.herbs >= 3));
			locations.Add(new ItemLocation("Tirso[2000]", "Tears[2000]", "tirso", (InventoryData d) => d.herbs >= 4));
			locations.Add(new ItemLocation("Tirso[5000]", "Tears[5000]", "tirso", (InventoryData d) => d.herbs >= 5));
			locations.Add(new ItemLocation("Tirso[10000]", "Tears[10000]", "tirso", (InventoryData d) => d.herbs >= 6));
			locations.Add(new ItemLocation("Lvdovico[500]", "Tears[500]", "tentudia", (InventoryData d) => d.tentudiaRemains >= 1));
			locations.Add(new ItemLocation("Lvdovico[1000]", "Tears[1000]", "tentudia", (InventoryData d) => d.tentudiaRemains >= 2));
			locations.Add(new ItemLocation("Arena_NailManager[1000]", "Tears[1000]", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("Arena_NailManager[3000]", "Tears[3000]", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("Arena_NailManager[5000]", "Tears[5000]", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess));
			locations.Add(new ItemLocation("Undertaker[250]", "Tears[250]", "ossuary", (InventoryData d) => d.bones >= 4));
			locations.Add(new ItemLocation("Undertaker[500]", "Tears[500]", "ossuary", (InventoryData d) => d.bones >= 8));
			locations.Add(new ItemLocation("Undertaker[750]", "Tears[750]", "ossuary", (InventoryData d) => d.bones >= 12));
			locations.Add(new ItemLocation("Undertaker[1000]", "Tears[1000]", "ossuary", (InventoryData d) => d.bones >= 16));
			locations.Add(new ItemLocation("Undertaker[1250]", "Tears[1250]", "ossuary", (InventoryData d) => d.bones >= 20));
			locations.Add(new ItemLocation("Undertaker[1500]", "Tears[1500]", "ossuary", (InventoryData d) => d.bones >= 24));
			locations.Add(new ItemLocation("Undertaker[1750]", "Tears[1750]", "ossuary", (InventoryData d) => d.bones >= 28));
			locations.Add(new ItemLocation("Undertaker[2000]", "Tears[2000]", "ossuary", (InventoryData d) => d.bones >= 32));
			locations.Add(new ItemLocation("Undertaker[2500]", "Tears[2500]", "ossuary", (InventoryData d) => d.bones >= 36));
			locations.Add(new ItemLocation("Undertaker[3000]", "Tears[3000]", "ossuary", (InventoryData d) => d.bones >= 40));
			locations.Add(new ItemLocation("Undertaker[5000]", "Tears[5000]", "ossuary", (InventoryData d) => d.bones >= 44));

			//Bosses
			locations.Add(new ItemLocation("BS01", "Tears[625]", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS03", "Tears[2600]", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS04", "Tears[2100]", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS05", "Tears[5500]", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("melquiades")));
			locations.Add(new ItemLocation("BS06", "Tears[9000]", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("BS12", "Tears[4300]", "crisanta", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("BS13", "Tears[300]", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS14", "Tears[11250]", "boss", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("BS16", "Tears[18000]", "boss", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.trueHeart && d.canBeatBoss("crisanta")));

			//Weird bosses
			locations.Add(new ItemLocation("D03Z01S03[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles));
			locations.Add(new ItemLocation("D02Z02S14[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.blood && d.root));
			locations.Add(new ItemLocation("D04Z01S04[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess));
			locations.Add(new ItemLocation("D09Z01S01[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("LaudesBossTrigger[30000]", "Tears[30000]", "boss", (InventoryData d) => d.bridgeAccess && d.verses >= 4 && d.canBeatBoss("laudes")));
			locations.Add(new ItemLocation("BossTrigger[5000]", "Tears[5000]", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("sierpes") && (d.root || d.nail || d.cherubAttack(786432) && d.swordLevel > 1)));
		}
	}
}
