using HarmonyLib;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Tools.Playmaker2.Condition;
using Framework.Managers;

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
        //[HarmonyPatch(typeof(ItemIsEquiped), "executeAction")]
        //public class ItemIsEquiped_Patch
        //{
        //    public static bool Prefix(string objectIdStting, ref bool __result)
        //    {
        //        if (objectIdStting == "RE04")
        //        {
        //            __result = true;
        //            return false;
        //        }
        //        return true;
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
