using HarmonyLib;
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
            if (currentScene == "D03Z01S03")
            {
                // Special id calculation for the linen drops over Jondo
                if (__instance.targetDoor == "CherubsL")
                    currentId = "-WestR";
                else if (__instance.targetDoor == "CherubsR")
                    currentId = "-EastL";
                else if (__instance.targetDoor == "Cherubs")
                {
                    if (__instance.targetScene == "D03Z02S10")
                        currentId = "-WestL";
                    else if (__instance.targetScene == "D03Z02S01")
                        currentId = "-EastR";
                }
            }

            string doorId = $"{currentScene}[{currentId}]";
            Main.Randomizer.LogWarning("Entering door: " + doorId);
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
        public static void Postfix(ref SpawnManager.PosibleSpawnPoints ___pendingSpawn, ref string ___customLevel, ref Vector3 ___customPosition, ref EntityOrientation ___customOrientation)
        {
            ___pendingSpawn = SpawnManager.PosibleSpawnPoints.CustomPosition;
            StartingLocation start = Main.Randomizer.StartingDoor;
            ___customLevel = start.Room;
            ___customPosition = start.Position;
            ___customOrientation = start.FacingRight ? EntityOrientation.Right : EntityOrientation.Left;
            Main.Randomizer.Log("Setting start location to " + start.Room);
        }
    }

    // Prevent crashing when loading same scene from door
    [HarmonyPatch(typeof(SpawnManager), "SpawnPlayer")]
    public class SpawnManagerSpawn_Patch
    {
        public static void Prefix(SpawnManager.PosibleSpawnPoints spawnType, ref bool forceLoad)
        {
            if (Main.Randomizer.gameConfig.DoorShuffleType > 0 && spawnType == SpawnManager.PosibleSpawnPoints.Door)
                forceLoad = true;
        }
    }
}
