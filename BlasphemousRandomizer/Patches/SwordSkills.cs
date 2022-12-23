using HarmonyLib;
using System.Collections.Generic;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Framework.Managers;
using Framework.FrameworkCore;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Patches
{
    // Show name, description, and image of new item
    [HarmonyPatch(typeof(NewInventory_Description), "SetKill")]
    public class InvDescription_Patch
    {
        public static void Postfix(string skillId, ref Text ___caption, ref Text ___description, ref TextMeshProUGUI ___instructionsPro)
        {
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(skillId);
            if (item != null)
            {
                if (Core.SkillManager.IsSkillUnlocked(skillId))
                {
                    RewardInfo info = item.getRewardInfo(false);
                    ___caption.text = info.name;
                    ___description.text = info.description;
                }
                else if (Core.SkillManager.CanUnlockSkillNoCheckPoints(skillId))
                {
                    RewardInfo info = item.getRewardInfo(true);
                    ___caption.text = info.name;
                    ___description.text = info.description;
                }
                else
                {
                    ___caption.text = "???";
                    ___description.text = "";
                }
                ___instructionsPro.text = "";
            }
        }
    }
    [HarmonyPatch(typeof(NewInventory_Skill), "UpdateStatus")]
    public class InvSkill_Patch
    {
        public static void Postfix(string ___skill, ref Image ___skillImage)
        {
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(___skill);
            if (item != null)
            {
                RewardInfo info = item.getRewardInfo(!Core.SkillManager.IsSkillUnlocked(___skill));
                ___skillImage.sprite = info.sprite;
            }
        }
        //public static bool Prefix(string ___skill, ref Image ___skillImage, ref GameObject ___backgorundLocked, ref GameObject ___backgorundUnLocked, ref GameObject ___backgorundEnabled, ref Text ___tierText, Color ___disabledColor)
        //{
        //    UnlockableSkill skill = Core.SkillManager.GetSkill(___skill);
        //    Item item = Main.Randomizer.itemShuffler.getItemAtLocation(___skill);
        //    if (item != null)
        //        ___skillImage.sprite = item.getRewardInfo(true).sprite;

        //    // Set initial states
        //    ___backgorundLocked.SetActive(false);
        //    ___backgorundUnLocked.SetActive(false);
        //    ___backgorundEnabled.SetActive(false);
        //    ___tierText.text = string.Empty;
        //    bool locked = false;

        //    if (Core.Events.GetFlag("LOCATION_" + ___skill))
        //    {
        //        // Slot has already been purchased
        //        ___backgorundUnLocked.SetActive(true);
        //    }
        //    else if (Core.SkillManager.CanUnlockSkillNoCheckPoints(___skill))
        //    {
        //        // Slot is able to be purchased
        //        ___backgorundEnabled.SetActive(true);
        //    }
        //    else
        //    {
        //        // Slot cant be purchased
        //        ___tierText.text = skill.tier.ToString();
        //        ___tierText.color = ___disabledColor;
        //        locked = true;
        //    }
        //}
    }

    // Unlocks a skill or buys a random item - ignoreChecks is changed to be whether to give actual skill or not
    [HarmonyPatch(typeof(SkillManager), "UnlockSkill")]
    public class SkillManagerUnlock_Patch
    {
        public static bool Prefix(SkillManager __instance, string skill, bool ignoreChecks, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            if (ignoreChecks)
            {
                // Actually give item
                ___allSkills[skill].unlocked = true;
            }
            else
            {
                // Buying item at mea culpa shrine
                if (__instance.CanUnlockSkill(skill, false))
                {
                    Main.Randomizer.Log("UnlockSkill(" + skill + ")");
                    Main.Randomizer.itemShuffler.giveItem(skill, true);
                    Core.Logic.Penitent.Stats.Purge.Current -= ___allSkills[skill].cost;
                }
            }
            return false;
        }
    }

    // Check if skill is unlockable
    [HarmonyPatch(typeof(SkillManager), "CanUnlockSkill")]
    public class SkillManagerCanUnlock_Patch
    {
        public static bool Prefix(SkillManager __instance, ref bool __result, string skill, bool ignoreChecks, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            // Cant unlock if slot is already purchased
            if (!___allSkills.ContainsKey(skill) || Core.Events.GetFlag("LOCATION_" + skill))
            {
                __result = false;
                return false;
            }

            // Can only unlock if other checks and has enough money
            __result = ignoreChecks || __instance.CanUnlockSkillNoCheckPoints(skill) && Core.Logic.Penitent.Stats.Purge.Current >= ___allSkills[skill].cost;
            return false;
        }
    }
    [HarmonyPatch(typeof(SkillManager), "CanUnlockSkillNoCheckPoints")]
    public class SkillManagerCanUnlockExtra_Patch
    {
        public static bool Prefix(SkillManager __instance, ref bool __result, string skill, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            // Cant unlock if slot is already purchased or mea culpa level is not enough
            if (!___allSkills.ContainsKey(skill) || Core.Events.GetFlag("LOCATION_" + skill) || __instance.GetCurrentMeaCulpa() < ___allSkills[skill].tier)
            {
                __result = false;
                return false;
            }

            // Can only unlock if slot is root or parent slot has been bought
            string parentSkill = ___allSkills[skill].GetParentSkill();
            __result = parentSkill == string.Empty || Core.Events.GetFlag("LOCATION_" + parentSkill);
            return false;
        }
    }
    [HarmonyPatch(typeof(SkillManager), "IsSkillUnlocked")]
    public class SkillManagerIsUnlocked_Patch
    {
        public static bool Prefix(ref string skill, ref bool __result)
        {
            if (skill.Length > 0 && skill[0] == '*')
            {
                // Actually checking if skill is unlocked
                skill = skill.Substring(1, skill.Length - 1);
                return true;
            }
            else
            {
                // Only checking for if the slot has been purchased
                __result = Core.Events.GetFlag("LOCATION_" + skill);
                return false;
            }
        }
    }

    // Special request to check if skill is unlocked
    [HarmonyPatch(typeof(Ability), "GetLastUnlockedSkill")]
    public class Ability_Patch
    {
        public static bool Prefix(ref UnlockableSkill __result, List<string> ___unlocableSkill)
        {
            __result = null;
            foreach (string skill in ___unlocableSkill)
            {
                if (!Core.SkillManager.IsSkillUnlocked("*" + skill))
                    break;
                __result = Core.SkillManager.GetSkill(skill);
            }
            return false;
        }
    }
}
