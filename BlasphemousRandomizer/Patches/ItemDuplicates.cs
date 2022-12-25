using HarmonyLib;
using Framework.Managers;
using Tools.Playmaker2.Action;
using Tools.Playmaker2.Condition;

namespace BlasphemousRandomizer.Patches
{
    public static class ItemFlags
    {
        public static string[] bannedFlags = new string[]
        {
			"VIRIDIANA_MATURE",
			"VIRIDIANA_OLD",
			"VIRIDIANA_DEAD",
			"TIRSO_KISSER1_DEAD",
			"TIRSO_KISSER2_DEAD",
			"TIRSO_KISSER3_DEAD",
			"TIRSO_KISSER4_DEAD",
			"TIRSO_KISSER5_DEAD",
			"TIRSO_KISSER6_DEAD",
			"TIRSO_TIRSO_DEAD",
			"CLEOFAS_SUICIDAL",
			"CLEOFAS_DEAD"
		};

		public static string[] thornScenes = new string[]
		{
			"D01Z04S17",
			"D17Z01S12",
			"D03Z03S14",
			"D02Z02S06",
			"D04Z02S17",
			"D05Z01S17",
			"D09Z01S13"
		};

		public static string[] duplicateScenes = new string[]
		{
			"D01Z04S18",
			"D03Z03S15",
			"D02Z03S20",
			"D04BZ03S01",
			"D02Z03S19",
			"D05Z01S15",
			"D08Z01S01",
			"D01BZ08S01",
			"D04BZ02S01",
			"D04Z02S20",
			"D02Z01S01",
			"D02Z01S01",
			"D02Z01S04",
			"D02Z01S04",
			"D01BZ02S01",
			"D01BZ02S01",
			"D01BZ02S01",
			"D02BZ02S01",
			"D02BZ02S01",
			"D02BZ02S01",
			"D05BZ02S01",
			"D05BZ02S01",
			"D05BZ02S01",
			"D01Z04S18",
			"D02Z03S20",
			"D03Z03S15",
			"D04Z02S22",
			"D05Z02S14",
			"D01BZ04S01",
			"D01BZ04S01",
			"D03Z01S06",
			"D01BZ04S01",
			"D20Z03S01"
		};

		public static string[] duplicateItems = new string[]
		{
			"QI38",
			"QI39",
			"QI40",
			"QI60",
			"QI61",
			"QI62",
			"PR09",
			"QI201",
			"QI54",
			"RE03",
			"QI59",
			"QI68",
			"QI59",
			"QI68",
			"QI58",
			"RB05",
			"RB09",
			"QI11",
			"RB02",
			"RB37",
			"QI49",
			"RB12",
			"QI71",
			"PR08",
			"PR08",
			"PR08",
			"PR08",
			"PR08",
			"RB104",
			"RB105",
			"RB13",
			"PR11",
			"QI203"
		};
	}

	// Don't allow certain npc's death flags to be set
	[HarmonyPatch(typeof(FlagModification), "OnEnter")]
	public class FlagModification_Patch
	{
		public static bool Prefix(FlagModification __instance)
		{
			if (!Main.Randomizer.gameConfig.items.disableNPCDeath 
				|| !FileUtil.arrayContains(ItemFlags.bannedFlags, __instance.flagName.Value) 
				|| Core.LevelManager.currentLevel.LevelName == "D07Z01S01")
            {
				if (__instance.flagName.Value != "SANTOS_LAUDES_ACTIVATED")
					Core.Events.SetFlag(__instance.flagName.Value, __instance.state.Value, false);  // Fix up this function
            }
			__instance.Finish();
			return false;
        }
    }

	// Change the value that certain flags are referencing
	[HarmonyPatch(typeof(FlagExists), "OnEnter")]
	public class FlagExists_Patch
    {
		public static bool Prefix(FlagExists __instance)
        {
			// Get flag name & scene name
			string text = __instance.flagName.Value.ToUpper().Replace(' ', '_');
			bool flag = Core.Events.GetFlag(text);
			string scene = Core.LevelManager.currentLevel.LevelName;
			Main.Randomizer.Log("Checking for flag: " + text);

			// Bridge - Make Esdras appear once you have 3 holy wounds
			if (scene == "D08Z01S01")
			{
				if (text == "D01Z06S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI38");
				}
				else if (text == "D02Z05S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI39");
				}
				else if (text == "D03Z04S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI40");
				}
			}
			// Rooftops elevator - Only unlock new floors when a mask is placed
			if (scene == "D06Z01S01" && (text == "D04Z02S17_ARCHDEACON1ITEMTAKEN" || text == "D05Z01S15_ARCHDEACON2ITEMTAKEN" || text == "D02Z03S19_ARCHDEACON3ITEMTAKEN"))
            {
				flag = false;
            }
			// Albero sick house - Only give final reward once all herbs are delivered
			if (scene == "D01Z02S02" && text == "D09Z01S03_BOSSDEAD" && (!Core.Events.GetFlag("TIRSO_QI19_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI20_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI37_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI63_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI64_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI65_DELIVERED")))
			{
				flag = false;
			}
			// Gemino - Dont progress quest ever (Move to FlagModification)
			if (scene == "D02Z01S01" && Main.Randomizer.gameConfig.items.disableNPCDeath && (text == "D01Z06S01_BOSSDEAD" || text == "D03Z04S01_BOSSDEAD" || text == "D08Z01S01_BOSSDEAD"))
			{
				flag = false;
			}
			// Viridiana reward
			if (text == "VIRIDIANA_REWARD_COMPLETED" && (scene == "D01Z04S18" || scene == "D02Z03S20" || scene == "D03Z03S15" || scene == "D04Z02S22" || scene == "D05Z02S14"))
			{
				flag = Core.Events.GetFlag("LOCATION_PR08");
			}
			// Redento corpse
			if (scene == "D04BZ02S01" && text == "REDENTO_QI54_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI54");
			}
			// Altasgracias cacoon
			if (scene == "D03Z03S10" && text == "ALTASGRACIAS_LAST_REWARD")
			{
				flag = Core.Events.GetFlag("LOCATION_RB06");
			}
			// Gemino bead
			if ((scene == "D02Z01S01" || scene == "D02Z01S04") && text == "GEMINO_RB10_REWARD")
			{
				flag = Core.Events.GetFlag("LOCATION_RB10");
			}
			// Gemino flowers
			if ((scene == "D02Z01S01" || scene == "D02Z01S04") && text == "GEMINO_OFFERING_DONE")
			{
				flag = Core.Events.GetFlag("LOCATION_QI68");
			}
			// Perpetva scapular
			if (scene == "D20Z03S01" && text == "D08Z01S01_BOSSDEAD")
            {
				flag = false;
            }
			// Esdras key
			if (scene == "D17Z01S15" && text == "ESDRAS_CHAPEL")
			{
				flag = true;
			}
			// Brotherhood church
			if (scene == "D17Z01S14" && (text == "ESDRAS_CHAPEL" || text == "D06Z01S25_BOSSDEAD"))
            {
				flag = Core.InventoryManager.IsQuestItemOwned("QI203");
            }
			// Fourth visage
			if (scene == "D04Z03S02" && text == "D06Z01S25_BOSSDEAD")
            {
				flag = false;
            }
			// Crisanta wound
			if (scene == "D17Z01S15" && text == "CRISANTA_LIBERATED")
            {
				flag = Core.Events.GetFlag("D06Z01S25_BOSSDEAD") && Core.InventoryManager.IsSwordOwned("HE201");
            }
			// Perpetva defeat
			if (scene == "D03Z01S06" && text == "BROTHERS_EVENTPERPETVA_COMPLETED")
            {
				flag = Core.Events.GetFlag("BROTHERS_PERPETUA_DEFEATED");
            }
			// Mercy shop
			if (scene == "D01BZ02S01" && text == "QI58_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI58");
			}
			// Graveyard shop
			if (scene == "D02BZ02S01" && text == "QI11_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI11");
			}
			// Canvases shop
			if (scene == "D05BZ02S01" && text == "QI49_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI49");
			}
			// Canvases shop
			if (scene == "D05BZ02S01" && text == "QI71_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI71");
			}

			// Finish action
			if (__instance.outValue != null)
				__instance.outValue.Value = flag;
			if (flag)
				__instance.Fsm.Event(__instance.flagAvailable);
			else
				__instance.Fsm.Event(__instance.flagUnavailable);
			__instance.Finish();
			return false;
		}
    }

	// Change what certain item checks are referencing
	[HarmonyPatch(typeof(ItemIsOwned), "executeAction")]
	public class ItemIsOwned_Patch
    {
		public static bool Prefix(string objectIdStting, ref bool __result)
		{
			// Get item name & scene name
			string item = objectIdStting;
			string scene = Core.LevelManager.currentLevel.LevelName;
			Main.Randomizer.Log("Checking for item owned: " + item);

			// If any standard scene-item pair
			for (int i = 0; i < ItemFlags.duplicateScenes.Length; i++)
			{
				if (scene == ItemFlags.duplicateScenes[i] && item == ItemFlags.duplicateItems[i])
				{
					__result = Core.Events.GetFlag("LOCATION_" + item);
					return false;
				}
			}
			// Thorn upgrades
			if (scene.Contains("D19") || FileUtil.arrayContains(ItemFlags.thornScenes, scene))
            {
				if (item == "QI31")
                {
					__result = !Core.Events.GetFlag("LOCATION_QI32");
					return false;
				}
				if (item == "QI32")
                {
					__result = Core.Events.GetFlag("LOCATION_QI32") && !Core.Events.GetFlag("LOCATION_QI33");
					return false;
				}
				if (item == "QI33")
                {
					__result = Core.Events.GetFlag("LOCATION_QI33") && !Core.Events.GetFlag("LOCATION_QI34");
					return false;
				}
				if (item == "QI34")
                {
					__result = Core.Events.GetFlag("LOCATION_QI34") && !Core.Events.GetFlag("LOCATION_QI35");
					return false;
				}
				if (item == "QI35")
                {
					__result = Core.Events.GetFlag("LOCATION_QI35") && !Core.Events.GetFlag("LOCATION_QI79");
					return false;
				}
				if (item == "QI79")
                {
					__result = Core.Events.GetFlag("LOCATION_QI79") && !Core.Events.GetFlag("LOCATION_QI80");
					return false;
				}
				if (item == "QI80")
                {
					__result = Core.Events.GetFlag("LOCATION_QI80") && !Core.Events.GetFlag("LOCATION_QI81");
					return false;
				}
				if (item == "QI81")
                {
					__result = Core.Events.GetFlag("LOCATION_QI81");
					return false;
				}
			}
			// Tirso cloth
			if (item == "QI66")
            {
				if (scene == "D01Z02S05")
                {
					__result = true;
					return false;
				}
				if (scene == "D01Z02S02")
                {
					__result = false;
					return false;
				}
			}
			// Holy Line Deosgracias
			if (scene == "D01Z01S07" && (item == "QI38" || item == "QI39" || item == "QI40"))
			{
				__result = Core.Events.GetFlag("LOCATION_QI31");
				return false;
			}
			// Esdras Scapular
			if (scene == "D08Z01S01" && item == "QI203")
            {
				__result = false;
				return false;
            }
			// Amanecida verses
			if ((scene == "D02Z02S14" || scene == "D03Z01S03" || scene == "D04Z01S04" || scene == "D09Z01S01" || scene.Contains("D21"))
				&& (item == "PR101" || item == "QI107" || item == "QI108" || item == "QI109" || item == "QI110"))
            {
				__result = Core.Events.GetFlag("LOCATION_" + item);
				return false;
            }
			// Red candle
			if (scene == "D01Z04S08" && "RB17RB18RB19".Contains(item))
			{
				__result = Core.Events.GetFlag("LOCATION_RB17");
				return false;
			}
			if (scene == "D02Z03S06" || scene == "D05Z01S02")
			{
				if (item == "RB17")
				{
					__result = !Core.Events.GetFlag("LOCATION_RB18") && (Core.InventoryManager.IsRosaryBeadOwned("RB17") || Core.InventoryManager.IsRosaryBeadOwned("RB18") || Core.InventoryManager.IsRosaryBeadOwned("RB19"));
					return false;
				}
				if (item == "RB18")
				{
					__result = Core.Events.GetFlag("LOCATION_RB18");
					return false;
				}
			}
			// Blue candle
			if (scene == "D02Z03S17" && "RB24RB25RB26".Contains(item))
			{
				__result = Core.Events.GetFlag("LOCATION_RB24");
				return false;
			}
			if (scene == "D17Z01S04" || scene == "D01Z04S16")
			{
				if (item == "RB24")
				{
					__result = !Core.Events.GetFlag("LOCATION_RB25") && (Core.InventoryManager.IsRosaryBeadOwned("RB24") || Core.InventoryManager.IsRosaryBeadOwned("RB25") || Core.InventoryManager.IsRosaryBeadOwned("RB26"));
					return false;
				}
				if (item == "RB25")
				{
					__result = Core.Events.GetFlag("LOCATION_RB25");
					return false;
				}
			}

			// If none of these special conditions
			return true;
		}

		// Prevent progressive items from being removed from the inventory
		[HarmonyPatch(typeof(ItemSubstraction), "executeAction")]
		public class ItemSubstraction_Patch
		{
			public static bool Prefix(string objectIdStting, ref bool __result)
			{
				string invalidItems = "RB17RB18RB19RB24RB25RB26QI31QI32QI33QI34QI35QI79QI80QI81QI107QI108QI109QI110";
				if (invalidItems.Contains(objectIdStting))
				{
					__result = true;
					return false;
				}
				return true;
			}
		}
	}
}
