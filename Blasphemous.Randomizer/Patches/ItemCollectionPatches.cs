using Blasphemous.ModdingAPI;
using HarmonyLib;
using Framework.Managers;
using Framework.Inventory;
using Tools.Playmaker2.Action;
using HutongGames.PlayMaker;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Entities;
using Blasphemous.ModdingAPI.Helpers;

namespace Blasphemous.Randomizer.Patches
{
    // Most item pickups in the game use this function
    [HarmonyPatch(typeof(InteractableInvAdd), "OnUsePost")]
    public class InteractableInvAdd_Patch
    {
        public static bool Prefix(string ___item)
        {
            ModLog.Info("InteractableInvAdd(" + ___item + ")");

            if (Main.Randomizer.itemShuffler.getItemAtLocation(___item) == null)
            {
                ModLog.Warn($"Location '{___item}' doesn't exist!");
                return true;
            }

            Main.Randomizer.itemShuffler.giveItem(___item, true);
            return false;
        }
    }

    // A few item pickups in the game use this function
    [HarmonyPatch(typeof(InteractableInventoryAdd), "OnUsePost")]
    public class InteractableInventoryAdd_Patch
    {
        public static bool Prefix(string ___item)
        {
            ModLog.Info("InteractableInventoryAdd(" + ___item + ")");
            Main.Randomizer.itemShuffler.giveItem(___item, true);
            return false;
        }
    }

    // This function is used for giving items in cutscenes / dialog
    [HarmonyPatch(typeof(ItemAddition), "executeAction")]
    public class ItemAddition_Patch
    {
        public static bool Prefix(string objectIdStting, FsmBool ___showMessage, ref bool __result)
        {
            ModLog.Info("ItemAddition(" + objectIdStting + ")");
            Main.Randomizer.itemShuffler.giveItem(objectIdStting, ___showMessage != null && ___showMessage.Value);
            __result = true;
            return false;
        }
    }

    // This function is used to display an item given from ItemAddition at a later time
    [HarmonyPatch(typeof(ItemAdditionMessage), "executeAction")]
    public class ItemAdditionMessage_Patch
    {
        public static bool Prefix(string objectIdStting, ref bool __result)
        {
            if (objectIdStting == "QI110")
            {
                ModLog.Info("SpecialVerseAddition(" + objectIdStting + ")");
                Main.Randomizer.itemShuffler.giveItem(objectIdStting, false);
            }

            ModLog.Info("ItemAdditionMessage(" + objectIdStting + ")");
            Main.Randomizer.itemShuffler.displayItem(objectIdStting);
            __result = true;
            return false;
        }
    }

    // Used for cherubs
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "OnCherubKilled")]
    public class CherubCaptorPersistentObject_Patch
    {
        public static bool Prefix(ref CherubCaptorPersistentObject __instance)
        {
            ModLog.Info("OnCherubKilled(" + __instance.cherubId + ")");
            __instance.destroyed = true;
            __instance.spawner.DisableCherubSpawn();
            Main.Randomizer.itemShuffler.giveItem(__instance.cherubId, true);
            return false;
        }
    }

    // Lady of Sorrows
    [HarmonyPatch(typeof(LifeUpgrade), "OnEnter")]
    public class LifeUpgrade_Patch
    {
        public static bool Prefix(LifeUpgrade __instance)
        {
            ModLog.Info("LifeUpgrade()");
            Main.Randomizer.itemShuffler.giveItem($"Lady[{Core.LevelManager.currentLevel.LevelName}]", true);
            __instance.Finish();
            return false;
        }
    }

    // Oil of Pilgrims
    [HarmonyPatch(typeof(FervourUpgrade), "OnEnter")]
    public class FervourUpgrade_Patch
    {
        public static bool Prefix(FervourUpgrade __instance)
        {
            ModLog.Info("FervourUpgrade()");
            Main.Randomizer.itemShuffler.giveItem($"Oil[{Core.LevelManager.currentLevel.LevelName}]", true);
            __instance.Finish();
            return false;
        }
    }

    // Sword Shrine
    [HarmonyPatch(typeof(StrengthUpgrade), "OnEnter")]
    public class StrengthUpgrade_Patch
    {
        public static bool Prefix(StrengthUpgrade __instance)
        {
            ModLog.Info("StrengthUpgrade()");
            Main.Randomizer.itemShuffler.giveItem($"Sword[{Core.LevelManager.currentLevel.LevelName}]", true);
            __instance.Finish();
            return false;
        }
    }
    [HarmonyPatch(typeof(MeaCulpaUpgrade), "OnEnter")]
    public class MeaCulpaUpgrade_Patch
    {
        public static bool Prefix(MeaCulpaUpgrade __instance)
        {
            // Can't upgrade mea culpa normally here, since stats can be duplicated in multiplayer
            // Instead this has to be upgraded in GiveItem(), once it has been verified that the location hasn't already been collected
            __instance.Finish();
            return false;
        }
    }

    // Used when giving tears of atonement to the player
    [HarmonyPatch(typeof(PurgeAdd), "OnEnter")]
    public class PurgeAdd_Patch
    {
        public static bool Prefix(PurgeAdd __instance)
        {
            int tears = __instance.value == null ? 0 : (int)__instance.value.Value;
            if (tears > 0)
            {
                //Tears are being added as a reward
                bool showMessage = __instance.ShowMessage != null && __instance.ShowMessage.Value;
                if (tears == 30000 || tears == 18000 || tears == 5000 && __instance.Owner.name == "BossTrigger")
                    showMessage = true;

                ModLog.Info("PurgeAdd(" + tears + ")");
                if (tears == 18000)
                {
                    Main.Randomizer.itemShuffler.giveItem($"Amanecida[{Core.LevelManager.lastLevel.LevelName}]", showMessage);
                }
                else
                {
                    Main.Randomizer.itemShuffler.giveItem($"{__instance.Owner.name}[{tears}]", showMessage);
                }
            }
            else
            {
                // Tears are being subtracted from buying something
                Core.Logic.Penitent.Stats.Purge.Current += tears;
            }

            __instance.Finish();
            return false;
        }
    }

    // Used when a boss is killed to give tears
    [HarmonyPatch(typeof(Penitent), "GetPurge")]
    public class Penitent_Patch
    {
        public static bool Prefix(Enemy enemy)
        {
            if (enemy.Id != "" && "BS01BS03BS04BS05BS06BS12BS13BS14BS16".Contains(enemy.Id))
            {
                // If killing and elder bro in the wall, return regular
                if (enemy.Id == "BS13" && Core.LevelManager.currentLevel.LevelName == "D09Z01S01") return true;

                ModLog.Info("GetPurge(" + enemy.Id + ")");
                Main.Randomizer.itemShuffler.giveItem(enemy.Id, true);
                return false;
            }
            return true;
        }
    }

    // Used when custom items are added from the modding api
    [HarmonyPatch(typeof(ItemHelper), nameof(ItemHelper.AddAndDisplayItem))]
    public class ItemModder_Patch
    {
        public static bool Prefix(string itemId)
        {
            ModLog.Info("ItemModderAdd(" + itemId + ")");

            if (Main.Randomizer.itemShuffler.getItemAtLocation(itemId) == null)
            {
                ModLog.Warn($"Location '{itemId}' doesn't exist!");
                return true;
            }

            Main.Randomizer.itemShuffler.giveItem(itemId, true);
            return false;
        }
    }
}
