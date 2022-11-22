using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    class ItemFiller : Filler
    {
        private List<ItemLocation> allLocations;
        private List<Item> allItems;

		private ItemConfig config;

        public ItemFiller()
        {
			allLocations = new List<ItemLocation>();
			fillLocations(allLocations); // Change to load from json
            FileUtil.loadJson("items.json", out allItems);
			replaceProgressionItems(allItems);
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
			string[] randomLocations = config.type > 0 ? config.randomizedLocations : new string[0];
            for (int i = 0; i < locations.Count; i++)
            {
				if (!FileUtil.arrayContains(randomLocations, locations[i].type))
                {
                    locations[i].item = items[i];
                    if (items[i].progression)
                        vanillaLocations.Add(locations[i]);
                    rewardsToRemove.Add(items[i]);
                }
            }
            for (int i = 0; i < rewardsToRemove.Count; i++)
                items.Remove(rewardsToRemove[i]);
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

		// Add progression items to the items list
		private void replaceProgressionItems(List<Item> items)
        {
			ProgressiveItem[] progressiveItems = new ProgressiveItem[]
			{
				new ProgressiveItem("RW", 0, 17, true, new Item[3], true),
				new ProgressiveItem("BW", 0, 24, true, new Item[3], true),
				new ProgressiveItem("TH", 5, 31, true, new Item[8], true)
			};
			string[][] itemNames = new string[][]
			{
				new string[] { "RB17", "RB18", "RB19" },
				new string[] { "RB24", "RB25", "RB26" },
				new string[] { "QI31", "QI32", "QI33", "QI34", "QI35", "QI79", "QI80", "QI81" }
			};

			int lastIdx = 0;
			for (int i = 0; i < progressiveItems.Length; i++)
            {
				for (int j = 0; j < itemNames[i].Length; j++)
                {
					for (; lastIdx < items.Count; lastIdx++)
                    {
						// This item in the list is supposed to be a progressive item
						if (itemNames[i][j] == items[lastIdx].name)
                        {
							progressiveItems[i].items[j] = items[lastIdx];
							items[lastIdx] = progressiveItems[i];
							break;
                        }
                    }
                }
            }
        }

        // Temporary until they can be loaded from json
        private void fillLocations(List<ItemLocation> locations)
        {
			locations.Clear();

			//Rosaries
			locations.Add(new ItemLocation("RB01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB02", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB03", "item", (InventoryData d) => d.shroud));
			locations.Add(new ItemLocation("RB04", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB05", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB06", "altasgracias", (InventoryData d) => d.ceremonyItems >= 3 && d.hatchedEgg));
			locations.Add(new ItemLocation("RB07", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("RB08", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB09", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB10", "gemino1", (InventoryData d) => d.fullThimble));
			locations.Add(new ItemLocation("RB11", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("RB12", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("RB13", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB14", "item", (InventoryData d) => d.bridgeAccess && (d.wheel && d.swordLevel > 1 || d.dawnHeart || d.root)));
			locations.Add(new ItemLocation("RB15", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB16", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RB17", "candle", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB18", "candle", (InventoryData d) => d.redWax > 0));
			locations.Add(new ItemLocation("RB19", "candle", (InventoryData d) => d.bridgeAccess && d.redWax > 0 && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("RB20", "redento", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB21", "redento", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB22", "redento", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB24", "candle", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB25", "candle", (InventoryData d) => d.blueWax > 0));
			locations.Add(new ItemLocation("RB26", "candle", (InventoryData d) => d.bridgeAccess && d.blueWax > 0));
			locations.Add(new ItemLocation("RB28", "item", (InventoryData d) => d.root));
			locations.Add(new ItemLocation("RB30", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB31", "item", (InventoryData d) => d.bridgeAccess && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("RB32", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB33", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB34", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("RB35", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && (d.blood || d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce"))));
			locations.Add(new ItemLocation("RB36", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("RB37", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB38", "guiltBead", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB101", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB102", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB103", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB104", "church", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB105", "church", (InventoryData d) => d.tears >= 145000));
			locations.Add(new ItemLocation("RB106", "item", (InventoryData d) => d.blood && d.root));
			locations.Add(new ItemLocation("RB107", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB108", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB201", "item", (InventoryData d) => d.bridgeAccess && (d.root || d.dawnHeart && d.swordLevel > 1)));
			locations.Add(new ItemLocation("RB202", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB203", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB204", "item", (InventoryData d) => d.blood && d.linen));
			locations.Add(new ItemLocation("RB301", "crisanta", (InventoryData d) => d.bridgeAccess && d.woodKey));

			//Prayers
			locations.Add(new ItemLocation("PR01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("PR03", "tentudia", (InventoryData d) => d.tentudiaRemains >= 3));
			locations.Add(new ItemLocation("PR04", "gemino", (InventoryData d) => (d.fullThimble || d.linen) && d.driedFlowers));
			locations.Add(new ItemLocation("PR05", "jocinero", (InventoryData d) => d.bridgeAccess && d.cherubs >= 38));
			locations.Add(new ItemLocation("PR07", "item", (InventoryData d) => d.bridgeAccess && d.blood));
			locations.Add(new ItemLocation("PR08", "viridiana", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.canBeatBoss("crisanta")));
			locations.Add(new ItemLocation("PR09", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("PR10", "item", (InventoryData d) => d.root || d.linen));
			locations.Add(new ItemLocation("PR11", "cleofas", (InventoryData d) => d.bridgeAccess && d.marksOfRefuge >= 3 && d.cord));
			locations.Add(new ItemLocation("PR12", "item", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.linen && (d.root || d.swordLevel > 1)));
			locations.Add(new ItemLocation("PR14", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("PR15", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("PR16", "item", (InventoryData d) => d.nail || d.linen));
			locations.Add(new ItemLocation("PR101", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("PR201", "miriam", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.linen && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("PR202", "item", (InventoryData d) => d.blood || d.bridgeAccess));
			locations.Add(new ItemLocation("PR203", "item", (InventoryData d) => d.blood));

			//Relics
			locations.Add(new ItemLocation("RE01", "item", (InventoryData d) => d.elderKey));
			locations.Add(new ItemLocation("RE02", "blessing", (InventoryData d) => d.hand));
			locations.Add(new ItemLocation("RE03", "redento", (InventoryData d) => d.bridgeAccess && d.limestones >= 3 && d.knots >= 3));
			locations.Add(new ItemLocation("RE04", "blessing", (InventoryData d) => d.cloth));
			locations.Add(new ItemLocation("RE05", "jocinero", (InventoryData d) => d.bridgeAccess && d.cherubs >= 20));
			locations.Add(new ItemLocation("RE07", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RE10", "blessing", (InventoryData d) => d.hatchedEgg));

			//Hearts
			locations.Add(new ItemLocation("HE01", "item", (InventoryData d) => d.bridgeAccess && d.blood));
			locations.Add(new ItemLocation("HE02", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE03", "item", (InventoryData d) => d.lung));
			locations.Add(new ItemLocation("HE04", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("HE05", "item", (InventoryData d) => d.root));
			locations.Add(new ItemLocation("HE06", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE07", "item", (InventoryData d) => d.bridgeAccess && d.redWax >= 3 && d.blueWax >= 3));
			locations.Add(new ItemLocation("HE10", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("HE11", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("HE101", "item", (InventoryData d) => d.bridgeAccess && d.verses >= 4 && d.canBeatBoss("laudes")));
			locations.Add(new ItemLocation("HE201", "crisanta", (InventoryData d) => d.bridgeAccess && d.woodKey && d.traitorEyes >= 2));

			//Collectibles
			locations.Add(new ItemLocation("CO01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO02", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.goldKey && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("CO03", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO04", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO05", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO06", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO07", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO08", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO09", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO10", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("CO11", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO12", "item", (InventoryData d) => d.lungDamage));
			locations.Add(new ItemLocation("CO13", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO14", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO15", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO16", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO17", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO18", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO19", "item", (InventoryData d) => (d.fullThimble || d.linen) && d.blood));
			locations.Add(new ItemLocation("CO20", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO21", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO22", "item", (InventoryData d) => d.bridgeAccess && d.root));
			locations.Add(new ItemLocation("CO23", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO24", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.silverKey));
			locations.Add(new ItemLocation("CO25", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO26", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.goldKey));
			locations.Add(new ItemLocation("CO27", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("CO28", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO29", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO30", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO31", "item", (InventoryData d) => d.bridgeAccess && d.linen && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("CO32", "item", (InventoryData d) => d.nail && d.lung));
			locations.Add(new ItemLocation("CO33", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO34", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("CO35", "item", (InventoryData d) => d.bridgeAccess && (d.blood || d.dawnHeart || d.wheel && d.swordLevel > 1)));
			locations.Add(new ItemLocation("CO36", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO37", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("CO38", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO39", "item", (InventoryData d) => d.bridgeAccess && (d.wheel && d.swordLevel > 1 || d.dawnHeart || d.root)));
			locations.Add(new ItemLocation("CO40", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.lung && d.root && d.blood));
			locations.Add(new ItemLocation("CO41", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO42", "item", (InventoryData d) => d.blood || d.canBreakHoles));
			locations.Add(new ItemLocation("CO43", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("CO44", "item", (InventoryData d) => d.linen));

			//Quest items
			locations.Add(new ItemLocation("QI01", "cleofas", (InventoryData d) => d.bridgeAccess && d.marksOfRefuge >= 3));
			locations.Add(new ItemLocation("QI02", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI03", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("QI04", "item", (InventoryData d) => d.bridgeAccess && d.masks > 1));
			locations.Add(new ItemLocation("QI06", "item", (InventoryData d) => d.blood || d.dawnHeart || (d.wheel && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI07", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI08", "item", (InventoryData d) => d.blood && d.root));
			locations.Add(new ItemLocation("QI10", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI11", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("QI12", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI13", "altasgracias", (InventoryData d) => d.ceremonyItems >= 3));
			locations.Add(new ItemLocation("QI14", "hatching", (InventoryData d) => d.egg));
			locations.Add(new ItemLocation("QI19", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI20", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI31", "thorn", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI32", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI33", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI34", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI35", "thorn", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("QI37", "herb", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI38", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI39", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI40", "visage", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI41", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI44", "item", (InventoryData d) => d.lungDamage));
			locations.Add(new ItemLocation("QI45", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI46", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI47", "item", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI48", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI49", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("QI50", "item", (InventoryData d) => d.bridgeAccess && d.canBreakHoles));
			locations.Add(new ItemLocation("QI51", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.goldKey));
			locations.Add(new ItemLocation("QI52", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI53", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI54", "redento", (InventoryData d) => d.bridgeAccess && d.limestones >= 3 && d.knots >= 3));
			locations.Add(new ItemLocation("QI55", "item", (InventoryData d) => d.nail && d.blood));
			locations.Add(new ItemLocation("QI56", "tirso", (InventoryData d) => d.herbs >= 6 && d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI57", "fountain", (InventoryData d) => d.emptyThimble));
			locations.Add(new ItemLocation("QI58", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("QI59", "gemino", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI60", "mask", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("melquiades")));
			locations.Add(new ItemLocation("QI61", "mask", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new ItemLocation("QI62", "mask", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("QI63", "herb", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("QI64", "herb", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI65", "herb", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI66", "tirso", (InventoryData d) => d.herbs > 0));
			locations.Add(new ItemLocation("QI67", "item", (InventoryData d) => d.nail));
			locations.Add(new ItemLocation("QI68", "gemino", (InventoryData d) => d.fullThimble));
			locations.Add(new ItemLocation("QI69", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("QI70", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("QI71", "shop", (InventoryData d) => d.bridgeAccess && d.tears >= 65000));
			locations.Add(new ItemLocation("QI72", "item", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI75", "chalice", (InventoryData d) => (d.lung && d.nail && (d.root || d.cherubAttack(786432))) || (d.linen && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI79", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess));
			locations.Add(new ItemLocation("QI80", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess && (d.blood || d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce"))));
			locations.Add(new ItemLocation("QI81", "thorn", (InventoryData d) => d.guiltBead && d.bridgeAccess && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("QI101", "item", (InventoryData d) => d.canBreakHoles));
			locations.Add(new ItemLocation("QI102", "item", (InventoryData d) => d.bridgeAccess && (d.blood && d.root || d.masks > 0 && d.bronzeKey)));
			locations.Add(new ItemLocation("QI103", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI104", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI105", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("QI106", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("QI107", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles));
			locations.Add(new ItemLocation("QI108", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && (d.blood && d.root || d.bridgeAccess)));
			locations.Add(new ItemLocation("QI109", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && (d.blood && d.root || d.masks > 0 && d.bronzeKey && d.silverKey)));
			locations.Add(new ItemLocation("QI110", "verse", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("QI201", "crisanta", (InventoryData d) => d.bones >= 30 && d.canBeatBoss("isidora")));
			locations.Add(new ItemLocation("QI202", "crisanta", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("sierpes") && (d.root || d.nail || d.cherubAttack(786432) && d.swordLevel > 1)));
			locations.Add(new ItemLocation("QI203", "crisanta", (InventoryData d) => d.blood || d.bridgeAccess));
			locations.Add(new ItemLocation("QI204", "crisanta", (InventoryData d) => d.bridgeAccess && d.blood && d.scapular));
			locations.Add(new ItemLocation("QI301", "crisanta", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.trueHeart && d.blood && d.canBeatBoss("crisanta")));

			//Cherubs
			locations.Add(new ItemLocation("RESCUED_CHERUB_01", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_02", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_03", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_04", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_05", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_06", "cherub", (InventoryData d) => d.cherubAttack(66) || (d.root && d.cherubAttack(197632)) || (d.blood && (d.linen || d.root))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_07", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_08", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_09", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_10", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_11", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_12", "cherub", (InventoryData d) => d.lung && d.nail));
			locations.Add(new ItemLocation("RESCUED_CHERUB_13", "cherub", (InventoryData d) => d.linen || d.nail || d.cherubAttack(467522)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_14", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_15", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_16", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_17", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_18", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_19", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_20", "cherub", (InventoryData d) => d.root || d.cherubAttack(205426)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_21", "cherub", (InventoryData d) => d.blood && (d.cherubAttack(8192) && d.swordLevel > 1 || d.cherubAttack(198210))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_22", "cherub", (InventoryData d) => d.linen || (d.lung && d.nail && d.cherubAttack(131138))));
			locations.Add(new ItemLocation("RESCUED_CHERUB_23", "cherub", (InventoryData d) => d.root || d.cherubAttack(131138)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_24", "cherub", (InventoryData d) => d.linen || d.cherubAttack(197186)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_25", "cherub", (InventoryData d) => d.blood || d.cherubAttack(157266)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_26", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_27", "cherub", (InventoryData d) => (d.fullThimble || d.linen) && d.cherubAttack(418386)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_28", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_29", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_30", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_31", "cherub", (InventoryData d) => d.blood || d.cherubAttack(131138)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_32", "cherub", (InventoryData d) => d.bridgeAccess && d.blood && d.canBeatBoss("exposito") && ((d.dawnHeart && d.swordLevel > 1 && d.cherubAttack(131138)) || d.root)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_33", "cherub", (InventoryData d) => d.bridgeAccess && d.cherubAttack(197186)));
			locations.Add(new ItemLocation("RESCUED_CHERUB_34", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("RESCUED_CHERUB_35", "cherub", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RESCUED_CHERUB_36", "cherub", (InventoryData d) => d.bridgeAccess && d.masks > 0));
			locations.Add(new ItemLocation("RESCUED_CHERUB_37", "cherub", (InventoryData d) => true));
			locations.Add(new ItemLocation("RESCUED_CHERUB_38", "cherub", (InventoryData d) => d.cherubAttack(942608)));

			//Lady of the Six Sorrows
			locations.Add(new ItemLocation("Lady[D01Z05S22]", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D02Z02S12]", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D02Z03S15]", "lady", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new ItemLocation("Lady[D01Z05S26]", "lady", (InventoryData d) => true));
			locations.Add(new ItemLocation("Lady[D06Z01S24]", "lady", (InventoryData d) => d.bridgeAccess && d.lung && d.root && d.blood));
			locations.Add(new ItemLocation("Lady[D05Z01S14]", "lady", (InventoryData d) => d.bridgeAccess));

			//Oil of Pilgrims
			locations.Add(new ItemLocation("Oil[D03Z03S13]", "oil", (InventoryData d) => d.blood));
			locations.Add(new ItemLocation("Oil[D01Z05S07]", "oil", (InventoryData d) => true));
			locations.Add(new ItemLocation("Oil[D02Z02S10]", "oil", (InventoryData d) => true));
			locations.Add(new ItemLocation("Oil[D04Z02S14]", "oil", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("Oil[D09Z01S12]", "oil", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("Oil[D05Z01S19]", "oil", (InventoryData d) => d.bridgeAccess));

			//Mea Culpa Shrines
			locations.Add(new ItemLocation("Sword[D01Z05S24]", "sword", (InventoryData d) => d.bridgeAccess && d.chalice && d.masks > 0 && d.bronzeKey && ((d.lung && d.nail && (d.root || d.cherubAttack(786432))) || (d.linen && d.swordLevel > 1))));
			locations.Add(new ItemLocation("Sword[D17Z01S08]", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D02Z03S13]", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D01Z02S06]", "sword", (InventoryData d) => true));
			locations.Add(new ItemLocation("Sword[D05Z01S13]", "sword", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("Sword[D04Z02S12]", "sword", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("Sword[D06Z01S11]", "sword", (InventoryData d) => d.bridgeAccess && d.masks > 1 && d.blood && d.lung));

			//Tears
			locations.Add(new ItemLocation("Tirso[500]", "tirso", (InventoryData d) => d.herbs >= 2));
			locations.Add(new ItemLocation("Tirso[1000]", "tirso", (InventoryData d) => d.herbs >= 3));
			locations.Add(new ItemLocation("Tirso[2000]", "tirso", (InventoryData d) => d.herbs >= 4));
			locations.Add(new ItemLocation("Tirso[5000]", "tirso", (InventoryData d) => d.herbs >= 5));
			locations.Add(new ItemLocation("Tirso[10000]", "tirso", (InventoryData d) => d.herbs >= 6));
			locations.Add(new ItemLocation("Lvdovico[500]", "tentudia", (InventoryData d) => d.tentudiaRemains >= 1));
			locations.Add(new ItemLocation("Lvdovico[1000]", "tentudia", (InventoryData d) => d.tentudiaRemains >= 2));
			locations.Add(new ItemLocation("Arena_NailManager[1000]", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("Arena_NailManager[3000]", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("Arena_NailManager[5000]", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess));
			locations.Add(new ItemLocation("Undertaker[250]", "ossuary", (InventoryData d) => d.bones >= 4));
			locations.Add(new ItemLocation("Undertaker[500]", "ossuary", (InventoryData d) => d.bones >= 8));
			locations.Add(new ItemLocation("Undertaker[750]", "ossuary", (InventoryData d) => d.bones >= 12));
			locations.Add(new ItemLocation("Undertaker[1000]", "ossuary", (InventoryData d) => d.bones >= 16));
			locations.Add(new ItemLocation("Undertaker[1250]", "ossuary", (InventoryData d) => d.bones >= 20));
			locations.Add(new ItemLocation("Undertaker[1500]", "ossuary", (InventoryData d) => d.bones >= 24));
			locations.Add(new ItemLocation("Undertaker[1750]", "ossuary", (InventoryData d) => d.bones >= 28));
			locations.Add(new ItemLocation("Undertaker[2000]", "ossuary", (InventoryData d) => d.bones >= 32));
			locations.Add(new ItemLocation("Undertaker[2500]", "ossuary", (InventoryData d) => d.bones >= 36));
			locations.Add(new ItemLocation("Undertaker[3000]", "ossuary", (InventoryData d) => d.bones >= 40));
			locations.Add(new ItemLocation("Undertaker[5000]", "ossuary", (InventoryData d) => d.bones >= 44));

			//Bosses
			locations.Add(new ItemLocation("BS01", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS03", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS04", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS05", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("melquiades")));
			locations.Add(new ItemLocation("BS06", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("exposito")));
			locations.Add(new ItemLocation("BS12", "crisanta", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("BS13", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS14", "boss", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("BS16", "boss", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.trueHeart && d.canBeatBoss("crisanta")));

			//Weird bosses
			locations.Add(new ItemLocation("D03Z01S03[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles));
			locations.Add(new ItemLocation("D02Z02S14[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.blood && d.root));
			locations.Add(new ItemLocation("D04Z01S04[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess));
			locations.Add(new ItemLocation("D09Z01S01[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("LaudesBossTrigger[30000]", "boss", (InventoryData d) => d.bridgeAccess && d.verses >= 4 && d.canBeatBoss("laudes")));
			locations.Add(new ItemLocation("BossTrigger[5000]", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("sierpes") && (d.root || d.nail || d.cherubAttack(786432) && d.swordLevel > 1)));
		}
	}
}
