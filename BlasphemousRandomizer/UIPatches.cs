using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Tools.Items;
using Tools.Playmaker2.Action;
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

    // Skip certain cutscenes
    [HarmonyPatch(typeof(CutscenePlay), "OnEnter")]
    public class CutscenePlay_Patch
    {
        public static bool Prefix(CutscenePlay __instance)
        {
            if (__instance.cutscene != null && Main.Randomizer.shouldSkipCutscene(__instance.cutscene.name))
            {
                Main.Randomizer.Log("Skipping cutscene: " + __instance.cutscene.name);
                __instance.Finish();
                __instance.Fsm.Event(__instance.onSuccess);
                return false;
            }
            Main.Randomizer.Log("Playing cutscene: " + __instance.cutscene.name);
            return true;
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
    [HarmonyPatch(typeof(NewMapMenuWidget), "Initialize")]
    public class NewMapMenuWidget_Patch
    {
        public static void Postfix(NewMapMenuWidget __instance)
        {
            Transform items = __instance.transform.Find("ItemsText");
            if (items == null)
            {
                RectTransform rect = Object.Instantiate(__instance.CherubsText.gameObject, __instance.transform).transform as RectTransform;
                rect.name = "ItemsText";
                rect.anchoredPosition = new Vector2(45f, -60f);
                rect.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                items = rect;
            }
            items.GetComponentInChildren<Text>().text = $"{Main.Randomizer.Localize("items")}: {Main.Randomizer.itemsCollected}/{Main.Randomizer.totalItems}";
        }
    }

    // SHow items collected per zone
    [HarmonyPatch(typeof(NewMapMenuWidget), "UpdateCellData")]
    public class MapZoneItems_Patch
    {
        public static void Postfix(NewMapMenuWidget __instance, CellData ___CurrentCell)
        {
            Transform zoneItems = __instance.transform.Find("ZoneItemsText");
            if (zoneItems == null)
            {
                RectTransform rect = Object.Instantiate(__instance.CherubsText.gameObject, __instance.transform).transform as RectTransform;
                rect.name = "ZoneItemsText";
                rect.anchoredPosition = new Vector2(45f, -80f);
                rect.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                zoneItems = rect;
            }

            Main.Randomizer.LogWarning("Updating zone items text");
            zoneItems.GetComponent<Text>().text = "Desecrated Cistern: 7/22";
        }
    }
}
