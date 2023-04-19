using System.Collections.Generic;
using HarmonyLib;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Achievements;

namespace BlasphemousRandomizer.Notifications
{
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
    public class PopupAchievementWidgetShow_Patch
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
