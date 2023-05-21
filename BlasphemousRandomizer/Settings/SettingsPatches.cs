using System.Collections.Generic;
using HarmonyLib;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Widgets;

namespace BlasphemousRandomizer.Settings
{
    // Allow visible cursor for settings menu
    [HarmonyPatch(typeof(DebugInformation), "Update")]
    public class DebugInformation_Patch
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    // Show settings menu when starting a new game
    [HarmonyPatch(typeof(SelectSaveSlots), "OnAcceptSlots")]
    public class SelectSaveSlotsMenu_Patch
    {
        public static bool Prefix(ref int idxSlot, List<SaveSlot> ___slots)
        {
            if (idxSlot >= 999) // Load new game
            {
                idxSlot -= 999;
                return true;
            }
            if (___slots[idxSlot].IsEmpty) // Show settings menu
            {
                Main.Randomizer.SettingsMenu.openMenu(idxSlot);
                return false;
            }

            return true;
        }
    }
}
