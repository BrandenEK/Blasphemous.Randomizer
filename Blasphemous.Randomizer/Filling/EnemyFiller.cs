using Blasphemous.Randomizer.EnemyRando;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.Filling
{
    public class EnemyFiller : Filler<SingleResult>
    {
        // 0 - normal, 1 - unused, 2 - stationary, 3 - large, 4 - flying, 5 - vanilla
        private int numTypes = 6;
        private int vanillaType = 5;

        public override SingleResult Fill(int seed, Config config)
        {
            Dictionary<string, string> mapping = new();
            initialize(seed);

            // Get lists
            List<EnemyLocation> locations = new List<EnemyLocation>(Main.Randomizer.data.enemyLocations.Values);
            List<EnemyData> enemies = new List<EnemyData>(Main.Randomizer.data.enemies.Values);

            // Only place the same number of each enemy as in original game
            if (config.EnemyShuffleType == 1)
            {
                // Set vanilla/arena locations & add random enemies to list
                List<string> possibleEnemies = new List<string>();
                for (int i = locations.Count - 1; i >= 0; i--)
                {
                    EnemyData original = Main.Randomizer.data.enemies[locations[i].originalEnemy];
                    if (original.type == vanillaType || locations[i].arena)
                    {
                        mapping.Add(locations[i].locationId, locations[i].originalEnemy);
                        locations.RemoveAt(i);
                    }
                    else
                    {
                        possibleEnemies.Add(locations[i].originalEnemy);
                    }
                }
                shuffleList(possibleEnemies);

                // Pick random location and place the first valid enemy in list
                while (locations.Count > 0 && possibleEnemies.Count > 0)
                {
                    int locationIdx = rand(locations.Count);
                    EnemyLocation location = locations[locationIdx];
                    int enemyIdx = possibleEnemies.Count - 1;

                    // Need to find enemy from this class
                    if (config.MaintainClass)
                    {
                        EnemyData original = Main.Randomizer.data.enemies[location.originalEnemy];
                        for (int i = possibleEnemies.Count - 1; i >= 0; i--)
                        {
                            EnemyData enemy = Main.Randomizer.data.enemies[possibleEnemies[i]];
                            if (original.type == enemy.type)
                            {
                                enemyIdx = i;
                                break;
                            }
                        }
                    }

                    mapping.Add(location.locationId, possibleEnemies[enemyIdx]);
                    locations.RemoveAt(locationIdx);
                    possibleEnemies.RemoveAt(enemyIdx);
                }
            }
            // Place any number of each enemy
            else if (config.EnemyShuffleType == 2)
            {
                // Get lists of possible enemy ids for each type
                List<string>[] enemyIdsByType = new List<string>[numTypes];
                List<string> singleList = new List<string>();
                for (int i = 0; i < numTypes; i++)
                {
                    if (config.MaintainClass)
                        enemyIdsByType[i] = new List<string>();
                    else
                        enemyIdsByType[i] = singleList;
                }
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].type != vanillaType)
                        enemyIdsByType[enemies[i].type].Add(enemies[i].id);
                }

                // Pick random enemy of the same type and place it at the location
                for (int i = 0; i < locations.Count; i++)
                {
                    int type = Main.Randomizer.data.enemies[locations[i].originalEnemy].type;
                    if (type == vanillaType || locations[i].arena)
                    {
                        // If vanilla or arena enemy, leave it as original
                        mapping.Add(locations[i].locationId, locations[i].originalEnemy);
                    }
                    else
                    {
                        // If maintainClass, get only id from type, otherwise, get id from all types
                        int randIdx = rand(enemyIdsByType[type].Count);
                        mapping.Add(locations[i].locationId, enemyIdsByType[type][randIdx]);
                    }
                }
            }

            return new SingleResult()
            {
                Mapping = mapping,
                Success = true
            };
        }
    }
}
