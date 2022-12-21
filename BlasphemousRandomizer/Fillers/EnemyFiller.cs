using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class EnemyFiller : Filler
    {
		// 0 - weak, 1 - normal, 2 - difficult, 3 - large, 4 - flying, 5 - vanilla
		private int numTypes = 6;

		public void Fill(int seed, EnemyConfig config, Dictionary<string, string> output)
		{
			initialize(seed);

			// Get lists
			List<EnemyLocation> locations = new List<EnemyLocation>(Main.Randomizer.data.enemyLocations.Values);
			//List<string> enemyIds = new List<string>(Main.Randomizer.data.enemies.Keys);
			List<Enemy> enemies = new List<Enemy>(Main.Randomizer.data.enemies.Values);

			// Get each list of enemies of each type
			List<Enemy>[] enemiesByType = new List<Enemy>[numTypes];
			for (int i = 0; i < enemies.Count; i++)
            {
				if (enemiesByType[enemies[i].type] == null)
					enemiesByType[enemies[i].type] = new List<Enemy>();
				enemiesByType[enemies[i].type].Add(enemies[i]);
            }

			// Get list of ids to place - vanilla enemies are removed
			List<string> possibleIds = new List<string>();
			for (int i = 0; i < enemies.Count; i++)
            {
				if (enemies[i].type != 5)
					possibleIds.Add(enemies[i].id);
            }

			// Each enemy id is randomized to a random id
			if (config.type == 1)
            {
				// Create dictionary to hold new id matchings
				Dictionary<string, string> newEnemyIds = new Dictionary<string, string>();
				shuffleList(enemies);
				
				// Assign a new enemy id to this original one
				foreach (Enemy originalEnemy in enemies)
                {
					int type = originalEnemy.type;
					if (type == 5)
                    {
						// If vanilla enemy, leave it as original
						newEnemyIds.Add(originalEnemy.id, originalEnemy.id);
                    }
					else if (config.maintainClass)
                    {
						// Get new enemy id from the same type as original (Unless vanilla)
						int randIdx = rand(enemiesByType[type].Count);
						newEnemyIds.Add(originalEnemy.id, enemiesByType[type][randIdx].id);
						enemiesByType[type].RemoveAt(randIdx);
					}
                    else
                    {
						// Get new enemy id from any remaining type
						int randIdx = rand(possibleIds.Count);
						newEnemyIds.Add(originalEnemy.id, possibleIds[randIdx]);
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
				shuffleList(enemies);

				for (int i = 0; i < locations.Count; i++)
				{
					int type = Main.Randomizer.data.enemies[locations[i].originalEnemy].type;
					if (type == 5 || locations[i].arena)
                    {
						// If vanilla or arena enemy, leave it as original
						output.Add(locations[i].locationId, locations[i].originalEnemy);
                    }
					else if (config.maintainClass)
                    {
						// Get random id only from same type
						int randIdx = rand(enemiesByType[type].Count);
						output.Add(locations[i].locationId, enemiesByType[type][randIdx].id);
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
    }
}
