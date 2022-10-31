using System;
using System.Collections.Generic;
using Framework.Managers;
using UnityEngine;

namespace Framework.Randomizer
{
	public class Enemizer
	{
		public Enemizer(int seed)
		{
			this.random = new System.Random(seed);
		}

		public GameObject getEnemy(string id)
		{
			if (EnemyLoader.loaded && Core.Randomizer.gameConfig.enemies.type > 0 && this.shuffledEnemies != null && this.shuffledEnemies.ContainsKey(id) && !FileIO.arrayContains(this.bannedScenes, Core.LevelManager.currentLevel.LevelName))
			{
				return EnemyLoader.getEnemy(this.shuffledEnemies[id]);
			}
			return null;
		}

		private void Randomize()
		{
			if (Core.Randomizer.gameConfig.enemies.type < 1)
			{
				return;
			}
			Core.Randomizer.Log("Randomizing enemies!", 0);
			this.shuffledEnemies = new Dictionary<string, string>();
			List<EnemyLocation> allLocations = new List<EnemyLocation>();
			List<string> allEnemies = new List<string>();
			this.getLists(allLocations, allEnemies);
			if (allLocations.Count != allEnemies.Count)
			{
				Core.Randomizer.Log("Enemizer lists are invalid lengths!", 0);
				return;
			}
			this.fillVanillaLocations(allLocations, allEnemies);
			if (Core.Randomizer.gameConfig.enemies.type == 1)
			{
				List<EnemyLocation> typeLocations = new List<EnemyLocation>();
				List<string> typeEnemies = new List<string>();
				while (allLocations.Count > 0)
				{
					int type = allLocations[0].enemyType;
					this.getLocationsOfType(allLocations, allEnemies, typeLocations, typeEnemies, type);
					this.fillEnemyLocations(typeLocations, typeEnemies);
					typeLocations.Clear();
					typeEnemies.Clear();
				}
			}
			else if (Core.Randomizer.gameConfig.enemies.type == 2)
			{
				this.fillEnemyLocations(allLocations, allEnemies);
			}
			else
			{
				this.fillEnemyLocations(allLocations, allEnemies);
			}
			string text = "Random Enemies:\n\n";
			foreach (string id in this.shuffledEnemies.Keys)
			{
				text = string.Concat(new string[]
				{
					text,
					id,
					" turns into ",
					this.shuffledEnemies[id],
					"\n"
				});
			}
			Core.Randomizer.LogFile(text);
		}

		private void addToDictionary(string id, string enemy)
		{
			this.shuffledEnemies.Add(id, enemy);
		}

		private void fillVanillaLocations(List<EnemyLocation> locations, List<string> enemies)
		{
			List<EnemyLocation> vanillaLocations = new List<EnemyLocation>();
			List<string> vanillaEnemies = new List<string>();
			this.getLocationsOfType(locations, enemies, vanillaLocations, vanillaEnemies, -1);
			for (int i = 0; i < vanillaLocations.Count; i++)
			{
				this.addToDictionary(vanillaLocations[i].enemyId, vanillaEnemies[i]);
			}
		}

		private void fillEnemyLocations(List<EnemyLocation> locations, List<string> enemies)
		{
			while (locations.Count > 0)
			{
				int randIdx = this.random.Next(locations.Count);
				this.addToDictionary(locations[randIdx].enemyId, enemies[enemies.Count - 1]);
				locations.RemoveAt(randIdx);
				enemies.RemoveAt(enemies.Count - 1);
			}
		}

		private void getLocationsOfType(List<EnemyLocation> allLocations, List<string> allEnemies, List<EnemyLocation> typeLocations, List<string> typeEnemies, int type)
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

		private void getLists(List<EnemyLocation> locations, List<string> enemies)
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
			locations.Add(new EnemyLocation(0, "EV20", -1, false));
			locations.Add(new EnemyLocation(0, "EV21", 0, false));
			locations.Add(new EnemyLocation(0, "EV22", 0, false));
			locations.Add(new EnemyLocation(0, "EV23", 2, false));
			locations.Add(new EnemyLocation(0, "EV24", 0, false));
			locations.Add(new EnemyLocation(0, "EV26", 2, false));
			locations.Add(new EnemyLocation(0, "EV27", 0, false));
			locations.Add(new EnemyLocation(0, "EV29", -1, false));
			locations.Add(new EnemyLocation(0, "EN201", 0, false));
			locations.Add(new EnemyLocation(0, "EN202", 2, false));
			locations.Add(new EnemyLocation(0, "EN203", -1, false));
			enemies.Clear();
			foreach (string id in EnemyLoader.enemyIds)
			{
				enemies.Add(id);
			}
		}

		private System.Random random;

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
			"D19Z01S07",
			"D03Z02S13"
		};

		private Dictionary<string, string> shuffledEnemies;
	}
}
