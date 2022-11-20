using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Tools.Items;
using Tools.Playmaker2.Action;
using Framework.Achievements;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using BlasphemousRandomizer.Structures;
using Framework.Managers;

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

    // Skip certain cutscenes
    [HarmonyPatch(typeof(CutscenePlay), "OnEnter")]
    public class CutscenePlay_Patch
    {
        public static bool Prefix(CutscenePlay __instance)
        {
            if (__instance.cutscene != null && Main.Randomizer.shouldSkipCutscene(__instance.cutscene.name))
            {
                __instance.Finish();
                __instance.Fsm.Event(__instance.onSuccess);
                return false;
            }
            return true;
        }
    }

    // Allow unequipping true heart
    [HarmonyPatch(typeof(ItemTemporalEffect), "ContainsEffect")]
    public class ItemTemporalEffect_ContainsPatch
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

    // Allow all achievement messages to go through
    [HarmonyPatch(typeof(UIController), "ShowPopupAchievement")]
    public class UIController_Patch
    {
        public static bool Prefix(Achievement achievement, PopupAchievementWidget ___popupAchievementWidget)
        {
            if (achievement.GetType() == typeof(RewardAchievement))
            {
                ___popupAchievementWidget.ShowPopup(achievement);
                UIController.instance.ShowPopUp("", "", 0.001f, false);
            }
            return false;
        }
    }

    // Change achievement message & time
    [HarmonyPatch(typeof(PopupAchievementWidget), "ShowPopupCorrutine")]
    public class PopupAchievementWidget_Show_Patch
    {
        public static void Prefix(RectTransform ___PopUp, ref float ___startDelay, ref float ___popupShowTime, ref float ___endTime, Achievement achievement)
        {
            ___startDelay = 0.1f;
            ___popupShowTime = 2f;
            ___endTime = 0.1f;
            ___PopUp.GetChild(1).GetComponent<Text>().text = achievement.Name;
            ___PopUp.GetChild(2).GetComponent<Text>().text = achievement.Description;
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
            items.GetComponentInChildren<Text>().text = $"Items collected: {Main.Randomizer.itemsCollected}/{Main.Randomizer.totalItems}";
        }
    }

    // Slight workaround to have CloseDialog event called when showing notification
    [HarmonyPatch(typeof(PopUpWidget), "ShowPopUp")]
    public class PopUpWidget_Patch
    {
        public static void Postfix(GameObject ___messageRoot, float timeToWait)
        {
            if (timeToWait == 0.001f)
                ___messageRoot.SetActive(false);
        }
    }
}
