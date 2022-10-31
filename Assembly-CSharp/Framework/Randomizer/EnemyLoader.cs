using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Framework.Randomizer
{
	public static class EnemyLoader
	{
		public static void loadEnemies()
		{
			if (EnemyLoader.loaded)
			{
				return;
			}
			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			for (int i = 0; i < array.Length; i++)
			{
				string id = array[i].Id;
				if (id != "" && !EnemyLoader.allEnemies.ContainsKey(id) && FileIO.arrayContains(EnemyLoader.enemyIds, id))
				{
					EnemyLoader.changeHitbox(array[i].transform, id);
					EnemyLoader.allEnemies.Add(array[i].Id, array[i].transform.gameObject);
				}
			}
			if (EnemyLoader.allEnemies.Count == EnemyLoader.enemyIds.Length)
			{
				EnemyLoader.loaded = true;
				Core.Randomizer.Log("All enemies loaded!", 0);
				return;
			}
			Core.Randomizer.Log(string.Concat(new object[]
			{
				"Not all enemies loaded yet! (",
				EnemyLoader.allEnemies.Count,
				"/",
				EnemyLoader.enemyIds.Length,
				")"
			}), 0);
		}

		private static void changeHitbox(Transform transform, string id)
		{
			if (id == "EN16" || id == "EV23")
			{
				Transform transform2 = transform.Find("#Constitution/Canopy");
				if (transform2 != null)
				{
					BoxCollider2D component = transform2.GetComponent<BoxCollider2D>();
					component.offset = new Vector2(component.offset.x, 1.5f);
					component.size = new Vector2(component.size.x, 3f);
					return;
				}
				Core.Randomizer.Log("Enemy " + id + " had no hitbox to change!", 0);
				return;
			}
			else
			{
				if (!(id == "EN15") && !(id == "EV19") && !(id == "EV26"))
				{
					return;
				}
				Transform transform3 = transform.Find("#Constitution/Sprite");
				if (transform3 != null)
				{
					BoxCollider2D component2 = transform3.GetComponent<BoxCollider2D>();
					component2.offset = new Vector2(component2.offset.x, 2f);
					component2.size = new Vector2(component2.size.x, 3.75f);
					return;
				}
				Core.Randomizer.Log("Enemy " + id + " had no hitbox to change!", 0);
				return;
			}
		}

		public static GameObject getEnemy(string id)
		{
			if (EnemyLoader.allEnemies.ContainsKey(id))
			{
				return EnemyLoader.allEnemies[id];
			}
			return null;
		}

		private static Dictionary<string, GameObject> allEnemies = new Dictionary<string, GameObject>();

		public static string[] enemyIds = new string[]
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

		public static bool loaded = false;
	}
}
