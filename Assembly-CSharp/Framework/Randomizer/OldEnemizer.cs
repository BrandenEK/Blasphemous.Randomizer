using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Framework.Randomizer
{
	public class OldEnemizer
	{
		public OldEnemizer()
		{
			this.count = 0;
			this.enemyIds = new string[]
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
		}

		public void loadEnemies()
		{
			this.enemies = new Dictionary<string, GameObject>();
			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			List<string> list = new List<string>();
			List<GameObject> list2 = new List<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Id != "" && !array[i].Id.Contains("BS") && !list.Contains(array[i].Id))
				{
					list.Add(array[i].Id);
					list2.Add(array[i].transform.root.gameObject);
				}
			}
			this.randomizeEnemies(this.enemies, list, list2);
			string text = "Enemies loaded: " + this.enemies.Count + "\n";
			this.defaultEnemy = this.enemies["EN22"];
			foreach (string text2 in this.enemies.Keys)
			{
				text = string.Concat(new object[]
				{
					text,
					text2,
					" turns into ",
					this.enemies[text2],
					"\n"
				});
			}
			Core.Randomizer.LogFile(text);
			for (int j = 0; j < array.Length; j++)
			{
				Core.Randomizer.Log(array[j].Id + ": " + array[j].EntityName);
			}
		}

		public GameObject getEnemy(string id)
		{
			if (id == "EV17")
			{
				return null;
			}
			if (this.enemies.ContainsKey(id))
			{
				return this.enemies[id];
			}
			Core.Randomizer.Log("Enemy: " + id + " does not exist!", 0);
			return null;
		}

		private void randomizeEnemies(Dictionary<string, GameObject> enemies, List<string> ids, List<GameObject> objects)
		{
			System.Random random = new System.Random();
			while (ids.Count > 0)
			{
				int index = random.Next(ids.Count);
				enemies.Add(ids[index], objects[objects.Count - 1]);
				ids.RemoveAt(index);
				objects.RemoveAt(objects.Count - 1);
			}
		}

		public void setNewEnemy()
		{
			this.count++;
			if (this.count >= this.loadedEnemies.Count)
			{
				return;
			}
			this.enemies.Clear();
			foreach (string key in this.enemyIds)
			{
				this.enemies.Add(key, this.loadedEnemies[this.count]);
			}
			Core.Randomizer.Log("Setting enemy to " + this.loadedEnemies[this.count].name, 0);
		}

		public void load()
		{
			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			this.loadedEnemies = new List<GameObject>();
			this.enemies = new Dictionary<string, GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Id != "" && !array[i].Id.Contains("BS"))
				{
					this.loadedEnemies.Add(array[i].gameObject);
				}
			}
			this.count = -1;
			this.setNewEnemy();
		}

		public void sceneLoad()
		{
			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			Core.Randomizer.Log("Loaded enemies: " + array.Length);
		}

		public Dictionary<string, GameObject> enemies;

		private int count;

		public GameObject defaultEnemy;

		private string[] enemyIds;

		private List<GameObject> loadedEnemies;
	}
}
