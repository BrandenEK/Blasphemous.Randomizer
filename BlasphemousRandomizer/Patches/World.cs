using HarmonyLib;
using Tools.Level;
using Tools.Level.Interactables;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using Framework.Managers;

namespace BlasphemousRandomizer.Patches
{
    class World
    {
        // Always allow teleportation if enabled in config
        [HarmonyPatch(typeof(PrieDieu), "HaveAnySwordHearts")]
        public class PrieDieu_Patch
        {
            public static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }
        [HarmonyPatch(typeof(UIController), "ShowKneelMenu")]
        public class UIController_Patch
        {
            public static void Prefix(ref KneelPopUpWidget.Modes mode)
            {
                mode = KneelPopUpWidget.Modes.PrieDieu_all;
            }
        }

        // Disable holy visage altars
        [HarmonyPatch(typeof(Interactable), "OnAwake")]
        public class Interactable_Patch
        {
            public static void Prefix(Interactable __instance)
            {
                string id = __instance.GetPersistenID();
                if (id == "22c0f081-b3a0-4310-8a40-9506d4a1315c" || id == "27213fd3-b05b-4157-b067-5206321cacb7" || id == "bc2b17e1-5c8c-4a90-b7c8-160eacdd538d")
                {
                    __instance.gameObject.SetActive(false);
                }
            }
        }
    }
}
