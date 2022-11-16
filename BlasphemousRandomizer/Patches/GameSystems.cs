using HarmonyLib;
using Framework.Managers;
using Framework.EditorScripts.EnemiesBalance;
using Framework.EditorScripts.BossesBalance;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Widgets;

namespace BlasphemousRandomizer.Patches
{
    // Make enemies stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentEnemiesBalanceChart")]
    public class GameModeEnemies_Patch
    {
        public static bool Prefix(ref EnemiesBalanceChart __result, EnemiesBalanceChart ___newGameEnemiesBalanceChart)
        {
            __result = ___newGameEnemiesBalanceChart;
            return false;
        }
    }

    // Make bosses stay as ng
    [HarmonyPatch(typeof(GameModeManager), "GetCurrentBossesBalanceChart")]
    public class GameModeBosses_Patch
    {
        public static bool Prefix(ref BossesBalanceChart __result, BossesBalanceChart ___newGameBossesBalanceChart)
        {
            __result = ___newGameBossesBalanceChart;
            return false;
        }
    }

    // Set up new game when select new save file
    [HarmonyPatch(typeof(NewMainMenu), "InternalPlay")]
    public class NewMainMenu_Patch
    {
        public static void PostFix(bool ___isContinue)
        {
            if (!___isContinue)
            {
                Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
                // Call randomizer newGame function
            }
        }
    }

    // Don't allow ascending a save file
    [HarmonyPatch(typeof(SaveSlot), "SetData")]
    public class SaveSlot_Patch
    {
        public static void Prefix(ref bool canConvert)
        {
            canConvert = false;
        }
    }

    /* Initialize Randomizer class
    [HarmonyPatch(typeof(AchievementsManager), "AllInitialized")]
    public class AchievementsManager_Patch
    {
        public static void Postfix()
        {

        }
    }
    */
}
