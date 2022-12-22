using HarmonyLib;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine.UI;
using TMPro;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Patches
{
    // Show name and description of new item
    [HarmonyPatch(typeof(NewInventory_Description), "SetKill")]
    public class InvDescription_Patch
    {
        public static void Postfix(string skillId, ref Text ___caption, ref Text ___description, ref TextMeshProUGUI ___instructionsPro)
        {
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(skillId);
            if (item != null)
            {
                RewardInfo info = item.getRewardInfo(true);
                ___caption.text = info.name;
                ___description.text = info.description;
                ___instructionsPro.text = "";
            }
        }
    }
}
