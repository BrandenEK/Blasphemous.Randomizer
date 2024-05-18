using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Environment.Elevator;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using Tools.Level.Actionables;
using HarmonyLib;
using Blasphemous.Randomizer.DoorRando;

namespace Blasphemous.Randomizer.Patches
{
    // Always set elevators to default positions with gates open
    [HarmonyPatch(typeof(EventManager), "GetCurrentPersistentState")]
    public class EventManagerSave_Patch
    {
        public static void Prefix(Dictionary<string, FlagObject> ___flags)
        {
            if (Main.Randomizer.GameSettings.DoorShuffleType <= 0) return;

            string[] elevators = ElevatorData.elevatorFlags;
            for (int i = 0; i < elevators.Length; i++)
            {
                if (___flags.ContainsKey(elevators[i]))
                    ___flags[elevators[i]].value = i == elevators.Length - 1 && !___flags["ELEVATOR_POSITION_FAKE"].value || i <= 1 && Core.LevelManager.currentLevel.LevelName == "D01Z02S03" && Core.Events.GetFlag("D01Z02S03_ELEVATOR");
            }

            // If entering from behind amanecida face, break it instantly
            DoorLocation doorToEnter = Main.Randomizer.itemShuffler.LastDoor;
            if (doorToEnter != null && doorToEnter.Id == "D08Z01S02[SE]")
                Core.Events.SetFlag("D08Z01S02_FACE_BROKEN", true);
        }
    }
    [HarmonyPatch(typeof(Gate), "GetCurrentPersistentState")]
    public class GateSave_Patch
    {
        public static void Prefix(Gate __instance, ref bool ___open)
        {
            if (Main.Randomizer.GameSettings.DoorShuffleType <= 0) return;

            string gateId = __instance.GetPersistenID();
            for (int i = 0; i < ElevatorData.elevatorGateIds.Length; i++)
            {
                if (Core.LevelManager.currentLevel.LevelName == ElevatorData.elevatorGateScenes[i] && gateId == ElevatorData.elevatorGateIds[i])
                    ___open = true;
            }
        }
    }
    [HarmonyPatch(typeof(StraightMovingPlatform), "SetCurrentPersistentState")]
    public class PlatformLoad_Patch
    {
        public static bool Prefix(StraightMovingPlatform __instance)
        {
            return Main.Randomizer.GameSettings.DoorShuffleType <= 0 || Core.LevelManager.currentLevel.LevelName != "D02Z02S11" || __instance.GetPersistenID() != "e78fa0c2-ba1f-40f4-97cc-394162f84d7c";
        }
    }
    [HarmonyPatch(typeof(Elevator), "GetDisplacementLapse")]
    public class ElevatorSpeed_Patch
    {
        public static void Postfix(ref float __result)
        {
            __result = 2f;
        }
    }

    // Data containing pers. ids and flag for the elevators & gates
    public static class ElevatorData
    {
        public static string[] elevatorFlags = new string[]
        {
            "D01Z02S03_ELEVATOR_IN_ALBERO",
            "D02Z05S01_ELEVATOR2_ON_BOTTOM",
            "D02Z05S01_ELEVATOR1_ON_BOTTOM",
            "D03Z05S01_ELEVATOR_ON_BOTTOM",
            "D04Z02S24_ELEVATOR_ON_BOTTOM",
            "D05Z04S01_ELEVATOR_ON_BOTTOM",
            "ELEVATOR_POSITION_1",
            "ELEVATOR_POSITION_2",
            "ELEVATOR_POSITION_3",
            "ELEVATOR_POSITION_4",
        };

        public static string[] elevatorGateIds = new string[]
        {
            "835afc15-8de9-47bf-9057-54a0f23d69c6",
            "6aa4b846-441a-4be6-8786-49a915b0df97",
            "6aa4b846-441a-4be6-8786-49a915b0df97",
            "17b487db-fb2c-4ac2-ace2-0017b67a7eda",
            "c1c68418-eb19-48f6-8bf6-fc979b8df329",
        };

        public static string[] elevatorGateScenes = new string[]
        {
            "D02Z02S11",
            "D01Z05S25",
            "D04Z02S24",
            "D05Z01S21",
            "D05Z01S21",
        };
    }
}
