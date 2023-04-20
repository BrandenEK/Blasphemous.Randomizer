﻿using HarmonyLib;
using Framework.Managers;
using Framework.FrameworkCore;
using Tools.Level.Interactables;
using UnityEngine;

namespace BlasphemousRandomizer.DoorRando
{
    // Change target door for door shuffle
    [HarmonyPatch(typeof(Door), "OnDoorActivated")]
    public class Door_Patch
    {
        public static void Prefix(ref Door __instance)
        {
            string currentScene = Core.LevelManager.currentLevel.LevelName;
            string currentId = __instance.identificativeName == "-" ? "-" + __instance.targetDoor : __instance.identificativeName;
            string doorId = $"{currentScene}[{currentId}]";
            if (Main.Randomizer.gameConfig.DoorShuffleType > 0 && Main.Randomizer.itemShuffler.getNewDoor(doorId, out string newScene, out string newId))
            {
                __instance.targetScene = newScene;
                __instance.targetDoor = newId;
            }
        }
    }

    // When beginning new game, set custom spawning properties
    [HarmonyPatch(typeof(SpawnManager), "ResetPersistence")]
    public class SpawnManagerStart_Patch
    {
        public static void Postfix(ref SpawnManager.PosibleSpawnPoints ___pendingSpawn, ref Vector3 ___customPosition, ref EntityOrientation ___customOrientation)
        {
            ___pendingSpawn = SpawnManager.PosibleSpawnPoints.CustomPosition;
            StartingLocation start = Main.Randomizer.StartingDoor;
            ___customPosition = start.Position;
            ___customOrientation = start.FacingRight ? EntityOrientation.Right : EntityOrientation.Left;
            Main.Randomizer.Log("Setting start location to " + start.Room);
        }
    }
}