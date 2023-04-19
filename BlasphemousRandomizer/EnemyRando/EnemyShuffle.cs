﻿using System.Collections.Generic;
using Framework.Audio;
using UnityEngine;

namespace BlasphemousRandomizer.EnemyRando
{
    public class EnemyShuffle : IShuffle
    {
        private Dictionary<string, string> newEnemies;
        private Dictionary<string, int> difficultyRatings;
        private EnemyFiller filler;

        // Enemizer fixes
        public FMODAudioCatalog[] audioCatalogs;

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
            if (difficultyRatings.ContainsKey(id)) return difficultyRatings[id]; // Move this into the json file perhaps ?
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

        public void Init()
        {
            filler = new EnemyFiller();

            // Load from json
            difficultyRatings = new Dictionary<string, int>()
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

        public void Reset()
        {
            newEnemies = null;
        }

        public void Shuffle(int seed)
        {
            if (Main.Randomizer.gameConfig.EnemyShuffleType < 1 || !Main.Randomizer.data.isValid)
                return;

            newEnemies = new Dictionary<string, string>();
            filler.Fill(seed, newEnemies);
            Main.Randomizer.Log(newEnemies.Count + " enemies have been shuffled!");
        }

        // temp
        public static string enemyData = "";
    }
}
