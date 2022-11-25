using HarmonyLib;
using Tools.Level.Layout;
using Tools.Level.Utils;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Enemies.WallEnemy;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy;
using UnityEngine;

namespace BlasphemousRandomizer.Patches
{
    // Replace enemy with random one
    [HarmonyPatch(typeof(EnemySpawnPoint), "Awake")]
    public class EnemySpawnPoint_Patch
    {
        public static void Postfix(ref GameObject ___selectedEnemy, Transform ___spawnPoint)
        {
            // Temporary logging & data collection
            Enemy enemy = ___selectedEnemy.GetComponentInChildren<Enemy>();
            if (enemy == null)
            {
                Main.Randomizer.Log("Enemy didn't have enemy component!");
                return;
            }

            string locationId = $"{Core.LevelManager.currentLevel.LevelName}[{(int)___spawnPoint.position.x},{(int)___spawnPoint.position.y}]";
            GameObject newEnemy = Main.Randomizer.enemyShuffler.getEnemy(locationId);
            if (newEnemy != null)
                ___selectedEnemy = newEnemy;

            //Main.Randomizer.Log($"Id: " + locationId);
            //Main.Randomizer.Log("Original enemy: " + enemy.Id);

            string output = "{\r\n\t\"locationId\": \"";
            output += locationId;
            output += "\",\r\n\t\"originalEnemy\": \"";
            output += enemy.Id;
            output += "\"\r\n},\r\n";
            Shufflers.EnemyShuffle.enemyData += output;

            // Can modify spawn position here too
        }
    }

    // Scale enemy stats based on area
    [HarmonyPatch(typeof(EnemyStatsImporter), "SetEnemyStats")]
    public class EnemyStatsImporter_Patch
    {
        public static void Postfix(Enemy enemy)
        {
            if (enemy == null || enemy.Id == "" || enemy.Id == "EV09")
                return;

            // Get ratings of enemy & area
            string scene = Core.LevelManager.currentLevel.LevelName;
            if (scene.Substring(0, 6) != "D19Z01")
            {
                scene = scene.Substring(0, 6);
            }
            int areaRating = Main.Randomizer.enemyShuffler.getRating(scene);
            int enemyRating = Main.Randomizer.enemyShuffler.getRating(enemy.Id);
            float percent = 0.07f;

            // If areaScaling is enabled, calculate percent scaling
            if (areaRating != 0 && enemyRating != 0 && Main.Randomizer.gameConfig.enemies.type > 0 && Main.Randomizer.gameConfig.enemies.areaScaling)
            {
                percent *= areaRating - enemyRating;
            }

            // Change stats
            enemy.Stats.Strength = new Strength(enemy.Stats.Strength.Base + enemy.Stats.Strength.Base * percent, enemy.Stats.StrengthUpgrade, 1f);
            enemy.Stats.Life = new Life(enemy.Stats.Life.Base + enemy.Stats.Life.Base * percent, enemy.Stats.LifeUpgrade, enemy.Stats.LifeMaximun, 1f);
            enemy.purgePointsWhenDead += enemy.purgePointsWhenDead * percent;
            EnemyAttack attack = enemy.GetComponentInChildren<EnemyAttack>();
            if (attack != null)
                attack.ContactDamageAmount += attack.ContactDamageAmount * percent;
        }

        // Prevent wall enemies from disabling climbable walls
        [HarmonyPatch(typeof(WallEnemy), "OnTriggerEnter2D")]
        public class WallEnemy_Patch
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        // Don't hard cast flying patrolling enemies
        [HarmonyPatch(typeof(FlyingPatrollingEnemySpawnConfigurator), "OnSpawn")]
        public class FlyingPatrollingEnemySpawnConfigurator_Patch
        {
            public static bool Prefix(Enemy e)
            {
                return e.GetType() == typeof(PatrollingFlyingEnemy);
            }
        }
    }
}
