using HarmonyLib;
using Tools.Level;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Tools.Playmaker2.Condition;
using Framework.Inventory;
using Framework.Managers;
using UnityEngine;

namespace BlasphemousRandomizer.Patches
{
    class World
    {
        // Always allow teleportation if enabled in config
        [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
        public class AlmsManager_Patch
        {
            public static bool Prefix(ref int __result)
            {
                if (Main.Randomizer.gameConfig.general.teleportationAlwaysUnlocked)
                {
                    __result = 3;
                    return false;
                }
                return true;
            }
        }

        // Always allow hint corpses to be active
        [HarmonyPatch(typeof(ItemIsEquiped), "executeAction")]
        public class ItemIsEquiped_Patch
        {
            public static bool Prefix(string objectIdStting, ItemIsEquiped __instance, ref bool __result)
            {
                if (objectIdStting == "RE04" && __instance.Owner.name != "LeftPuzzleCheck" && __instance.Owner.name != "RightPuzzleCheck")
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Interactable), "HasRequiredItem")]
        public class Interactable_Patch
        {
            public static bool Prefix(ref bool __result, InventoryObjectInspector ___requiredItem)
            {
                if (___requiredItem.id == "RE04")
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        // Open WaHP gate faster
        //[HarmonyPatch(typeof(Lever), "Start")]
        //public class Lever_Patch
        //{
        //    public static void Postfix(LeverAction ___action, GameObject[] ___target)
        //    {
        //        string output = "";
        //        output += "Action: " + ___action.ToString() + "\n";
        //        output += "Targets: " + ___target.Length + "\n";
        //        foreach (GameObject obj in ___target)
        //        {
        //            output += "Components: \n";
        //            foreach (Component c in obj.GetComponents<Component>())
        //            {
        //                output += c.ToString() + "\n";
        //            }
        //        }
        //        Main.Randomizer.Log(output);
        //    }
        //}

        //[HarmonyPatch(typeof(PrieDieu), "KneeledMenuCoroutine")]
        //public class PrieDieu_Patch
        //{
        //    public static void Prefix(ref bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log("Setting use teleport to true");
        //        canUseTeleport = true;
        //    }

        //    public static void Postfix(bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log(canUseTeleport.ToString());
        //    }
        //}
    }
}
