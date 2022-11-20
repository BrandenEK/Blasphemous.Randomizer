using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;
using Framework.Managers;
using UnityEngine;

namespace BlasphemousRandomizer.Shufflers
{
    public class EnemyShuffle : IShuffle
    {
        private Dictionary<string, string> newEnemies;
        private string[] bannedScenes;
        private EnemyFiller filler;

        public GameObject getEnemy(string id)
        {
            if (EnemyLoader.loaded && newEnemies != null && newEnemies.ContainsKey(id) && !FileUtil.arrayContains(bannedScenes, Core.LevelManager.currentLevel.LevelName))
            {
                return EnemyLoader.getEnemy(newEnemies[id]);
            }
            return null;
        }

        public void Init()
        {
            filler = new EnemyFiller();
            bannedScenes = new string[]
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
        }

        public void Reset()
        {
            newEnemies = null;
        }

        public void Shuffle(int seed)
        {
            if (Main.Randomizer.gameConfig.enemies.type < 1)
                return;
            if (!filler.isValid())
            {
                Main.Randomizer.Log("Error: Enemy data could not be loaded!");
                return;
            }

            newEnemies = new Dictionary<string, string>();
            filler.Fill(seed, Main.Randomizer.gameConfig.enemies, newEnemies);
            Main.Randomizer.Log(newEnemies.Count + " enemies have been shuffled!");
        }

        public string GetSpoiler()
        {
            if (Main.Randomizer.gameConfig.enemies.type < 1)
                return "";

            string spoiler = "================\nEnemies\n================\n\n";
            Dictionary<string, string> enemyNames = new Dictionary<string, string>();

            // Ensure data is valid
            if (!FileUtil.parseFileToDictionary("names_enemies.dat", enemyNames) || newEnemies == null)
            {
                return spoiler + "Failed to generate enemy spoiler.\n\n";
            }

            foreach (string key in newEnemies.Keys)
            {
                spoiler += $"{enemyNames[key]} --> {enemyNames[newEnemies[key]]}\n";
            }
            return spoiler + "\n";
        }
    }
}
