using HarmonyLib;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Tools.Playmaker2.Condition;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore.Attributes;
using UnityEngine;

namespace BlasphemousRandomizer.Patches
{
    class World
    {
        // Always allow teleportation if enabled in config
        [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
        public class AlmsManager_Patch
        {
            public static bool Prefix(ref int __result)
            {
                if (Main.Randomizer.gameConfig.general.teleportationAlwaysUnlocked)
                {
                    __result = 3;
                    return false;
                }
                return true;
            }
        }

        // Replace enemy with random one
        [HarmonyPatch(typeof(EnemySpawnPoint), "Start")]
        public class EnemySpawnPoint_Patch
        {
            public static void Postfix(ref GameObject ___selectedEnemy)
            {
                string id = ___selectedEnemy.GetComponentInChildren<Enemy>().Id;
                if (Main.Randomizer.gameConfig.enemies.type < 1 || (Core.LevelManager.currentLevel.LevelName == "D03Z01S02" && id == "EN03"))
                    return;

                GameObject newEnemy = Main.Randomizer.enemyShuffler.getEnemy(id);
                if (newEnemy != null)
                    ___selectedEnemy = newEnemy;

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
        }

        // Always allow hint corpses to be active
        //[HarmonyPatch(typeof(ItemIsEquiped), "executeAction")]
        //public class ItemIsEquiped_Patch
        //{
        //    public static bool Prefix(string objectIdStting, ref bool __result)
        //    {
        //        if (objectIdStting == "RE04")
        //        {
        //            __result = true;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        // Disable holy visage altars
        //[HarmonyPatch(typeof(Interactable), "OnAwake")]
        //public class Interactable_Patch
        //{
        //    public static void Prefix(Interactable __instance)
        //    {
        //        string id = __instance.GetPersistenID();
        //        if (id == "22c0f081-b3a0-4310-8a40-9506d4a1315c" || id == "27213fd3-b05b-4157-b067-5206321cacb7" || id == "bc2b17e1-5c8c-4a90-b7c8-160eacdd538d")
        //        {
        //            __instance.gameObject.SetActive(false);
        //        }
        //    }
        //}

        //[HarmonyPatch(typeof(PrieDieu), "KneeledMenuCoroutine")]
        //public class PrieDieu_Patch
        //{
        //    public static void Prefix(ref bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log("Setting use teleport to true");
        //        canUseTeleport = true;
        //    }

        //    public static void Postfix(bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log(canUseTeleport.ToString());
        //    }
        //}
    }
}
