using HarmonyLib;
using Framework.Managers;
using Framework.FrameworkCore;
using Tools.Level.Interactables;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;
using Blasphemous.Randomizer.DoorRando;

namespace Blasphemous.Randomizer.Patches
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
            DoorLocation targetDoor = Main.Randomizer.itemShuffler.GetTargetDoor(doorId);
            Main.Randomizer.Log("Entering door: " + doorId);
            if (targetDoor != null)
            {
                __instance.targetScene = targetDoor.Room;
                __instance.targetDoor = targetDoor.IdentityName;
            }

            if (__instance.targetScene == "D03Z03S15" && !Core.Events.GetFlag("D03Z04S01_BOSSDEAD"))
            {
                // If Anguish hasnt been killed yet, load boss fight room instead
                __instance.targetScene = "D03BZ01S01";
                __instance.targetDoor = "W";
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
            StartingLocation start = Main.Randomizer.GameSettings.RealStartingLocation;
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
            if (Main.Randomizer.GameSettings.DoorShuffleType > 0 && spawnType == SpawnManager.PosibleSpawnPoints.Door)
                forceLoad = true;
        }
    }

    // Enable wall collider on certain boss fight starts
    [HarmonyPatch(typeof(BossFightManager), "StartBossFight", typeof(bool))]
    public class BossFightStart_Patch
    {
        public static void Postfix()
        {
            string currentScene = Core.LevelManager.currentLevel.LevelName;
            if (currentScene == "D17Z01S11" || currentScene == "D05Z02S14")
                Main.Randomizer.BossBoundaryStatus = true;
        }
    }

    // Disable wall collider on certain boss fight ends
    [HarmonyPatch(typeof(BossFightManager), "OnDeathBoss")]
    public class BossFightEnd_Patch
    {
        public static void Postfix()
        {
            string currentScene = Core.LevelManager.currentLevel.LevelName;
            if (currentScene == "D17Z01S11" || currentScene == "D01Z04S18" || currentScene == "D02Z03S20")
                Main.Randomizer.BossBoundaryStatus = false;
        }
    }

    // Make fervour penance free
    [HarmonyPatch(typeof(FervourPenance), "GetPurgeCost")]
    public class FervourPenanceCost_Patch
    {
        public static bool Prefix(ref float __result)
        {
            __result = -1;
            return false;
        }
    }
    [HarmonyPatch(typeof(FervourPenance), "DrainResources")]
    public class FervourPenanceDrain_Patch
    {
        public static void Postfix(FervourPenance __instance)
        {
            __instance.EntityOwner.Stats.Purge.Current -= 1;
        }
    }
}
