using Blasphemous.ModdingAPI;
using HarmonyLib;
using Tools.Level;
using Tools.Playmaker2.Condition;
using Framework.Managers;
using Framework.Dialog;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.Patches
{
    // Always allow hint corpses to be active
    [HarmonyPatch(typeof(ItemIsEquiped), "executeAction")]
    public class ItemIsEquiped_Patch
    {
        public static bool Prefix(string objectIdStting, ItemIsEquiped __instance, ref bool __result)
        {
            if (objectIdStting == "RE04" && Main.Randomizer.GameSettings.AllowHints && __instance.Owner.name != "LeftPuzzleCheck" && __instance.Owner.name != "RightPuzzleCheck")
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
            if (___requiredItem.id == "RE04" && Main.Randomizer.GameSettings.AllowHints)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    // Update dialogs for hints and cleofas quest
    [HarmonyPatch(typeof(DialogManager), "StartConversation")]
    public class DialogManager_Patch
    {
        public static void Prefix(string conversiationId, Dictionary<string, DialogObject> ___allDialogs)
        {
            ModLog.Info("Starting dialog: " + conversiationId);

            if (conversiationId == "DLG_QT_0904")
            {
                // Change socorro options for cleofas quest
                DialogObject current = ___allDialogs[conversiationId];
                if (current.answersLines.Count > 1)
                    current.answersLines.RemoveAt(1);
            }
            else if (Main.Randomizer.GameSettings.AllowHints && conversiationId.Length == 8 && int.TryParse(conversiationId.Substring(4), out int id) && id > 2000 && id < 2035)
            {
                // Change corpse hints
                string hint = Main.Randomizer.hintShuffler.getHint(conversiationId);
                DialogObject current = ___allDialogs[conversiationId];
                current.dialogLines.Clear();
                current.dialogLines.Add(hint);
            }
        }
    }
}
