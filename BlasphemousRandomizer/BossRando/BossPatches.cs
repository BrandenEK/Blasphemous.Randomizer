using UnityEngine;
using HarmonyLib;
using Gameplay.GameControllers.Effects.Player.Recolor;
using Framework.Managers;
using Framework.FrameworkCore;
using Tools.DataContainer;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Gameplay.UI.Widgets;
using System.Collections.Generic;

namespace BlasphemousRandomizer.BossRando
{
    // Return to real room after boss
    [HarmonyPatch(typeof(BossRushManager), "LoadHub")]
    public class BossRushManager_LeaveBoss_Patch
    {
        public static bool Prefix()
        {
            Main.Randomizer.bossShuffler.ReturnToGameWorld();
            return false;
        }
    }

    //[HarmonyPatch(typeof(SpawnManager), "Start")]
    //public class temp
    //{
    //    public static void Postfix(Dictionary<string, TeleportDestination> ___TeleportDict)
    //    {
    //        foreach (TeleportDestination tp in ___TeleportDict.Values)
    //        {
    //            Main.Randomizer.LogWarning($"{tp.id}: {tp.teleportName} ({tp.sceneName})");
    //        }
    //    }
    //}

    

    [HarmonyPatch(typeof(SpawnManager), "PrepareForBossRush")]
    public class SpawnManager_EnterBoss_Patch
    {
        public static bool Prefix(ref SpawnManager.PosibleSpawnPoints ___pendingSpawn, ref string ___spawnId)
        {
            ___pendingSpawn = SpawnManager.PosibleSpawnPoints.Teleport;
            ___spawnId = "BOSSRUSH02";
            return false;
        }
    }

    

    // First, the player enters a boss door and potentially starts the fight
    [HarmonyPatch(typeof(Door), "EnterDoor")]
    public class Door_EnterBoss_Patch
    {
        public static void Prefix(Door __instance)
        {
            switch (__instance.targetScene)
            {
                case "D17Z01S11": Main.Randomizer.bossShuffler.EnterBossFight("WS"); break;
                case "D01Z04S18": Main.Randomizer.bossShuffler.EnterBossFight("TP"); break;
            }
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
            //if (levelName == "MainMenu")
            //    return;
            //if (Core.LevelManager.currentLevel.LevelName.StartsWith("D22"))
            //{
            //    background = Color.white;
            //    return;
            //}

            //string newBossRoom = Main.Randomizer.bossShuffler.GetRandomBossRoom(levelName);
            //if (newBossRoom == null)
            //    return;
            string newBossRoom = Main.Randomizer.bossShuffler.EnterBossRoom;

            Main.Randomizer.LogWarning($"Loading boss room for {levelName}: {newBossRoom}");
            Core.SpawnManager.PrepareForBossRush();
            levelName = newBossRoom;
            useFade = true;
            background = Color.white;
        }
    }

    //// Set objects needed for boss shuffle
    //[HarmonyPatch(typeof(GuiltManager), "OnLevelLoaded")]
    //public class GuiltManager_Patch
    //{
    //    public static void Postfix(GuiltConfigData ___guiltConfig)
    //    {
    //        Main.Randomizer.bossShuffler.setBossActivator(___guiltConfig.dropPrefab);
    //    }
    //}
    //[HarmonyPatch(typeof(PrieDieu), "OnPlayerReady")]
    //public class PrieDieu_Patch
    //{
    //    public static void Postfix(PrieDieu __instance)
    //    {
    //        Main.Randomizer.bossShuffler.setKneelingAnimator(__instance.transform.GetChild(4).GetComponent<Animator>().runtimeAnimatorController, __instance.transform.GetChild(4).GetComponent<ColorPaletteSwapper>().extraMaterial);
    //    }
    //}

    // Spawn from custom position when returning from boss fight
    //[HarmonyPatch(typeof(SpawnManager), "SpawnFromCustom")]
    //public class SpawnManager_Patch
    //{
    //    public static void Prefix(ref string ___customLevel, ref Vector3 ___customPosition, ref EntityOrientation ___customOrientation, ref bool ___customIsMiriam)
    //    {
    //        if (Main.Randomizer.bossShuffler.bossStatus == BossShuffle.BossFightStatus.Returning)
    //        {
    //            ___customLevel = Main.Randomizer.bossShuffler.spawnScene;
    //            ___customPosition = Main.Randomizer.bossShuffler.spawnPosition;
    //            ___customOrientation = Main.Randomizer.bossShuffler.spawnDirection;
    //            ___customIsMiriam = false;
    //        }
    //    }
    //}

    // Change red guilt drops to instead start a boss fight
    //[HarmonyPatch(typeof(GuiltDropCollectibleItem), "InteractionStart")]
    //public class GuiltDropStart_Patch
    //{
    //    public static bool Prefix(GuiltDropCollectibleItem __instance)
    //    {
    //        // If the drop's ttw is 99 then its a special boss fight starter
    //        InteractableGuiltDrop drop = __instance.gameObject.GetComponent<InteractableGuiltDrop>();
    //        if (drop == null || drop.timeToWait != 99) return true;
    //        if (Main.Randomizer.bossShuffler.usedStart) return false; Main.Randomizer.bossShuffler.usedStart = true;

    //        if (Main.Randomizer.bossShuffler.bossStatus == BossShuffle.BossFightStatus.Returning)
    //        {
    //            // When returning, make guilt drop disappear
    //            return true;
    //        }
    //        else
    //        {
    //            // When interacting with guilt drop, disable input & kneel
    //            Main.Randomizer.bossShuffler.startPlayerKneeling();
    //            return false;
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(GuiltDropCollectibleItem), "InteractionEnd")]
    //public class GuiltDropEnd_Patch
    //{
    //    public static bool Prefix(GuiltDropCollectibleItem __instance)
    //    {
    //        // If the drop's ttw is 99 then its a special boss fight starter
    //        InteractableGuiltDrop drop = __instance.gameObject.GetComponent<InteractableGuiltDrop>();
    //        if (drop == null || drop.timeToWait != 99) return true;
    //        if (Main.Randomizer.bossShuffler.usedEnd) return false; Main.Randomizer.bossShuffler.usedEnd = true;

    //        if (Main.Randomizer.bossShuffler.bossStatus == BossShuffle.BossFightStatus.Returning)
    //        {
    //            // When returning, make guilt drop disappear
    //            Main.Randomizer.bossShuffler.bossStatus = BossShuffle.BossFightStatus.Nothing;
    //            __instance.transform.GetChild(0).gameObject.SetActive(false);
    //            __instance.transform.GetChild(2).gameObject.SetActive(false);
    //            return false;
    //        }
    //        else
    //        {
    //            // When interacting with guilt drop, load boss room
    //            Main.Randomizer.bossShuffler.loadBossRoom();
    //            return false;
    //        }
    //    }
    //}
    //[HarmonyPatch(typeof(InteractableGuiltDrop), "OnUsePost")]
    //public class GuiltDrop_Patch
    //{
    //    public static bool Prefix(InteractableGuiltDrop __instance)
    //    {
    //        return __instance.timeToWait != 99f;
    //    }
    //}

}
