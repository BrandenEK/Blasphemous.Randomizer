using UnityEngine;
using System.Collections.Generic;
using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.EnemyRando;
using Blasphemous.Randomizer.ItemRando;
using System.Linq;
using Newtonsoft.Json;
using Blasphemous.ModdingAPI.Files;

namespace Blasphemous.Randomizer
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

		// Image data
		private Sprite[] randomizerImages;
		public Sprite ImageCherub => randomizerImages[0];
		public Sprite ImageHealth => randomizerImages[1];
		public Sprite ImageFervour => randomizerImages[2];
		public Sprite ImageSword => randomizerImages[3];
		public Sprite ImageDash => randomizerImages[4];
		public Sprite ImageWallClimb => randomizerImages[5];

		// New & improved
		public Dictionary<string, string> LocationNames { get; private set; }
		public Dictionary<string, string> ShopInteractables { get; private set; }
		public Dictionary<string, Vector3> FixedDoorPositions { get; private set; }
		public Dictionary<string, string> FixedDoorWalls { get; private set; }
		public string[] CutsceneNames { get; private set; }
		public string[] CutsceneFlags { get; private set; }

		public Sprite Logo { get; private set; }

		public bool loadData()
		{
			bool valid = true;

            // Items - Need to special load these
            items = Main.Randomizer.FileHandler.LoadDataAsText("items.json", out string jsonItems)
				? processItems(jsonItems)
				: new Dictionary<string, Item>();
            Main.Randomizer.Log($"Loaded {items.Count} items!");

            // Item locations
            itemLocations = Main.Randomizer.FileHandler.LoadDataAsJson("locations_items.json", out ItemLocation[] tempItemLocations)
                ? tempItemLocations.ToDictionary(x => x.Id, x => x)
                : new Dictionary<string, ItemLocation>();
            Main.Randomizer.Log($"Loaded {itemLocations.Count} item locations!");

            // Enemies
            enemies = Main.Randomizer.FileHandler.LoadDataAsJson("enemies.json", out EnemyData[] tempEnemies)
                ? tempEnemies.ToDictionary(x => x.id, x => x)
                : new Dictionary<string, EnemyData>();
            Main.Randomizer.Log($"Loaded {enemies.Count} enemies!");

            // Enemy locations
            enemyLocations = Main.Randomizer.FileHandler.LoadDataAsJson("locations_enemies.json", out EnemyLocation[] tempEnemyLocations)
                ? tempEnemyLocations.ToDictionary(x => x.locationId, x => x)
                : new Dictionary<string, EnemyLocation>();
            Main.Randomizer.Log($"Loaded {enemyLocations.Count} enemy locations!");

            // Doors
            doorLocations = Main.Randomizer.FileHandler.LoadDataAsJson("doors.json", out DoorLocation[] tempDoors)
                ? tempDoors.ToDictionary(x => x.Id, x => x)
                : new Dictionary<string, DoorLocation>();
            Main.Randomizer.Log($"Loaded {doorLocations.Count} doors!");

            // Images
            Main.Randomizer.FileHandler.LoadDataAsFixedSpritesheet("rando-items.png", new Vector2(30, 30), out randomizerImages, new SpriteImportOptions()
            {
                PixelsPerUnit = 30
            });
            Main.Randomizer.Log($"Loaded {randomizerImages.Length} randomizer images!");

			// Logo
			Main.Randomizer.FileHandler.LoadDataAsSprite("logo.png", out Sprite logo);
			Logo = logo;

            CreateInternalData();
			isValid = valid;
			return valid;
		}

		// Fills the dictionary with the items parsed from the json string
		private Dictionary<string, Item> processItems(string json) // items.json file can't end with a comma for this to work!
		{
            Dictionary<string, Item> items = new();

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
					ProgressiveItem item = JsonConvert.DeserializeObject<ProgressiveItem>(array[i]);
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
					Item item = JsonConvert.DeserializeObject<Item>(array[i]);
					items.Add(item.id, item);
				}
			}

			return items;
		}

		private void CreateInternalData()
        {
			LocationNames = new Dictionary<string, string>()
			{
				{ "D01Z01", "The Holy Line" },
				{ "D01Z02", "Albero" },
				{ "D01Z03", "Wasteland of the Buried Churches" },
				{ "D01Z04", "Mercy Dreams" },
				{ "D01Z05", "Desecrated Cistern" },
				{ "D01Z06", "Petrous" },

				{ "D02Z01", "Where Olive Trees Wither" },
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
				{ "D05Z02", "The Sleeping Canvases" },

				{ "D06Z01", "Archcathedral Rooftops" },
				{ "D07Z01", "Deambulatory of his Holiness" },

				{ "D08Z01", "Bridge of the Three Calvaries" },
				{ "D08Z02", "Ferrous Tree" },
				{ "D08Z03", "Hall of the Dawning" },
				{ "D09Z01", "Wall of the Holy Prohibitions" },

				{ "D17Z01", "Brotherhood of the Silent Sorrow" },
				{ "D20Z01", "Echoes of Salt" },
				{ "D20Z02", "Mourning and Havoc" },
				{ "D20Z03", "The Resting Place of the Sister" },

				{ "Initia", "Various" },
			};

			ShopInteractables = new Dictionary<string, string>()
			{
				{ "225a8ff2-be3f-4c10-bd90-e88d8671d0ac", "QI58" },
				{ "a927f0a1-0431-40f1-952e-87383c97194a", "RB05" },
				{ "5d413c5a-63a7-4053-bf21-a13060b84e15", "RB09" },
				{ "a23c71b5-d29c-4b7b-8425-8722b9c4c4a0", "QI11" },
				{ "36709870-5584-41c9-83d1-627d1eae9b2b", "RB37" },
				{ "c5952d17-848b-4027-8b82-7cb04349c0f2", "RB02" },
				{ "bed4fc04-7ae6-41b3-8a9c-6ae292825aeb", "QI71" },
				{ "c0af12b1-a4a0-4d33-a858-ba3bd393a95a", "RB12" },
				{ "208bff40-6ae6-4bfc-a906-c182b3aa5439", "QI49" },
			};

			CutsceneNames = new string[]
			{
				"IntroBrotherhood",
				"IntroDeosgracias",
				"CTS12-Intro2",
				"CTS07-Deosgracias",
				"CTS02-Bloody Baptism",
				//"MeaCulpa",
				//"CTS04-Dagger_Lady",
				"CTS102-Santos1",
				"CTS103-Santos2",
				"CTS105-LaudesAwakening",
				"CTS201-MiriamIntro",
				"CTS08-Throne",
			};

			CutsceneFlags = new string[]
			{
				"D17Z01S01_INTRO",
				"PONTIFF_ALBERO_EVENT",
				"PONTIFF_BRIDGE_EVENT",
				"PONTIFF_KEY1_USED",
				"PONTIFF_KEY2_USED",
				"PONTIFF_KEY3_USED",
				"PONTIFF_ARCHDEACON1_EVENT",
				"PONTIFF_ARCHDEACON2_EVENT",
				"BROTHERS_EVENT1_COMPLETED",
				"BROTHERS_EVENT2_COMPLETED",
				"BROTHERS_GRAVEYARD_EVENT",
				"BROTHERS_WASTELAND_EVENT",
				"SANTOS_LAUDES_CUTSCENE_PLAYED",
				"MEACULPA_CUTSCENE_PLAYED",
				"DAGGER_CUTSCENE_FINISHED",
			};

			FixedDoorPositions = new Dictionary<string, Vector3>()
			{
				{ "D01BZ06S01[E]", new Vector3(-433, -2) },
				{ "D01Z04S15[W]", new Vector3(-125, -60) },
				{ "D01Z05S03[W]", new Vector3(-387, -49) },
				{ "D02Z01S06[E]", new Vector3(-253.5f, 42) },
				{ "D03Z03S07[NW]", new Vector3(-606, -199) },
				{ "D05Z02S06[SE]", new Vector3(286, -101) },
				{ "D08Z02S03[W]", new Vector3(-7, 40) },
				{ "D09Z01S03[W]", new Vector3(46, 106) },
				{ "D09Z01S08[W]", new Vector3(33, 117) },
				{ "D09Z01S09[NW]", new Vector3(73, 99) },
				{ "D20Z01S04[E]", new Vector3(-493, -114) },
			};

			FixedDoorWalls = new Dictionary<string, string>()
			{
				{ "D01Z01S01[S]", "GEO_DestroyableBlock (32x128)/ACT_HiddenArea/SecretArea/forest-spritesheet_23 (1)" },
				{ "D01Z04S10[SW]", "GEO_DestroyableBlock (32x128)/ACT_HiddenArea/SecretRoom/cathedral-sprite-sheet_70" },
				{ "D03Z01S01[S]", "PassageVisualBlocker" },
				{ "D05Z01S02[W]", "PassageVisualBlocker" },
			};
		}
	}
}
