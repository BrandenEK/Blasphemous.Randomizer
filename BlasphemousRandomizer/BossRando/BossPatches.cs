using Framework.Managers;
using Gameplay.UI.Widgets;
using HarmonyLib;
using Tools.Level.Interactables;
using UnityEngine;

namespace BlasphemousRandomizer.BossRando
{
    // First, the player enters a boss door and potentially starts the fight
    [HarmonyPatch(typeof(Door), "EnterDoor")]
    public class Door_EnterBoss_Patch
    {
        public static void Prefix(Door __instance)
        {
            string bossId;
            switch (__instance.targetScene)
            {
                case "D17Z01S11": bossId = "WS"; break;
                case "D01Z04S18": bossId = "TP"; break;
                case "D02Z03S20": bossId = "CL"; break;
                default: return;
            }

            Main.Randomizer.bossShuffler.EnterBossFight(bossId, __instance.targetScene, __instance.targetDoor);
        }
    }

    // Next, the fade out will change to white
    [HarmonyPatch(typeof(FadeWidget), "FadeAfterDelay")]
    public class FadeWidget_EnterBoss_Patch
    {
        public static void Prefix(ref float duration, ref Color target)
        {
            if (Main.Randomizer.bossShuffler.InBossFight)
            {
                duration = 0.5f;
                target = Color.white;
            }
        }
    }

    // After fade, replace the level to load with the new one
    [HarmonyPatch(typeof(LevelManager), "ChangeLevel")]
    public class LevelManager_EnterBoss_Patch
    {
        public static void Prefix(ref string levelName, ref bool useFade, ref Color background)
        {
            if (!Main.Randomizer.bossShuffler.InBossFight)
                return;

            string newBossRoom = Main.Randomizer.bossShuffler.CurrentBossFight.BossRoomSceneId;

            Main.Randomizer.LogWarning($"Loading boss room for {levelName}: {newBossRoom}");
            Core.SpawnManager.PrepareForBossRush();
            levelName = newBossRoom;
            useFade = true;
            background = Color.white;
        }
    }

    // Also set the spawn type and id for the new boss room
    [HarmonyPatch(typeof(SpawnManager), "PrepareForBossRush")]
    public class SpawnManager_EnterBoss_Patch
    {
        public static bool Prefix(ref SpawnManager.PosibleSpawnPoints ___pendingSpawn, ref string ___spawnId)
        {
            ___pendingSpawn = SpawnManager.PosibleSpawnPoints.Teleport;
            ___spawnId = Main.Randomizer.bossShuffler.CurrentBossFight.BossRoomTeleportId;
            return false;
        }
    }

    // After defeating the boss, return to real game
    [HarmonyPatch(typeof(BossRushManager), "LoadHub")]
    public class BossRushManager_LeaveBoss_Patch
    {
        public static bool Prefix()
        {
            Main.Randomizer.bossShuffler.LeaveBossFight();
            return false;
        }
    }
}
