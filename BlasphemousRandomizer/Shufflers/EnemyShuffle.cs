using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;
using BlasphemousRandomizer.Structures;
using Framework.Managers;
using Framework.Audio;
using UnityEngine;

namespace BlasphemousRandomizer.Shufflers
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

        // Gets the difficulty rating of a certain enemy or area
        public int getRating(string id)
        {
            if (difficultyRatings.ContainsKey(id)) return difficultyRatings[id];
            if (id == "D19Z01S01") return 2;
            if (id == "D19Z01S02") return 3;
            if (id == "D19Z01S03") return 4;
            if (id == "D19Z01S04") return 6;
            if (id == "D19Z01S05") return 7;
            if (id == "D19Z01S06") return 8;
            if (id == "D19Z01S07") return 10;
            Main.Randomizer.Log("Enemy/Area rating " + id + " does not exist!");
            return 0;
        }

        // Gets the y offset of certain enemies 
        public float getEnemyOffset(string id)
        {
            if (id == "EN02") return 1.7f;
            if (id == "EN03") return 1.65f;
            if (id == "EN05") return 0.9f;
            if (id == "EN09") return -0.19f;
            if (id == "EN09_E") return -0.19f;
            if (id == "EN14") return 3.2f;
            if (id == "EN15") return 0.18f;
            if (id == "EN16") return 0.1f;
            if (id == "EN34") return 0.18f;
            if (id == "EV01") return 1.75f;
            if (id == "EV02") return 1.7f;
            if (id == "EV13") return -0.19f;
            if (id == "EV14") return -0.19f;
            if (id == "EV19") return 0.1f;
            if (id == "EV23") return 0.1f;
            if (id == "EV27") return 2f;
            return 0f;
        }

        // Gets the y offset of certain locations
        public float getLocationOffset(string id)
        {
            if (Main.Randomizer.data.enemyLocations.TryGetValue(id, out EnemyLocation location))
                return location.yOffset;
            return 99;
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
                { "D20Z02", 11 },
                { "EN01", 2 },
                { "EN02", 2 },
                { "EN03", 4 },
                { "EN04", 4 },
                { "EN05", 2 },
                { "EN06", 3 },
                { "EN07", 6 },
                { "EN08", 3 },
                { "EN09", 8 },
                { "EN09_E", 8 },
                { "EN10", 8 },
                { "EN11", 6 },
                { "EN12", 6 },
                { "EN13", 2 },
                { "EN14", 4 },
                { "EN15", 2 },
                { "EN16", 7 },
                { "EN17", 4 },
                { "EN18", 3 },
                { "EN20", 4 },
                { "EN21", 6 },
                { "EN22", 5 },
                { "EN23", 8 },
                { "EN24", 3 },
                { "EN26", 7 },
                { "EN27", 6 },
                { "EN28", 3 },
                { "EN29", 7 },
                { "EN31", 9 },
                { "EN32", 3 },
                { "EN33", 10 },
                { "EN34", 9 },
                { "EV01", 8 },
                { "EV02", 7 },
                { "EV03", 3 },
                { "EV05", 8 },
                { "EV08", 1 },
                { "EV10", 1 },
                { "EV11", 1 },
                { "EV12", 3 },
                { "EV13", 3 },
                { "EV14", 6 },
                { "EV15", 4 },
                { "EV17", 6 },
                { "EV18", 8 },
                { "EV19", 10 },
                { "EV20", 8 },
                { "EV21", 4 },
                { "EV22", 8 },
                { "EV23", 9 },
                { "EV24", 7 },
                { "EV26", 8 },
                { "EV27", 9 },
                { "EV29", 9 },
                { "EN201", 11 },
                { "EN202", 11 },
                { "EN203", 11 }
			};
        }

        public void Reset()
        {
            newEnemies = null;
        }

        public void Shuffle(int seed)
        {
            if (Main.Randomizer.gameConfig.enemies.type < 1 || !Main.Randomizer.data.isValid)
                return;

            newEnemies = new Dictionary<string, string>();
            filler.Fill(seed, Main.Randomizer.gameConfig.enemies, newEnemies);
            Main.Randomizer.Log(newEnemies.Count + " enemies have been shuffled!");
        }

        public string GetSpoiler()
        {
            return "";
            //if (Main.Randomizer.gameConfig.enemies.type < 1)
            //    return "";

            //string spoiler = "================\nEnemies\n================\n\n";
            //Dictionary<string, string> enemyNames = new Dictionary<string, string>();

            //// Ensure data is valid
            //if (!FileUtil.parseFileToDictionary("names_enemies.dat", enemyNames) || newEnemies == null)
            //{
            //    return spoiler + "Failed to generate enemy spoiler.\n\n";
            //}

            //foreach (string key in newEnemies.Keys)
            //{
            //    spoiler += $"{enemyNames[key]} --> {enemyNames[newEnemies[key]]}\n";
            //}
            //return spoiler + "\n";
        }

        // temp
        public static string enemyData = "";
    }
}
