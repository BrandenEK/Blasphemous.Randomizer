using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class EnemyFiller : Filler
    {
		private List<EnemyLocation> allLocations;

        public EnemyFiller()
        {
			FileUtil.loadJson("locations_enemies.json", out allLocations);
        }

        public override bool isValid()
        {
			return allLocations.Count > 0;
        }

		public void Fill(int seed, EnemyConfig config, Dictionary<string, string> output)
		{
			initialize(seed);

			// Get lists
			List<EnemyLocation> locations = new List<EnemyLocation>(allLocations);
			List<string> enemyIds = new List<string>(EnemyLoader.enemyIds);

			// Get each list of enemies of each type
			List<string>[] enemyIdsByType = new List<string>[] { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
			for (int i = 0; i < enemyIds.Count; i++)
            {
				enemyIdsByType[enemyTypes[enemyIds[i]]].Add(enemyIds[i]);
            }

			// Get list of ids to place - vanilla enemies are removed
			List<string> possibleIds = new List<string>();
			for (int i = 0; i < enemyIds.Count; i++)
            {
				if (enemyTypes[enemyIds[i]] != 4)
					possibleIds.Add(enemyIds[i]);
            }

			// Each enemy id is randomized to a random id
			if (config.type == 1)
            {
				// Create dictionary to hold new id matchings
				Dictionary<string, string> newEnemyIds = new Dictionary<string, string>();
				shuffleList(enemyIds);
				
				// Assign a new enemy id to this original one
				foreach (string originalId in enemyIds)
                {
					int type = enemyTypes[originalId];
					if (type == 4)
                    {
						// If vanilla or arena enemy, leave it as original
						newEnemyIds.Add(originalId, originalId);
                    }
					else if (config.maintainClass)
                    {
						// Get new enemy id from the same type as original (Unless vanilla)
						int randIdx = rand(enemyIdsByType[type].Count);
						newEnemyIds.Add(originalId, enemyIdsByType[type][randIdx]);
						enemyIdsByType[type].RemoveAt(randIdx);
					}
                    else
                    {
						// Get new enemy id from any remaining type
						int randIdx = rand(possibleIds.Count);
						newEnemyIds.Add(originalId, possibleIds[randIdx]);
						possibleIds.RemoveAt(randIdx);
                    }
                }

				// Assign each location a new enemy id based on its original enemy id
				for (int i = 0; i < locations.Count; i++)
                {
					string newEnemy = locations[i].arena ? locations[i].originalEnemy : newEnemyIds[locations[i].originalEnemy];
					output.Add(locations[i].locationId, newEnemy);
                }
            }
			// Each enemy location is randomized to a random id
            else if (config.type == 2)
            {
				shuffleList(enemyIds);

				for (int i = 0; i < locations.Count; i++)
				{
					int type = enemyTypes[locations[i].originalEnemy];
					if (type == 4 || locations[i].arena)
                    {
						// If vanilla enemy, leave it as original
						output.Add(locations[i].locationId, locations[i].originalEnemy);
                    }
					else if (config.maintainClass)
                    {
						// Get random id only from same type
						int randIdx = rand(enemyIdsByType[type].Count);
						output.Add(locations[i].locationId, enemyIdsByType[type][randIdx]);
					}
                    else
                    {
						// Get random id from all possible ids
						int randIdx = rand(possibleIds.Count);
						output.Add(locations[i].locationId, possibleIds[randIdx]);
                    }
				}
			}
        }

		// 0 - normal, 1 - weak, 2 - large, 3 - flying, 4 - vanilla (for now)
		private Dictionary<string, int> enemyTypes = new Dictionary<string, int>()
		{
			{ "EN01", 1 },
			{ "EN02", 1 },
			{ "EN03", 0 },
			{ "EN04", 3 },
			{ "EN05", 1 },
			{ "EN06", 1 },
			{ "EN07", 3 },
			{ "EN08", 4 },
			{ "EN09", 0 },
			{ "EN10", 4 },
			{ "EN11", 0 },
			{ "EN12", 0 },
			{ "EN13", 0 },
			{ "EN14", 0 },
			{ "EN15", 2 },
			{ "EN16", 2 },
			{ "EN17", 0 },
			{ "EN18", 1 },
			{ "EN20", 0 },
			{ "EN21", 0 },
			{ "EN22", 1 },
			{ "EN23", 3 },
			{ "EN24", 1 },
			{ "EN26", 0 },
			{ "EN27", 3 },
			{ "EN28", 0 },
			{ "EN29", 0 },
			{ "EN31", 3 },
			{ "EN32", 0 },
			{ "EN33", 0 },
			{ "EN34", 4 },
			{ "EV01", 0 },
			{ "EV02", 0 },
			{ "EV03", 3 },
			{ "EV05", 4 },
			{ "EV08", 1 },
			{ "EV10", 1 },
			{ "EV11", 1 },
			{ "EV12", 1 },
			{ "EV13", 1 },
			{ "EV14", 1 },
			{ "EV15", 0 },
			{ "EV17", 3 },
			{ "EV18", 3 },
			{ "EV19", 2 },
			{ "EV20", 4 },
			{ "EV21", 0 },
			{ "EV22", 0 },
			{ "EV23", 2 },
			{ "EV24", 0 },
			{ "EV26", 2 },
			{ "EV27", 0 },
			{ "EV29", 3 },
			{ "EN201", 0 },
			{ "EN202", 2 },
			{ "EN203", 4 }
		};
    }
}
