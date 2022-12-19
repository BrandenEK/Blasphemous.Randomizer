using HarmonyLib;
using Tools.Level;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using Tools.Playmaker2.Condition;
using Framework.Inventory;
using Framework.Managers;
using UnityEngine;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Patches
{
    class World
    {
        // Always allow teleportation if enabled in config
        [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
        public class AlmsManager_Patch
        {
            public static bool Prefix(ref int __result)
            {
                if (Main.Randomizer.gameConfig.general.teleportationAlwaysUnlocked)
                {
                    __result = 3;
                    return false;
                }
                return true;
            }
        }

        // Always allow hint corpses to be active
        [HarmonyPatch(typeof(ItemIsEquiped), "executeAction")]
        public class ItemIsEquiped_Patch
        {
            public static bool Prefix(string objectIdStting, ItemIsEquiped __instance, ref bool __result)
            {
                if (objectIdStting == "RE04" && Main.Randomizer.gameConfig.general.allowHints && __instance.Owner.name != "LeftPuzzleCheck" && __instance.Owner.name != "RightPuzzleCheck")
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Interactable), "HasRequiredItem")]
        public class InteractableCorpse_Patch
        {
            public static bool Prefix(ref bool __result, InventoryObjectInspector ___requiredItem)
            {
                if (___requiredItem.id == "RE04" && Main.Randomizer.gameConfig.general.allowHints)
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        // Modify special interactables when they load
        [HarmonyPatch(typeof(Interactable), "OnAwake")]
        public class InteractableAltar_Patch
        {
            public static void Postfix(Interactable __instance)
            {
                if (!Main.Randomizer.isSpecialInteractable(__instance.GetPersistenID()))
                    return;
                string scene = Core.LevelManager.currentLevel.LevelName;
                //Log($"{interactable.transform.parent.name} ({interactable.name}): {interactable.GetPersistenID()}");

                // Update images of shop items
                if (scene == "D02BZ02S01" || scene == "D01BZ02S01" || scene == "D05BZ02S01")
                {
                    SpriteRenderer render = __instance.transform.parent.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                    if (render != null)
                    {
                        Item item = Main.Randomizer.itemShuffler.getItemAtLocation(render.sprite.name.ToUpper());
                        render.sprite = item == null ? null : item.getRewardInfo(true).sprite;
                    }
                }
                // Give holy visage reward & disable altar
                else if (scene == "D01Z04S19")
                {
                    Main.Randomizer.itemShuffler.giveItem("QI38", true);  // Change to spawn item pickup at center of room
                    Core.Events.SetFlag("ATTRITION_ALTAR_DONE", true, false);
                    __instance.gameObject.SetActive(false);
                }
                else if (scene == "D03Z03S16")
                {
                    Main.Randomizer.itemShuffler.giveItem("QI39", true);
                    Core.Events.SetFlag("CONTRITION_ALTAR_DONE", true, false);
                    __instance.gameObject.SetActive(false);
                }
                else if (scene == "D02Z03S21")
                {
                    Main.Randomizer.itemShuffler.giveItem("QI40", true);
                    Core.Events.SetFlag("COMPUNCTION_ALTAR_DONE", true, false);
                    __instance.gameObject.SetActive(false);
                }
            }
        }

        // Open WaHP gate faster
        //[HarmonyPatch(typeof(Lever), "Start")]
        //public class Lever_Patch
        //{
        //    public static void Postfix(LeverAction ___action, GameObject[] ___target)
        //    {
        //        string output = "";
        //        output += "Action: " + ___action.ToString() + "\n";
        //        output += "Targets: " + ___target.Length + "\n";
        //        foreach (GameObject obj in ___target)
        //        {
        //            output += "Components: \n";
        //            foreach (Component c in obj.GetComponents<Component>())
        //            {
        //                output += c.ToString() + "\n";
        //            }
        //        }
        //        Main.Randomizer.Log(output);
        //    }
        //}

        //[HarmonyPatch(typeof(PrieDieu), "KneeledMenuCoroutine")]
        //public class PrieDieu_Patch
        //{
        //    public static void Prefix(ref bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log("Setting use teleport to true");
        //        canUseTeleport = true;
        //    }

        //    public static void Postfix(bool canUseTeleport)
        //    {
        //        Main.Randomizer.Log(canUseTeleport.ToString());
        //    }
        //}
    }
}
