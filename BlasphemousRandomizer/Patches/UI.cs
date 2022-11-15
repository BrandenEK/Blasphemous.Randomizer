using HarmonyLib;
using UnityEngine.UI;
using Tools.Playmaker2.Action;
using Gameplay.UI.Others.MenuLogic;

namespace BlasphemousRandomizer.Patches
{
    // Change version number on main menu to randomizer version
    [HarmonyPatch(typeof(VersionNumber), "Start")]
    public class VersionNumberPatch
    {
        public static bool Prefix(VersionNumber __instance)
        {
            __instance.GetComponent<Text>().text = "Randomizer v" + MyPluginInfo.PLUGIN_VERSION;
            return false;
        }
    }

    // Skip showing certain messages to the player
    [HarmonyPatch(typeof(ShowMessage), "OnEnter")]
    public class ShowMessage_Patch
    {
        public static bool Prefix(ShowMessage __instance)
        {
            if (__instance.textId != null && "MSG_0501,MSG_BAPTISMAL,MSG_1801,MSG_2101,MSG_10101".Contains(__instance.textId.Value))
            {
                __instance.Finish();
                return false;
            }
            return true;
        }
    }

    // Can be used to close dialog
    [HarmonyPatch(typeof(PopUpWidget), "ShowAreaPopUp")]
    public class PopUpWidget_Patch
    {
        public static bool Prefix(string area, PopUpWidget __instance)
        {
            if (area == "closeDialog")
            {
                //PopUpWidget.OnDialogClose();
            }
            return true;
        }
    }
}
