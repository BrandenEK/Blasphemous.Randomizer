using System.Collections.Generic;
using HarmonyLib;
using Framework.Managers;
using Tools.Level.Interactables;

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
}
