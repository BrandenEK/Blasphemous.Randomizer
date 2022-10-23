using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Framework.Randomizer
{
	public class Enemizer
	{
		public Enemizer(int seed)
		{
			this.random = new System.Random(seed);
			this.Randomize();
		}

		public GameObject getEnemy(string id)
		{
			if (Core.Randomizer.gameConfig.enemies.type > 0 && this.newEnemies != null && this.newEnemies.ContainsKey(id) && !FileIO.arrayContains(this.bannedScenes, Core.LevelManager.currentLevel.LevelName))
			{
				return this.newEnemies[id];
			}
			return null;
		}

		public static void loadEnemies()
		{
			if (Enemizer.loadStatus > 0)
			{
				return;
			}
			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			for (int i = 0; i < array.Length; i++)
			{
				string id = array[i].Id;
				if (!Enemizer.allEnemies.ContainsKey(id) && id != "" && FileIO.arrayContains(Enemizer.enemyIds, id))
				{
					Enemizer.allEnemies.Add(array[i].Id, array[i].transform.root.gameObject);
				}
			}
			if (Enemizer.allEnemies.Count == Enemizer.enemyIds.Length)
			{
				Enemizer.loadStatus = 1;
				Core.Randomizer.Log("All enemies loaded!", 0);
				return;
			}
			Core.Randomizer.Log(string.Concat(new object[]
			{
				"Not all enemies loaded yet! (",
				Enemizer.allEnemies.Count,
				"/",
				Enemizer.enemyIds.Length,
				")"
			}), 0);
		}

		public void onSceneLoaded()
		{
			this.Randomize();
		}

		public static void resetStatus()
		{
			Enemizer.loadStatus = ((Enemizer.allEnemies.Count == Enemizer.enemyIds.Length) ? 1 : 0);
		}

		private void Randomize()
		{
			if (Enemizer.loadStatus != 1 || Core.Randomizer.gameConfig.enemies.type == 0)
			{
				return;
			}
			Core.Randomizer.Log("Randomizing enemies!", 0);
			this.newEnemies = new Dictionary<string, GameObject>();
			List<EnemyLocation> list = new List<EnemyLocation>();
			List<GameObject> list2 = new List<GameObject>();
			this.getLists(list, list2);
			if (list.Count != list2.Count)
			{
				Core.Randomizer.Log("Enemizer lists are invalid lengths!", 0);
				return;
			}
			this.fillVanillaLocations(list, list2);
			if (Core.Randomizer.gameConfig.enemies.type == 1)
			{
				List<EnemyLocation> list3 = new List<EnemyLocation>();
				List<GameObject> list4 = new List<GameObject>();
				while (list.Count > 0)
				{
					int enemyType = list[0].enemyType;
					this.getLocationsOfType(list, list2, list3, list4, enemyType);
					this.fillEnemyLocations(list3, list4);
					list3.Clear();
					list4.Clear();
				}
			}
			else if (Core.Randomizer.gameConfig.enemies.type == 2)
			{
				this.fillEnemyLocations(list, list2);
			}
			else
			{
				this.fillEnemyLocations(list, list2);
			}
			Enemizer.loadStatus = 2;
			string text = "Random Enemies:\n\n";
			foreach (string text2 in this.newEnemies.Keys)
			{
				text = string.Concat(new string[]
				{
					text,
					text2,
					" turns into ",
					this.newEnemies[text2].name,
					"\n"
				});
			}
			Core.Randomizer.LogFile(text);
		}

		private void addToDictionary(string id, GameObject enemy)
		{
			this.newEnemies.Add(id, enemy);
		}

		private void fillVanillaLocations(List<EnemyLocation> locations, List<GameObject> enemies)
		{
			List<EnemyLocation> list = new List<EnemyLocation>();
			List<GameObject> list2 = new List<GameObject>();
			this.getLocationsOfType(locations, enemies, list, list2, -1);
			for (int i = 0; i < list.Count; i++)
			{
				this.addToDictionary(list[i].enemyId, list2[i]);
			}
		}

		private void fillEnemyLocations(List<EnemyLocation> locations, List<GameObject> enemies)
		{
			while (locations.Count > 0)
			{
				int index = this.random.Next(locations.Count);
				this.addToDictionary(locations[index].enemyId, enemies[enemies.Count - 1]);
				locations.RemoveAt(index);
				enemies.RemoveAt(enemies.Count - 1);
			}
		}

		private void getLocationsOfType(List<EnemyLocation> allLocations, List<GameObject> allEnemies, List<EnemyLocation> typeLocations, List<GameObject> typeEnemies, int type)
		{
			for (int i = 0; i < allLocations.Count; i++)
			{
				if (allLocations[i].enemyType == type)
				{
					typeLocations.Add(allLocations[i]);
					typeEnemies.Add(allEnemies[i]);
					allLocations.RemoveAt(i);
					allEnemies.RemoveAt(i);
					i--;
				}
			}
		}

		private void getLists(List<EnemyLocation> locations, List<GameObject> enemies)
		{
			locations.Clear();
			locations.Add(new EnemyLocation(0, "EN01", 1, false));
			locations.Add(new EnemyLocation(0, "EN02", 0, false));
			locations.Add(new EnemyLocation(0, "EN03", 0, false));
			locations.Add(new EnemyLocation(0, "EN04", 0, false));
			locations.Add(new EnemyLocation(0, "EN05", 1, false));
			locations.Add(new EnemyLocation(0, "EN06", 1, false));
			locations.Add(new EnemyLocation(0, "EN07", 0, false));
			locations.Add(new EnemyLocation(0, "EN08", 1, false));
			locations.Add(new EnemyLocation(0, "EN09", 0, false));
			locations.Add(new EnemyLocation(0, "EN10", -1, false));
			locations.Add(new EnemyLocation(0, "EN11", 0, false));
			locations.Add(new EnemyLocation(0, "EN12", 0, false));
			locations.Add(new EnemyLocation(0, "EN13", 0, false));
			locations.Add(new EnemyLocation(0, "EN14", 0, false));
			locations.Add(new EnemyLocation(0, "EN15", 2, false));
			locations.Add(new EnemyLocation(0, "EN16", 2, false));
			locations.Add(new EnemyLocation(0, "EN17", 0, false));
			locations.Add(new EnemyLocation(0, "EN18", 0, false));
			locations.Add(new EnemyLocation(0, "EN20", 0, false));
			locations.Add(new EnemyLocation(0, "EN21", 0, false));
			locations.Add(new EnemyLocation(0, "EN22", 1, false));
			locations.Add(new EnemyLocation(0, "EN23", 0, false));
			locations.Add(new EnemyLocation(0, "EN24", 0, false));
			locations.Add(new EnemyLocation(0, "EN26", 0, false));
			locations.Add(new EnemyLocation(0, "EN27", -1, false));
			locations.Add(new EnemyLocation(0, "EN28", 0, false));
			locations.Add(new EnemyLocation(0, "EN29", 0, false));
			locations.Add(new EnemyLocation(0, "EN31", 0, false));
			locations.Add(new EnemyLocation(0, "EN32", 0, false));
			locations.Add(new EnemyLocation(0, "EN33", 0, false));
			locations.Add(new EnemyLocation(0, "EV01", 0, false));
			locations.Add(new EnemyLocation(0, "EV02", 0, false));
			locations.Add(new EnemyLocation(0, "EV03", 0, false));
			locations.Add(new EnemyLocation(0, "EV05", -1, false));
			locations.Add(new EnemyLocation(0, "EV08", 1, false));
			locations.Add(new EnemyLocation(0, "EV10", 0, false));
			locations.Add(new EnemyLocation(0, "EV11", 1, false));
			locations.Add(new EnemyLocation(0, "EV12", 1, false));
			locations.Add(new EnemyLocation(0, "EV13", 0, false));
			locations.Add(new EnemyLocation(0, "EV14", 0, false));
			locations.Add(new EnemyLocation(0, "EV15", 0, false));
			locations.Add(new EnemyLocation(0, "EV17", -1, false));
			locations.Add(new EnemyLocation(0, "EV18", -1, false));
			locations.Add(new EnemyLocation(0, "EV19", 2, false));
			locations.Add(new EnemyLocation(0, "EV20", 4, false));
			locations.Add(new EnemyLocation(0, "EV21", 0, false));
			locations.Add(new EnemyLocation(0, "EV22", 0, false));
			locations.Add(new EnemyLocation(0, "EV23", 2, false));
			locations.Add(new EnemyLocation(0, "EV24", 0, false));
			locations.Add(new EnemyLocation(0, "EV26", 2, false));
			locations.Add(new EnemyLocation(0, "EV27", 0, false));
			locations.Add(new EnemyLocation(0, "EV29", -1, false));
			locations.Add(new EnemyLocation(0, "EN201", 0, false));
			locations.Add(new EnemyLocation(0, "EN202", 2, false));
			locations.Add(new EnemyLocation(0, "EN203", 4, false));
			enemies.Clear();
			foreach (string key in Enemizer.enemyIds)
			{
				enemies.Add(Enemizer.allEnemies[key]);
			}
		}

		private System.Random random;

		private Dictionary<string, GameObject> newEnemies;

		private static Dictionary<string, GameObject> allEnemies = new Dictionary<string, GameObject>();

		private static string[] enemyIds = new string[]
		{
			"EN01",
			"EN02",
			"EN03",
			"EN04",
			"EN05",
			"EN06",
			"EN07",
			"EN08",
			"EN09",
			"EN10",
			"EN11",
			"EN12",
			"EN13",
			"EN14",
			"EN15",
			"EN16",
			"EN17",
			"EN18",
			"EN20",
			"EN21",
			"EN22",
			"EN23",
			"EN24",
			"EN26",
			"EN27",
			"EN28",
			"EN29",
			"EN31",
			"EN32",
			"EN33",
			"EV01",
			"EV02",
			"EV03",
			"EV05",
			"EV08",
			"EV10",
			"EV11",
			"EV12",
			"EV13",
			"EV14",
			"EV15",
			"EV17",
			"EV18",
			"EV19",
			"EV20",
			"EV21",
			"EV22",
			"EV23",
			"EV24",
			"EV26",
			"EV27",
			"EV29",
			"EN201",
			"EN202",
			"EN203"
		};

		private static int loadStatus = 0;

		private string[] bannedScenes = new string[]
		{
			"D03Z03S12",
			"D03Z03S03",
			"D03Z03S05",
			"D01Z04S07",
			"D01Z04S11",
			"D06Z01S03",
			"D06Z01S20",
			"D06Z01S06",
			"D06Z01S21",
			"D19Z01S01",
			"D19Z01S02",
			"D19Z01S03",
			"D19Z01S04",
			"D19Z01S05",
			"D19Z01S06",
			"D19Z01S07"
		};
	}
}
