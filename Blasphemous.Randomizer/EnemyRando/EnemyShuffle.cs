using Blasphemous.Randomizer.Filling;
using Framework.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Randomizer.EnemyRando
{
    public class EnemyShuffle : IShuffle
    {
        private readonly Filler<SingleResult> _filler;

        private Dictionary<string, string> newEnemies;

        public EnemyShuffle(Filler<SingleResult> filler)
        {
            _filler = filler;
        }

        // Enemizer fixes
        public FMODAudioCatalog[] audioCatalogs;

        // Manage mapped enemies
        public Dictionary<string, string> SaveMappedEnemies() => newEnemies;
        public void LoadMappedEnemies(Dictionary<string, string> mappedEnemies) => newEnemies = mappedEnemies;
        public void ClearMappedEnemies() => newEnemies = null;

        public GameObject getEnemy(string id, bool facingLeft)
        {
            if (EnemyLoader.loaded && newEnemies != null && newEnemies.ContainsKey(id))
            {
                return EnemyLoader.getEnemy(newEnemies[id], facingLeft);
            }
            return null;
        }

        // Gets the y offset of a specific enemy 
        public float getEnemyOffset(string id)
        {
            if (Main.Randomizer.data.enemies.TryGetValue(id, out EnemyData enemy))
                return enemy.yOffset;
            return 0f;
        }

        // Gets the difficulty rating of a specific enemy
        public int getEnemyRating(string id)
        {
            if (Main.Randomizer.data.enemies.TryGetValue(id, out EnemyData enemy))
                return enemy.difficulty;
            return 0;
        }

        // Gets the y offset of a specific location
        public float getLocationOffset(string id)
        {
            if (Main.Randomizer.data.enemyLocations.TryGetValue(id, out EnemyLocation location))
                return location.yOffset;
            return 99;
        }

        // Gets the difficulty rating of a specific location
        public int getLocationRating(string id)
        {
            if (_difficultyRatings.ContainsKey(id)) return _difficultyRatings[id]; // Move this into the json file perhaps ?
            if (id == "D19Z01S01") return 2;
            if (id == "D19Z01S02") return 3;
            if (id == "D19Z01S03") return 4;
            if (id == "D19Z01S04") return 6;
            if (id == "D19Z01S05") return 7;
            if (id == "D19Z01S06") return 8;
            if (id == "D19Z01S07") return 10;
            Main.Randomizer.LogError("Area rating " + id + " does not exist!");
            return 0;
        }

        public bool Shuffle(int seed, Config config)
        {
            if (Main.Randomizer.GameSettings.EnemyShuffleType < 1)
                return true;

            var result = _filler.Fill(seed, config);
            newEnemies = result.Mapping;

            Main.Randomizer.Log(newEnemies.Count + " enemies have been shuffled!");
            return true;
        }

        // temp
        public static string enemyData = "";

        private readonly Dictionary<string, int> _difficultyRatings = new()
        {
            { "D01Z01", 1 },
            { "D01Z03", 2 },
            { "D01Z04", 2 },
            { "D01Z05", 3 },
            { "D03Z01", 3 },
            { "D02Z01", 3 },
            { "D02Z02", 4 },
            { "D03Z02", 4 },
            { "D20Z01", 5 },
            { "D03Z03", 6 },
            { "D02Z03", 6 },
            { "D04Z01", 7 },
            { "D04Z02", 7 },
            { "D05Z01", 8 },
            { "D05Z02", 8 },
            { "D06Z01", 9 },
            { "D09Z01", 10 },
            { "D09BZ0", 10 },
            { "D20Z02", 11 }
        };
    }
}
