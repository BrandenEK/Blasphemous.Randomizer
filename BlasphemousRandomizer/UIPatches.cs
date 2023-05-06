using HarmonyLib;
using Tools.Items;
using Tools.Playmaker2.Action;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using Framework.Managers;
using Framework.Map;

namespace BlasphemousRandomizer
{
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

    // Dont show area pop up in door shuffle
    [HarmonyPatch(typeof(UIController), "ShowAreaPopUp")]
    public class UIControllerArea_Patch
    {
        public static bool Prefix()
        {
            return Main.Randomizer.gameConfig.DoorShuffleType <= 1;
        }
    }

    // Skip certain cutscenes
    [HarmonyPatch(typeof(CutscenePlay), "OnEnter")]
    public class CutscenePlay_Patch
    {
        public static bool Prefix(CutscenePlay __instance)
        {
            if (__instance.cutscene != null && Main.Randomizer.shouldSkipCutscene(__instance.cutscene.name))
            {
                Main.Randomizer.Log("Skipping cutscene: " + __instance.cutscene.name);
                __instance.Fsm.Event(__instance.onSuccess);
                __instance.Finish();
                return false;
            }
            Main.Randomizer.Log("Playing cutscene: " + __instance.cutscene.name);
            return true;
        }

        //public static void Postfix(CutscenePlay __instance)
        //{
        //    if (__instance.cutscene == null) return;

        //    if (Main.Randomizer.shouldSkipCutscene(__instance.cutscene.name))
        //    {
        //        Main.Randomizer.Log("Skipping cutscene: " + __instance.cutscene.name);
        //        Core.Cinematics.EndCutscene(true);
        //    }
        //    else
        //    {
        //        Main.Randomizer.Log("Playing cutscene: " + __instance.cutscene.name);
        //    }
        //}
    }

    // Decrease time spent on boss defeated message
    [HarmonyPatch(typeof(ShowFullMessage), "OnEnter")]
    public class FullMessage_Patch
    {
        public static void Prefix(ShowFullMessage __instance)
        {
            __instance.totalTime = 0f;
        }
    }

    // Allow unequipping true heart
    [HarmonyPatch(typeof(ItemTemporalEffect), "ContainsEffect")]
    public class ItemTemporalEffectContains_Patch
    {
        public static bool Prefix(ref bool __result, ItemTemporalEffect.PenitentEffects effect)
        {
            if (effect == ItemTemporalEffect.PenitentEffects.DisableUnEquipSword)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(ItemTemporalEffect), "OnApplyEffect")]
    public class ItemTemporalEffect_ApplyPatch
    {
        public static void Postfix()
        {
            Core.Logic.Penitent.AllowEquipSwords = true;
        }
    }

    // Show number of items collected on map screen
    [HarmonyPatch(typeof(NewMapMenuWidget), "UpdateCellData")]
    public class MapZoneItems_Patch
    {
        public static void Postfix(NewMapMenuWidget __instance, CellData ___CurrentCell)
        {
            Main.Randomizer.MapCollection.UpdateMap(__instance, ___CurrentCell);
        }
    }
}
