using UnityEngine;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities;

namespace BlasphemousRandomizer
{
    public static class EnemyLoader
    {
        private static Dictionary<string, GameObject> allEnemies = new Dictionary<string, GameObject>();
        public static bool loaded = false;

		// Get the gameobject for a certain enemy id
		public static GameObject getEnemy(string id, bool facingLeft)
		{
			if (id == "EN11" || id == "EV15")
            {
				id += facingLeft ? "_L" : "_R";
            }
			if (allEnemies.ContainsKey(id))
			{
				return allEnemies[id];
			}
			return null;
		}

		// Every scene, try to load more enemies
		public static void loadEnemies()
		{
			if (loaded)
				return;

			Enemy[] array = Resources.FindObjectsOfTypeAll<Enemy>();
			for (int i = 0; i < array.Length; i++)
			{
				string baseId = array[i].Id;
				string fullId = baseId;

				// Load separate objects for left/right wall enemies
				if (baseId == "EN11" || baseId == "EV15")
                {
					if (array[i].name.EndsWith("_L"))
						fullId += "_L";
					else if (array[i].name.EndsWith("_R"))
						fullId += "_R";
				}
				// Load separate objects for normal/exploding heads
				if (baseId == "EN09")
                {
					if (array[i].name.Contains("_Exploding"))
                    {
						baseId += "_E";
						fullId += "_E";
                    }
                }
				//Main.Randomizer.Log($"Processing enemy {baseId}({fullId}): {array[i].name}");

				if (baseId != "" && array[i].gameObject.scene.name == null && !allEnemies.ContainsKey(fullId) && FileUtil.arrayContains(enemyIds, baseId))
				{
					//Main.Randomizer.Log($"Loading enemy {baseId}({fullId}): {array[i].name}");
					changeHitbox(array[i].transform, baseId);

					// Chained Angels are nested
					if (baseId == "EV27")
						allEnemies.Add(fullId, array[i].transform.parent.gameObject);
					else
						allEnemies.Add(fullId, array[i].gameObject);
				}
			}

			int totalEnemies = enemyIds.Length + 2;
			if (allEnemies.Count != totalEnemies)
            {
				Main.Randomizer.Log($"Not all enemies processed yet! ({allEnemies.Count}/{totalEnemies})");
            }
			else
			{
				loaded = true;
				Main.Randomizer.Log($"All {totalEnemies} enemies processed!");
			}
		}

		// Certain large enemies need a modifed hitbox
		private static void changeHitbox(Transform transform, string id)
		{
			if (id == "EN16" || id == "EV23")
			{
				Transform child = transform.Find("#Constitution/Canopy");
				if (child != null)
				{
					BoxCollider2D component = child.GetComponent<BoxCollider2D>();
					component.offset = new Vector2(component.offset.x, 1.5f);
					component.size = new Vector2(component.size.x, 3f);
					return;
				}
				Main.Randomizer.Log("Enemy " + id + " had no hitbox to change!");
			}
			else if (id == "EN15" || id == "EV19" || id == "EV26")
            {
				Transform child = transform.Find("#Constitution/Sprite");
				if (child != null)
				{
					BoxCollider2D component2 = child.GetComponent<BoxCollider2D>();
					component2.offset = new Vector2(component2.offset.x, 2f);
					component2.size = new Vector2(component2.size.x, 3.75f);
					return;
				}
				Main.Randomizer.Log("Enemy " + id + " had no hitbox to change!");
			}
		}

		public static string[] enemyIds = new string[]
		{
			"EN01",
			"EN02",
			"EN03",
			"EN04",
			"EN05",
			"EN06",
			"EN07",
			//"EN08",
			"EN09",
			"EN09_E",
			//"EN10",
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
			"EN34",
			"EV01",
			"EV02",
			"EV03",
			//"EV05",
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
}
