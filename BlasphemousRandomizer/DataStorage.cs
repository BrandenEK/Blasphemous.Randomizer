﻿using UnityEngine;
using System.Collections.Generic;
using BlasphemousRandomizer.DoorRando;
using BlasphemousRandomizer.EnemyRando;
using BlasphemousRandomizer.ItemRando;
using ModdingAPI;

namespace BlasphemousRandomizer
{
	public class DataStorage
	{
		public bool isValid { get; private set; }

		// Json data
		public Dictionary<string, Item> items;
		public Dictionary<string, ItemLocation> itemLocations;
		public Dictionary<string, EnemyData> enemies;
		public Dictionary<string, EnemyLocation> enemyLocations;
		public Dictionary<string, DoorLocation> doorLocations;

		// Text data
		public Dictionary<string, string> interactableIds;
		public string[] cutsceneNames;
		public string[] cutsceneFlags;

		// Image data
		public Sprite[] randomizerImages;
		public Sprite[] uiImages;

		// New & improved
		public Dictionary<string, string> LocationNames { get; private set; }

		public bool loadData(FileUtil fileUtil)
		{
			bool valid = true;

			// Items - Need to special load these
			items = new Dictionary<string, Item>();
			if (fileUtil.loadDataText("items.json", out string json))
			{
				processItems(fileUtil, items, json);
				Main.Randomizer.Log($"Loaded {items.Count} items!");
			}
			else { Main.Randomizer.LogError("Error: Failed to load items!"); valid = false; }

			// Item locations
			itemLocations = new Dictionary<string, ItemLocation>();
			if (fileUtil.loadDataText("locations_items.json", out string jsonItemLocations))
			{
				List<ItemLocation> tempItemLocations = fileUtil.jsonObject<List<ItemLocation>>(jsonItemLocations);
				for (int i = 0; i < tempItemLocations.Count; i++)
					itemLocations.Add(tempItemLocations[i].Id, tempItemLocations[i]);
				Main.Randomizer.Log($"Loaded {itemLocations.Count} item locations!");
			}
			else { Main.Randomizer.LogError("Error: Failed to load item locations!"); valid = false; }

			// Enemies
			enemies = new Dictionary<string, EnemyData>();
			if (fileUtil.loadDataText("enemies.json", out string jsonEnemies))
			{
				List<EnemyData> tempEnemies = fileUtil.jsonObject<List<EnemyData>>(jsonEnemies);
				for (int i = 0; i < tempEnemies.Count; i++)
					enemies.Add(tempEnemies[i].id, tempEnemies[i]);
				Main.Randomizer.Log($"Loaded {enemies.Count} enemies!");
			}
			else { Main.Randomizer.LogError("Error: Failed to load enemies!"); valid = false; }

			// Enemy locations
			enemyLocations = new Dictionary<string, EnemyLocation>();
			if (fileUtil.loadDataText("locations_enemies.json", out string jsonEnemyLocations))
			{
				List<EnemyLocation> tempEnemyLocations = fileUtil.jsonObject<List<EnemyLocation>>(jsonEnemyLocations);
				for (int i = 0; i < tempEnemyLocations.Count; i++)
					enemyLocations.Add(tempEnemyLocations[i].locationId, tempEnemyLocations[i]);
				Main.Randomizer.Log($"Loaded {enemyLocations.Count} enemy locations!");
			}
			else { Main.Randomizer.LogError("Error: Failed to load enemy locations!"); valid = false; }

			// Doors
			doorLocations = new Dictionary<string, DoorLocation>();
			if (fileUtil.loadDataText("doors.json", out string jsonDoors))
			{
				List<DoorLocation> tempDoorLocations = fileUtil.jsonObject<List<DoorLocation>>(jsonDoors);
				for (int i = 0; i < tempDoorLocations.Count; i++)
					doorLocations.Add(tempDoorLocations[i].Id, tempDoorLocations[i]);
				Main.Randomizer.Log($"Loaded {doorLocations.Count} doors!");
			}
			else { Main.Randomizer.LogError("Error: Failed to load doors!"); valid = false; }

			// Load text data
			if (fileUtil.loadDataDictionary("interactable_ids.dat", out interactableIds)) Main.Randomizer.Log($"Loaded {interactableIds.Count} interactable ids!");
			else { Main.Randomizer.LogError("Error: Failed to load interactable ids!"); valid = false; }
			if (fileUtil.loadDataArray("cutscenes_names.dat", out cutsceneNames)) Main.Randomizer.Log($"Loaded {cutsceneNames.Length} cutscene names!");
			else { Main.Randomizer.LogError("Error: Failed to load cutscene names!"); valid = false; }
			if (fileUtil.loadDataArray("cutscenes_flags.dat", out cutsceneFlags)) Main.Randomizer.Log($"Loaded {cutsceneFlags.Length} cutscene flags!");
			else { Main.Randomizer.LogError("Error: Failed to load cutscene flags!"); valid = false; }

			// Load image data
			if (fileUtil.loadDataImages("custom_images.png", 32, 32, 32, 0, true, out randomizerImages)) Main.Randomizer.Log($"Loaded {randomizerImages.Length} randomizer images!");
			else { Main.Randomizer.LogError("Error: Failed to load randomizer images!"); valid = false; }
			if (fileUtil.loadDataImages("ui.png", 36, 36, 36, 0, false, out uiImages)) Main.Randomizer.Log($"Loaded {uiImages.Length} ui images!");
			else { Main.Randomizer.LogError("Error: Failed to load ui images!"); valid = false; }

			CreateInternalData();
			isValid = valid;
			return valid;
		}

		// Fills the dictionary with the items parsed from the json string
		private void processItems(FileUtil fileUtil, Dictionary<string, Item> items, string json) // items.json file can't end with a comma for this to work!
		{
			// Parse json string into array
			json = json.Replace("},", "}*");
			json = json.Substring(1, json.Length - 2);
			string[] array = json.Split('*');

			// Determine if item is progressive or not and add to dictionary
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains("\"removePrevious\""))
				{
					// Progressive item
					ProgressiveItem item = fileUtil.jsonObject<ProgressiveItem>(array[i]);
					if (item.items == null)
					{
						item.items = new string[item.count];
						for (int j = 0; j < item.count; j++)
							item.items[j] = item.id + (j + 1).ToString("00");
					}
					items.Add(item.id, item);
				}
				else
				{
					// Regular item
					Item item = fileUtil.jsonObject<Item>(array[i]);
					items.Add(item.id, item);
				}
			}
		}

		private void CreateInternalData()
        {
			LocationNames = new Dictionary<string, string>()
			{
				{ "D01Z01", "The Holy Line" },
				{ "D01Z02", "Albero" },
				{ "D01Z03", "Wasteland of the Buried Churches" },
				{ "D01Z04", "Mercy Dreams" },
				{ "D01Z05", "The Desecrated Cistern" },
				{ "D01Z06", "Petrous" },

				{ "D02Z01", "Where the Olive Trees Wither" },
				{ "D02Z02", "Graveyard of the Peaks" },
				{ "D02Z03", "Convent of Our Lady of the Charred Visage" },

				{ "D03Z01", "Mountains of the Endless Dusk" },
				{ "D03Z02", "Jondo" },
				{ "D03Z03", "Grievance Ascends" },

				{ "D04Z01", "Patio of the Silent Steps" },
				{ "D04Z02", "Mother of Mothers" },
				{ "D04Z03", "Knot of the Three Words" },
				{ "D04Z04", "All the Tears of the Sea" },

				{ "D05Z01", "Library of the Negated Words" },
				{ "D05Z02", "Sleeping Canvases" },

				{ "D06Z01", "Archcathedral Rooftops" },
				{ "D07Z01", "Deambulatory of his Holiness" },

				{ "D08Z01", "Bridge of the Three Calvaries" },
				{ "D08Z02", "Ferrous Tree" },
				{ "D08Z03", "Hall of the Dawning" },
				{ "D09Z01", "Wall of the Holy Prohibitions" },

				{ "D17Z01", "Brotherhood of the Silent Sorrow" },
				{ "D20Z01", "Echoes of Salt" },
				{ "D20Z02", "Mourning and Havoc" },
				{ "D20Z03", "Resting Place of the Sister" },

				{ "Initia", "Various" },
			};
        }
	}
}
