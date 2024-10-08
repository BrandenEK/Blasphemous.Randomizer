﻿using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.ItemRando;
using Framework.Managers;
using Framework.FrameworkCore;
using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Blasphemous.Randomizer.Patches
{
    // Show name, description, and image of new item
    [HarmonyPatch(typeof(NewInventory_Description), "SetKill")]
    public class InvDescription_Patch
    {
        public static void Postfix(string skillId, ref Text ___caption, ref Text ___description, ref TextMeshProUGUI ___instructionsPro)
        {
            // Only show changed items at shrine
            if (!Main.Randomizer.ShrineEditMode)
            {
                return;
            }

            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(skillId);
            if (item != null)
            {
                if (Core.SkillManager.IsSkillUnlocked(skillId))
                {
                    ___caption.text = item.GetName(false);
                    ___description.text = item.GetDescription(false);
                }
                else if (Core.SkillManager.CanUnlockSkillNoCheckPoints(skillId))
                {
                    ___caption.text = item.GetName(true);
                    ___description.text = item.GetDescription(true);
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
    public class InvSkillImage_Patch
    {
        public static void Postfix(string ___skill, ref Image ___skillImage, ref Text ___tierText)
        {
            // Any skill not owned is locked
            if (!Main.Randomizer.ShrineEditMode)
            {
                ___tierText.text = "";
                return;
            }

            // Show image of new item
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(___skill);
            if (item != null)
            {
                ___skillImage.sprite = item.GetImage(!Core.SkillManager.IsSkillUnlocked(___skill));
            }
        }
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
                    ModLog.Info("UnlockSkill(" + skill + ")");
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
            // Might not be necessary
            if (!Main.Randomizer.ShrineEditMode)
            {
                __result = false;
                return false;
            }

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
            // Always locked in regular pause menu
            if (!Main.Randomizer.ShrineEditMode)
            {
                __result = false;
                return false;
            }

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
            if (!Main.Randomizer.ShrineEditMode)
            {
                // Actually checking if skill is unlocked
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

    // Update shrine edit mode status
    [HarmonyPatch(typeof(NewInventory_LayoutSkill), "ShowLayout")]
    public class InvSkillShow_Patch
    {
        public static void Prefix(bool editMode)
        {
            Main.Randomizer.ShrineEditMode = editMode;
        }

        public static void Postfix(bool editMode, NewInventory_LayoutSkill __instance, ref Text ___maxTier)
        {
            // Change text to show mea culpa / shrine tier
            if (!editMode)
                ___maxTier.text = Core.Logic.Penitent.Stats.Strength.GetUpgrades().ToString();

            Text captionText = __instance.transform.GetChild(3).GetComponent<Text>();
            if (captionText != null)
                captionText.text = Main.Randomizer.LocalizationHandler.Localize(editMode ? "shrine" : "sword");
        }
    }
    [HarmonyPatch(typeof(NewInventory_LayoutSkill), "CancelEditMode")]
    public class InvSkillCancel_Patch
    {
        public static void Postfix()
        {
            Main.Randomizer.ShrineEditMode = false;
        }
    }
}
