using HarmonyLib;
using Framework.Managers;

namespace BlasphemousRandomizer.ItemRando
{
    // Only have laudes activated in boss room with verse
    [HarmonyPatch(typeof(EventManager), "GetFlag")]
    public class EventManagerGet_Patch
    {
        public static void Postfix(string id, EventManager __instance, ref bool __result)
        {
            string formatted = __instance.GetFormattedId(id);
            if (formatted == "SANTOS_LAUDES_ACTIVATED")
            {
                string scene = Core.LevelManager.currentLevel.LevelName;
                __result = (scene == "D08Z03S01" || scene == "D08Z03S03") && __instance.GetFlag("ITEM_QI110");
            }
        }
    }

    // Set flag for what miriam portal has been activated
    [HarmonyPatch(typeof(EventManager), "EndMiriamPortalAndReturn")]
    public class EventManagerMiriam_Patch
    {
        public static void Prefix(EventManager __instance)
        {
            if (__instance.AreInMiriamLevel())
            {
                __instance.SetFlag("RMIRIAM_" + __instance.MiriamCurrentScenePortal, true);
            }
        }
    }

    // Change functionality of the Ossuary
    [HarmonyPatch(typeof(OssuaryManager), "CheckGroupCompletion")]
    public class OssuaryManager_Patch
    {
        public static bool Prefix(OssuaryManager __instance)
        {
            __instance.pendingRewards = 0;
            int collected = OssuaryManager.CountAlreadyRetrievedCollectibles();
            int alreadyClaimed = 0;

            for (int i = 0; i < 11; i++)
            {
                string id = "OSSUARY_REWARD_" + (i + 1);
                if (Core.Events.GetFlag(id))
                {
                    alreadyClaimed++;
                }
                else if (collected >= (i + 1) * 4)
                {
                    Core.Events.SetFlag(id, true, false);
                    __instance.pendingRewards++;
                }
            }
            __instance.alreadyClaimedRewards = alreadyClaimed;
            Core.Events.LaunchEvent(__instance.CheckRewardsEvent, string.Empty);
            return false;
        }
    }
}
