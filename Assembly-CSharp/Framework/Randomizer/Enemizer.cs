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

		private void Randomize()
		{
			if (Enemizer.loadStatus != 1 || Core.Randomizer.gameConfig.enemies.type == 0)
			{
				return;
			}
			Core.Randomizer.Log("Randomizing enemies!", 0);
			this.newEnemies = new Dictionary<string, GameObject>();
			List<string> list = new List<string>(Enemizer.enemyIds);
			List<GameObject> list2 = new List<GameObject>(Enemizer.allEnemies.Values);
			foreach (string text in new string[]
			{
				"EN10",
				"EN27",
				"EV05",
				"EV17",
				"EV18",
				"EV29"
			})
			{
				this.newEnemies.Add(text, Enemizer.allEnemies[text]);
				list.Remove(text);
				list2.Remove(Enemizer.allEnemies[text]);
			}
			string text2 = "Random enemies:\n\n";
			while (list.Count > 0)
			{
				int index = this.random.Next(list.Count);
				this.newEnemies.Add(list[index], list2[list2.Count - 1]);
				text2 = string.Concat(new string[]
				{
					text2,
					list[index],
					" turns into ",
					list2[list2.Count - 1].name,
					"\n"
				});
				list.RemoveAt(index);
				list2.RemoveAt(list2.Count - 1);
			}
			Core.Randomizer.LogFile(text2);
			Enemizer.loadStatus = 2;
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
