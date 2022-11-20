using HarmonyLib;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
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
