using UnityEngine;
using System.Collections.Generic;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer
{
    public class DataStorage
    {
        public bool isValid { get; private set; }

        // Json data
        public Dictionary<string, Item> items;
        public Dictionary<string, ItemLocation> itemLocations;
		public Dictionary<string, Enemy> enemies;
        public Dictionary<string, EnemyLocation> enemyLocations;
        public Dictionary<string, DoorLocation> doorLocations;

        // Text data
        public Dictionary<string, string> itemNames;
        public Dictionary<string, string> enemyNames;
        public Dictionary<string, string> itemHints;
        public Dictionary<string, string> locationHints;
        public string[] interactableIds;
        public string[] cutsceneNames;
        public string[] cutsceneFlags;
        public string spoilerTemplate;

        // Image data
        public Sprite[] randomizerImages;
        public Sprite[] uiImages;

        public bool loadData()
        {
            bool valid = true;

            // Items
            items = new Dictionary<string, Item>();
            if (FileUtil.loadJson("items.json", out List<Item> tempItems))
            {
                addSpecialItems(tempItems);
                for (int i = 0; i < tempItems.Count; i++)
                    items.Add(tempItems[i].name, tempItems[i]);
                Main.Randomizer.Log($"Loaded {items.Count} items!");
            }
            else { Main.Randomizer.Log("Error: Failed to load items!"); valid = false; }

			// Item locations
			itemLocations = new Dictionary<string, ItemLocation>();
			List<ItemLocation> tempItemLocations = fillLocations();
			for (int i = 0; i < tempItemLocations.Count; i++)
				itemLocations.Add(tempItemLocations[i].id, tempItemLocations[i]);

			// Enemies
			enemies = new Dictionary<string, Enemy>();
			if (FileUtil.loadJson("enemies.json", out List<Enemy> tempEnemies))
            {
				for (int i = 0; i < tempEnemies.Count; i++)
					enemies.Add(tempEnemies[i].id, tempEnemies[i]);
				Main.Randomizer.Log($"Loaded {enemies.Count} enemies!");
            }
			else { Main.Randomizer.Log("Error: Failed to load enemies!"); valid = false; }

			// Enemy locations
			enemyLocations = new Dictionary<string, EnemyLocation>();
			if (FileUtil.loadJson("locations_enemies.json", out List<EnemyLocation> tempEnemyLocations))
			{
				for (int i = 0; i < tempEnemyLocations.Count; i++)
					enemyLocations.Add(tempEnemyLocations[i].locationId, tempEnemyLocations[i]);
				Main.Randomizer.Log($"Loaded {enemyLocations.Count} enemy locations!");
			}
			else { Main.Randomizer.Log("Error: Failed to load enemy locations!"); valid = false; }

			// Doors
			doorLocations = new Dictionary<string, DoorLocation>();
            if (FileUtil.loadJson("doors.json", out List<DoorLocation> tempDoorLocations))
            {
                for (int i = 0; i < tempDoorLocations.Count; i++)
                    doorLocations.Add(tempDoorLocations[i].id, tempDoorLocations[i]);
                Main.Randomizer.Log($"Loaded {doorLocations.Count} doors!");
            }
            else { Main.Randomizer.Log("Error: Failed to load doors!"); valid = false; }

            // Load text data
            if (FileUtil.parseFileToDictionary("names_items.dat", out itemNames)) Main.Randomizer.Log($"Loaded {itemNames.Count} item names!");
            else { Main.Randomizer.Log("Error: Failed to load item names!"); valid = false; }
            if (FileUtil.parseFileToDictionary("names_enemies.dat", out enemyNames)) Main.Randomizer.Log($"Loaded {enemyNames.Count} enemy names!");
            else { Main.Randomizer.Log("Error: Failed to load enemy names!"); valid = false; }
            if (FileUtil.parseFileToDictionary("hints_items.dat", out itemHints)) Main.Randomizer.Log($"Loaded {itemHints.Count} item hints!");
            else { Main.Randomizer.Log("Error: Failed to load item hints!"); valid = false; }
            if (FileUtil.parseFileToDictionary("hints_locations.dat", out locationHints)) Main.Randomizer.Log($"Loaded {locationHints.Count} location hints!");
            else { Main.Randomizer.Log("Error: Failed to load locations hints!"); valid = false; }
            if (FileUtil.parseFiletoArray("interactable_ids.dat", out interactableIds)) Main.Randomizer.Log($"Loaded {interactableIds.Length} interactable ids!");
            else { Main.Randomizer.Log("Error: Failed to load interactable ids!"); valid = false; }
            if (FileUtil.parseFiletoArray("cutscenes_names.dat", out cutsceneNames)) Main.Randomizer.Log($"Loaded {cutsceneNames.Length} cutscene names!");
            else { Main.Randomizer.Log("Error: Failed to load cutscene names!"); valid = false; }
            if (FileUtil.parseFiletoArray("cutscenes_flags.dat", out cutsceneFlags)) Main.Randomizer.Log($"Loaded {cutsceneFlags.Length} cutscene flags!");
            else { Main.Randomizer.Log("Error: Failed to load cutscene flags!"); valid = false; }
            if (FileUtil.read("spoiler_items.dat", true, out spoilerTemplate)) Main.Randomizer.Log("Loaded spoiler template!");
            else { Main.Randomizer.Log("Error: Failed to load spoiler template!"); valid = false; }

            // Load image data
            if (FileUtil.loadImages("custom_images.png", 32, 32, 0, out randomizerImages)) Main.Randomizer.Log($"Loaded {randomizerImages.Length} randomizer images!");
            else { Main.Randomizer.Log("Error: Failed to load randomizer images!"); valid = false; }
            if (FileUtil.loadImages("ui.png", 36, 36, 0, out uiImages)) Main.Randomizer.Log($"Loaded {uiImages.Length} ui images!");
            else { Main.Randomizer.Log("Error: Failed to load ui images!"); valid = false; }

            isValid = valid;
            return valid;
        }

        // temp - should be moved to inside the json file
        private void addSpecialItems(List<Item> items)
        {
            // Create progression items
            ProgressiveItem[] progressiveItems = new ProgressiveItem[]
            {
                new ProgressiveItem("RW", 0, true, 3, new string[] { "RB17", "RB18", "RB19" }, false, true),
                new ProgressiveItem("BW", 0, true, 3, new string[] { "RB24", "RB25", "RB26" }, false, true),
                new ProgressiveItem("TH", 5, true, 8, new string[] { "QI31", "QI32", "QI33", "QI34", "QI35", "QI79", "QI80", "QI81" }, false, true),
                new ProgressiveItem("RK", 5, true, 6, new string[] { "QI44", "QI52", "QI53", "QI54", "QI55", "QI56" }, false, false),
                new ProgressiveItem("BV", 5, true, 8, new string[] { "QI41", "QI45", "QI46", "QI47", "QI48", "QI49", "QI50", "QI51" }, false, false),
                new ProgressiveItem("QS", 5, true, 5, new string[] { "QI101", "QI102", "QI103", "QI104", "QI105" }, false, false),
                new ProgressiveItem("CH", 6, true, 38, new string[38], false, false),
                new ProgressiveItem("CO", 4, true, 44, new string[44], true, false)
            };
            for (int i = 1; i <= 38; i++)
                progressiveItems[6].items[i - 1] = "CH" + i.ToString("00");
            for (int i = 1; i <= 44; i++)
                progressiveItems[7].items[i - 1] = "CO" + i.ToString("00");

			for (int i = 0; i < progressiveItems.Length; i++)
            {
				items.Add(progressiveItems[i]);
            }
        }

		// Temporary until they can be loaded from json
		private List<ItemLocation> fillLocations()
		{
			List<ItemLocation> locations = new List<ItemLocation>();

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
			locations.Add(new ItemLocation("RB10", "RB10", "gemino", (InventoryData d) => d.fullThimble));
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
			locations.Add(new ItemLocation("RB28", "RB28", "item", (InventoryData d) => d.root || d.dawnHeart));
			locations.Add(new ItemLocation("RB30", "RB30", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB31", "RB31", "item", (InventoryData d) => d.bridgeAccess && d.blood && d.root && d.lung));
			locations.Add(new ItemLocation("RB32", "RB32", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB33", "RB33", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB34", "RB34", "guiltArena", (InventoryData d) => d.guiltBead));
			locations.Add(new ItemLocation("RB35", "RB35", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && (d.blood || d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce"))));
			locations.Add(new ItemLocation("RB36", "RB36", "guiltArena", (InventoryData d) => d.guiltBead && d.bridgeAccess && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("RB37", "RB37", "shop", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB38", "RB38", "guiltBead", (InventoryData d) => true));
			//locations.Add(new ItemLocation("RB101", "RB101", "penitence", (InventoryData d) => true));
			//locations.Add(new ItemLocation("RB102", "RB102", "penitence", (InventoryData d) => true));
			//locations.Add(new ItemLocation("RB103", "RB103", "penitence", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB104", "RB104", "church", (InventoryData d) => d.tears >= 15000));
			locations.Add(new ItemLocation("RB105", "RB105", "church", (InventoryData d) => d.tears >= 145000));
			locations.Add(new ItemLocation("RB106", "RB106", "item", (InventoryData d) => d.blood && d.root));
			locations.Add(new ItemLocation("RB107", "RB107", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB108", "RB108", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB201", "RB201", "item", (InventoryData d) => d.bridgeAccess && (d.root || d.dawnHeart && d.swordLevel > 1)));
			locations.Add(new ItemLocation("RB202", "RB202", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("RB203", "RB203", "item", (InventoryData d) => d.bridgeAccess));
			locations.Add(new ItemLocation("RB204", "RB204", "item", (InventoryData d) => d.blood && d.linen));
			locations.Add(new ItemLocation("RB301", "RB301", "item", (InventoryData d) => d.bridgeAccess && d.woodKey));

			//Prayers
			locations.Add(new ItemLocation("PR01", "PR01", "item", (InventoryData d) => true));
			locations.Add(new ItemLocation("PR03", "PR03", "tentudia", (InventoryData d) => d.tentudiaRemains >= 3));
			locations.Add(new ItemLocation("PR04", "PR04", "gemino", (InventoryData d) => (d.fullThimble || d.linen) && d.driedFlowers));
			locations.Add(new ItemLocation("PR05", "PR05", "jocinero", (InventoryData d) => d.bridgeAccess && d.cherubs >= 38));
			locations.Add(new ItemLocation("PR07", "PR07", "item", (InventoryData d) => d.bridgeAccess && d.blood));
			locations.Add(new ItemLocation("PR08", "PR08", "viridiana", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.canBeatBoss("crisanta")));
			locations.Add(new ItemLocation("PR09", "PR09", "item", (InventoryData d) => d.holyWounds >= 3 && d.canBeatBoss("esdras")));
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
			locations.Add(new ItemLocation("QI204", "QI204", "crisanta", (InventoryData d) => d.holyWounds >= 3 && d.canBeatBoss("esdras") && d.blood && d.scapular));
			locations.Add(new ItemLocation("QI301", "QI301", "abnegation", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.trueHeart && d.blood && d.scapular && d.canBeatBoss("crisanta")));

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
			locations.Add(new ItemLocation("BS12", "Tears[4300]", "boss", (InventoryData d) => d.holyWounds >= 3 && d.canBeatBoss("esdras")));
			locations.Add(new ItemLocation("BS13", "Tears[300]", "boss", (InventoryData d) => true));
			locations.Add(new ItemLocation("BS14", "Tears[11250]", "boss", (InventoryData d) => d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey && d.canBeatBoss("quirce")));
			locations.Add(new ItemLocation("BS16", "Tears[18000]", "boss", (InventoryData d) => d.bridgeAccess && d.masks >= 3 && d.canBeatBoss("crisanta")));

			//Weird bosses
			locations.Add(new ItemLocation("D03Z01S03[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles));
			locations.Add(new ItemLocation("D02Z02S14[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.blood && d.root));
			locations.Add(new ItemLocation("D04Z01S04[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess));
			locations.Add(new ItemLocation("D09Z01S01[18000]", "Tears[18000]", "boss", (InventoryData d) => d.bell && d.canBeatBoss("amanecida") && d.canBreakHoles && d.bridgeAccess && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new ItemLocation("LaudesBossTrigger[30000]", "Tears[30000]", "boss", (InventoryData d) => d.bridgeAccess && d.verses >= 4 && d.canBeatBoss("laudes")));
			locations.Add(new ItemLocation("BossTrigger[5000]", "Tears[5000]", "boss", (InventoryData d) => d.bridgeAccess && d.canBeatBoss("sierpes") && (d.root || d.nail || d.cherubAttack(786432) && d.swordLevel > 1)));

			return locations;
		}
	}
}
