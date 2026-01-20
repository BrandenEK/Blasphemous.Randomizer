using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Input;
using HarmonyLib;
using Framework.Managers;
using Framework.EditorScripts.EnemiesBalance;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.InputSystem;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using Tools.Level.Interactables;
using Tools.Level.Actionables;

namespace Blasphemous.Randomizer.Patches
{
    // Always allow teleportation if enabled in config
    [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
    class AlmsManager_Patch
    {
        public static bool Prefix(ref int __result)
        {
            __result = 3;
            return false;
        }
    }

    // Make enemies stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentEnemiesBalanceChart")]
    class GameModeEnemies_Patch
    {
        public static bool Prefix(ref EnemiesBalanceChart __result, EnemiesBalanceChart ___newGameEnemiesBalanceChart)
        {
            __result = ___newGameEnemiesBalanceChart;
            return false;
        }
    }

    // Make bosses stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentBossesBalanceChart")]
    class GameModeBosses_Patch
    {
        public static bool Prefix(ref BossesBalanceChart __result, BossesBalanceChart ___newGameBossesBalanceChart)
        {
            __result = ___newGameBossesBalanceChart;
            return false;
        }
    }

    // Log what flags are being set & track certain ones
    [HarmonyPatch(typeof(EventManager), "SetFlag")]
    class EventManagerSet_Patch
    {
        public static void Prefix(EventManager __instance, string id, bool b)
        {
            string formatted = __instance.GetFormattedId(id);
            if (formatted == "" || formatted == "REVEAL_FAITH_PLATFORMS")
                return;

            string text = b ? "Setting" : "Clearing";
            ModLog.Info(text + " flag: " + formatted);

            // Special autotracker & map collection functionality when setting an item/location flag
            if (!b || __instance.GetFlag(id)) return;

            // Increase zone counter when location flag or special flag is set (Cant use formatted because location ids have lowercase)
            foreach (ItemRando.ItemLocation location in Main.Randomizer.data.itemLocations.Values)
            {
                if (location.LocationFlag == null && id == "LOCATION_" + location.Id || location.LocationFlag != null && location.LocationFlag.StartsWith(id))
                {
                    Main.Randomizer.MapCollection.CollectLocation(location.Id, Main.Randomizer.GameSettings);
                }
            }
        }
    }

    // Don't allow playing in sacred sorrows mode
    [HarmonyPatch(typeof(BossRushWidget), "OptionPressed")]
    class BossRushWidget_Patch
    {
        public static bool Prefix()
        {
            ModLog.Display(Main.Randomizer.LocalizationHandler.Localize("sorrow"));
            return false;
        }
    }

    // Load custom starting location
    [HarmonyPatch(typeof(NewMainMenu), "InternalPlay")]
    class MainMenuNewGame_Patch
    {
        public static void Prefix(bool ___isContinue, ref string ___sceneName)
        {
            if (!___isContinue)
            {
                // Must load config settings from menu here before OnNewGame is called
                // Also used in DoorPatches
                Main.Randomizer.GameSettings = Main.Randomizer.ModMenu.MenuSettings;
                ___sceneName = Main.Randomizer.GameSettings.RealStartingLocation.Room;
            }
        }
    }

    // Always think that no sword hearts are equipped
    [HarmonyPatch(typeof(InventoryManager), "IsAnySwordHeartEquiped")]
    class InventorySwordHeart_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }

    // Insta open WotHP gate
    [HarmonyPatch(typeof(Lever), "InteractionEnd")]
    class Lever_Patch
    {
        public static void Postfix()
        {
            //"5add5f9e-c290-4c0c-9a93-3728584f015b && 213379f6-1168-4c0a-bc99-e6118bce5a48"
            if (Core.LevelManager.currentLevel.LevelName != "D09Z01S02") return;

            Gate[] gates = Object.FindObjectsOfType<Gate>();
            foreach (Gate gate in gates)
            {
                if (gate.GetPersistenID() == "debd2dd2-4061-4b29-9ea2-8db07e884f5d")
                {
                    if (!gate.IsOpenOrActivated())
                    {
                        gate.Use();
                        Core.Events.SetFlag("D09Z01S02_GATERIDDLE", true);
                    }
                    break;
                }
            }
        }
    }

    // Prevent dash unless the item is owned
    [HarmonyPatch(typeof(Rewired.Player), "GetButton", typeof(int))]
    class RewiredButton_Patch
    {
        public static bool Prefix(int actionId)
        {
            return actionId != 7 || Main.Randomizer.CanDash || Main.Randomizer.DashChecker;
        }
    }
    [HarmonyPatch(typeof(Rewired.Player), "GetButtonDown", typeof(int))]
    class RewiredButtonDown_Patch
    {
        public static bool Prefix(int actionId)
        {
            return actionId != 7 || Main.Randomizer.CanDash || Main.Randomizer.DashChecker;
        }
    }
    [HarmonyPatch(typeof(PlatformCharacterInput), "IsDashButtonHold", MethodType.Getter)]
    class PlatformInputDash_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            Main.Randomizer.DashChecker = true;
            __result = Main.Randomizer.InputHandler.GetButton(ButtonCode.Dash);
            Main.Randomizer.DashChecker = false;
            return false;
        }
    }

    // Prevent wall climb unless the item is owned
    [HarmonyPatch(typeof(WallJump), "EndStickCoolDown", MethodType.Getter)]
    class WallJump_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            bool wallClimb = Main.Randomizer.CanWallClimb;
            __result = wallClimb;
            return wallClimb;
        }
    }
}
