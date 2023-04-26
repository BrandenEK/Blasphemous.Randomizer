using HarmonyLib;
using Framework.Managers;
using UnityEngine;
using Tools.Items;
using BlasphemousRandomizer.Notifications;

namespace BlasphemousRandomizer.ItemRando
{
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

    // Update shop dialog when buying items
    [HarmonyPatch(typeof(DialogWidget), "ShowBuy")]
    public class DialogWidget_Patch
    {
        public static void Prefix(ref string caption, ref string description, ref Sprite image)
        {
            // Can also use this to show different cost
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(image.name.ToUpper());
            if (item != null)
            {
                RewardInfo info = item.getRewardInfo(true);
                caption = info.name;
                description = info.description;
                image = info.sprite;
            }
        }
    }

    // Prevent the chalice from unfilling
    [HarmonyPatch(typeof(ChaliceEffect), "ClearEnemiesFlags")]
    public class ChaliceFlags_Patch
    {
        public static bool Prefix() { return false; }
    }
    [HarmonyPatch(typeof(ChaliceEffect), "UnfillChalice")]
    public class ChaliceItem_Patch
    {
        public static bool Prefix() { return false; }
    }
}
