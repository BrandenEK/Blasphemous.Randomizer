using HarmonyLib;
using Framework.Managers;
using Framework.EditorScripts.EnemiesBalance;
using Framework.EditorScripts.BossesBalance;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Widgets;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BlasphemousRandomizer
{
    // Always allow teleportation if enabled in config
    [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
    public class AlmsManager_Patch
    {
        public static bool Prefix(ref int __result)
        {
            if (Main.Randomizer.gameConfig.UnlockTeleportation)
            {
                __result = 3;
                return false;
            }
            return true;
        }
    }

    // Make enemies stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentEnemiesBalanceChart")]
    public class GameModeEnemies_Patch
    {
        public static bool Prefix(ref EnemiesBalanceChart __result, EnemiesBalanceChart ___newGameEnemiesBalanceChart)
        {
            __result = ___newGameEnemiesBalanceChart;
            return false;
        }
    }

    // Make bosses stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentBossesBalanceChart")]
    public class GameModeBosses_Patch
    {
        public static bool Prefix(ref BossesBalanceChart __result, BossesBalanceChart ___newGameBossesBalanceChart)
        {
            __result = ___newGameBossesBalanceChart;
            return false;
        }
    }

    // Call load data even if loading vanilla game
    [HarmonyPatch(typeof(PersistentManager), "LoadSnapShot")]
    public class PersistentManagerLoad_Patch
    {
        public static void Postfix(PersistentManager __instance, PersistentManager.SnapShot snapShot)
        {
            if (!snapShot.commonElements.ContainsKey(Main.Randomizer.PersistentID))
            {
                Main.Randomizer.LoadGame(null);
            }
        }
    }

    // Log what flags are being set & track certain ones
    [HarmonyPatch(typeof(EventManager), "SetFlag")]
    public class EventManagerSet_Patch
    {
        public static void Prefix(EventManager __instance, string id, bool b)
        {
            string formatted = __instance.GetFormattedId(id);
            if (formatted == "" || formatted == "REVEAL_FAITH_PLATFORMS")
                return;

            string text = b ? "Setting" : "Clearing";
            Main.Randomizer.Log(text + " flag: " + formatted);

            // Autotracking flags
            if (formatted.StartsWith("ITEM_"))
                Main.Randomizer.tracker.NewItem(id.Substring(5));
            else if (formatted.StartsWith("LOCATION_"))
                Main.Randomizer.tracker.NewLocation(id.Substring(9));
            else if (Main.arrayContains(Main.Randomizer.tracker.SpecialLocations, formatted))
            {
                Main.Randomizer.tracker.NewItem(formatted);
                Main.Randomizer.tracker.NewLocation(formatted);
            }
        }
    }

    // Show validity of save slot on select screen
    [HarmonyPatch(typeof(SelectSaveSlots), "SetAllData")]
    public class SelectSaveSlotsData_Patch
    {
        public static void Postfix(List<SaveSlot> ___slots)
        {
            for (int i = 0; i < ___slots.Count; i++)
            {
                PersistentManager.PublicSlotData slotData = Core.Persistence.GetSlotData(i);
                if (slotData == null)
                    continue;

                // Check if this save file was played in supported version
                string majorVersion = Main.Randomizer.ModVersion;
                majorVersion = majorVersion.Substring(0, majorVersion.LastIndexOf('.'));

                string type = $"({Main.Randomizer.Localize("vandis")})";
                if (slotData.flags.flags.ContainsKey(majorVersion))
                    type = $"({Main.Randomizer.Localize("randis")})";
                else if (slotData.flags.flags.ContainsKey("RANDOMIZED"))
                    type = $"({Main.Randomizer.Localize("outdis")})";

                // Send extra info to the slot
                ___slots[i].SetData("ignore", type, 0, false, false, false, 0, SelectSaveSlots.SlotsModes.Normal);
            }
        }
    }
    [HarmonyPatch(typeof(SaveSlot), "SetData")]
    public class SaveSlotData_Patch
    {
        public static bool Prefix(string zoneName, string info, ref Text ___ZoneText, ref bool canConvert)
        {
            canConvert = false;
            if (zoneName == "ignore")
            {
                ___ZoneText.text += "   " + info;
                return false;
            }
            return true;
        }
    }

    // Don't allow playing in sacred sorrows mode
    [HarmonyPatch(typeof(BossRushWidget), "OptionPressed")]
    public class BossRushWidget_Patch
    {
        public static bool Prefix()
        {
            Main.Randomizer.LogDisplay(Main.Randomizer.Localize("sorrow"));
            return false;
        }
    }
}
