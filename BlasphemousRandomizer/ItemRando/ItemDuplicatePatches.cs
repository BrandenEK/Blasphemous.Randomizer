using HarmonyLib;
using Framework.Managers;
using Tools.Playmaker2.Action;
using Tools.Playmaker2.Condition;
using Tools.Level.Actionables;
using Tools.Gameplay;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using System.Collections.Generic;

namespace BlasphemousRandomizer.ItemRando
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

		public static string[] itemsToNotRemove = new string[]
		{
			"RB17",
			"RB18",
			"RB19",
			"RB24",
			"RB25",
			"RB26",
			"QI31",
			"QI32",
			"QI33",
			"QI34",
			"QI35",
			"QI79",
			"QI80",
			"QI81",
			"QI107",
			"QI108",
			"QI109",
			"QI110",
		};

		public static string[] elevatorFlags = new string[]
		{
			"D01Z02S03_ELEVATOR_IN_ALBERO",
			"D02Z05S01_ELEVATOR2_ON_BOTTOM",
			"D02Z05S01_ELEVATOR1_ON_BOTTOM",
			"D03Z05S01_ELEVATOR_ON_BOTTOM",
			"D04Z02S24_ELEVATOR_ON_BOTTOM",
			"D05Z04S01_ELEVATOR_ON_BOTTOM",
			"ELEVATOR_POSITION_1",
			"ELEVATOR_POSITION_2",
			"ELEVATOR_POSITION_3",
			"ELEVATOR_POSITION_4",
		};

		public static string[] elevatorGateIds = new string[]
		{
			"835afc15-8de9-47bf-9057-54a0f23d69c6",
			"6aa4b846-441a-4be6-8786-49a915b0df97",
			"6aa4b846-441a-4be6-8786-49a915b0df97",
			"17b487db-fb2c-4ac2-ace2-0017b67a7eda",
			"c1c68418-eb19-48f6-8bf6-fc979b8df329",
		};
		public static string[] elevatorGateScenes = new string[]
		{
			"D02Z02S11",
			"D01Z05S25",
			"D04Z02S24",
			"D05Z01S21",
			"D05Z01S21",
		};
	}

	// Always set elevators to default positions with gates open
	[HarmonyPatch(typeof(EventManager), "GetCurrentPersistentState")]
	public class EventManagerSave_Patch
    {
		public static void Prefix(Dictionary<string, FlagObject> ___flags)
        {
			string[] elevators = ItemFlags.elevatorFlags;
			for (int i = 0; i < elevators.Length; i++)
            {
				if (___flags.ContainsKey(elevators[i]))
					___flags[elevators[i]].value = i == elevators.Length - 1 && !___flags["ELEVATOR_POSITION_FAKE"].value || i <= 1 && Core.LevelManager.currentLevel.LevelName == "D01Z02S03" && Core.Events.GetFlag("D01Z02S03_ELEVATOR");
			}
        }
    }
	[HarmonyPatch(typeof(Gate), "GetCurrentPersistentState")]
	public class GateSave_Patch
    {
		public static void Prefix(Gate __instance, ref bool ___open)
        {
			string gateId = __instance.GetPersistenID();
			for (int i = 0; i < ItemFlags.elevatorGateIds.Length; i++)
            {
				if (Core.LevelManager.currentLevel.LevelName == ItemFlags.elevatorGateScenes[i] && gateId == ItemFlags.elevatorGateIds[i])
					___open = true;
            }
        }
    }
	[HarmonyPatch(typeof(StraightMovingPlatform), "SetCurrentPersistentState")]
	public class PlatformLoad_Patch
	{
		public static bool Prefix(StraightMovingPlatform __instance)
		{
			return Core.LevelManager.currentLevel.LevelName != "D02Z02S11" || __instance.GetPersistenID() != "e78fa0c2-ba1f-40f4-97cc-394162f84d7c";
		}
	}

	// Only have laudes activated in boss room with verse
	[HarmonyPatch(typeof(EventManager), "GetFlag")]
	public class EventManagerGet_Patch
	{
		public static void Postfix(string id, EventManager __instance, ref bool __result)
		{
			string formatted = __instance.GetFormattedId(id);
			if (formatted == "SANTOS_LAUDES_ACTIVATED")
			{
				string scene = Core.LevelManager.currentLevel.LevelName;
				__result = (scene == "D08Z03S01" || scene == "D08Z03S03") && __instance.GetFlag("ITEM_QI110");
			}
		}
	}

	// Don't allow certain npc's death flags to be set
	[HarmonyPatch(typeof(FlagModification), "OnEnter")]
	public class FlagModification_Patch
	{
		public static bool Prefix(FlagModification __instance)
		{
			if (!ItemFlags.bannedFlags.Contains(__instance.flagName.Value) || Core.LevelManager.currentLevel.LevelName == "D07Z01S01")
            {
				Core.Events.SetFlag(__instance.flagName.Value, __instance.state.Value, false);
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
			if (scene == "D02Z01S01" && (text == "D01Z06S01_BOSSDEAD" || text == "D03Z04S01_BOSSDEAD" || text == "D08Z01S01_BOSSDEAD"))
			{
				flag = false;
			}
			// Gemino - Dont require convent boss to be defeated if already have filled thimble
			if (scene == "D02Z01S01" && text == "D02Z05S01_BOSSDEAD")
            {
				flag = Core.InventoryManager.IsQuestItemOwned("QI57");
            }
			// Viridiana reward
			if (text == "VIRIDIANA_REWARD_COMPLETED" && (scene == "D01Z04S18" || scene == "D02Z03S20" || scene == "D03Z03S15" || scene == "D04Z02S22" || scene == "D05Z02S14"))
			{
				flag = Core.Events.GetFlag("LOCATION_PR08");
			}
			// Rooftops - If both cleofas & Jibrael present prioritize Jibrael
			if (scene == "D06Z01S18" && text == "CLEOFAS_BURYING")
            {
				flag = flag && (!Core.Events.GetFlag("SANTOS_FIRSTCONVERSATION_DONE") || Core.Events.GetFlag("SANTOS_AMANECIDA_LOCATION3_ACTIVATED") || Core.Events.GetFlag("SANTOS_AMANECIDA_FACCATA_DEFEATED"));
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
			if (text == "GEMINO_RB10_REWARD")
			{
				if (scene == "D02Z01S01")
					flag = Core.Events.GetFlag("LOCATION_RB10");
				else if (scene == "D02Z01S04")
					flag = true;
			}
			// Gemino flowers
			if (text == "GEMINO_OFFERING_DONE")
			{
				if (scene == "D02Z01S01")
					flag = true;
				else if (scene == "D02Z01S04")
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
				flag = Core.Events.GetFlag("D08Z01S01_BOSSDEAD");
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
			// Final amanecida
			if ((scene == "D02Z02S14" || scene == "D03Z01S03" || scene == "D04Z01S04" || scene == "D09Z01S01") && text == "SANTOS_LAUDES_ACTIVATED")
            {
				flag = Core.Events.GetFlag("LOCATION_QI110");
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

			// Cistern shroud puzzle
			if (scene == "D01Z05S21")
            {
				bool completePuzzle = Core.InventoryManager.IsRelicEquipped("RE04");
				if (text == "D01Z05S21_LEFTSHOW4" || text == "D01Z05S21_RIGHTSHOW2")
					flag = completePuzzle;
				else if (text == "D01Z05S21_LEFTSHOWNONE" || text == "D01Z05S21_LEFTSHOW1" || text == "D01Z05S21_LEFTSHOW2" || text == "D01Z05S21_LEFTSHOW3" ||
					text == "D01Z05S21_RIGHTSHOWNONE" || text == "D01Z05S21_RIGHTSHOW1")
					flag = !completePuzzle;
            }

			// Allows open albero warp room
			if (scene == "D01Z02S06" && text == "D01Z02S07_TELEPORT_ALBERO")
            {
				flag = true;
            }

			// Mothers mask room
			if (scene == "D04Z02S15" && text == "D04Z02S17_ARCHDEACON1ITEMTAKEN")
            {
				flag = Core.Events.GetFlag("LOCATION_QI60");
            }

			// Make sure that certain gates are always open regardless of holy wound or boss status
			if (scene == "D01Z05S19" && text == "D01Z06S01_BOSSDEAD" || scene == "D01Z05S25" && text == "D03Z04S01_BOSSDEAD" || scene == "D04Z02S22" && text == "D04Z04S01_DREAMVISITED" ||
				scene == "D03Z03S11" && (text == "D03Z04S01_BOSSDEAD" || text == "CONTRITION_ALTAR_DONE") || scene == "D03Z03S15" && text == "CONTRITION_ALTAR_DONE")
			{
				flag = true;
			}

			if (Main.Randomizer.gameConfig.DoorShuffleType > 0 && (scene == "D08Z01S02" && text == "D08Z01S02_FACE_BROKEN" || scene == "D05Z01S02" && text == "D05Z01S02_PASSAGEUNVEILED"))
            {
				//, mountains, petrous
				flag = true;
			}

			// Boss shuffle disabling
			//if (scene == "D17Z01S11" && text == "D17Z01_BOSSDEAD")
			//{
			//	flag = true;
			//}

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
			if (scene.Contains("D19") || ItemFlags.thornScenes.Contains(scene))
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
			// Gemino thimbles
			if (scene == "D02Z01S01" && item == "QI57")
			{
				__result = Core.Events.GetFlag("LOCATION_QI59");
				return false;
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
					__result = !Core.Events.GetFlag("LOCATION_RB18") && Core.Events.GetFlag("ITEM_RB17");
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
					__result = !Core.Events.GetFlag("LOCATION_RB25") && Core.Events.GetFlag("ITEM_RB24");
					return false;
				}
				if (item == "RB25")
				{
					__result = Core.Events.GetFlag("LOCATION_RB25");
					return false;
				}
			}
			// Penitence bead rewards
			if (scene == "D07Z01S04" && item == "RB101" || item == "RB102" || item == "RB103")
            {
				__result = true;
				return false;
            }
			// Disable the boundaries in holy wound boss rooms
			if (scene == "D01Z04S18" && item == "QI38" || scene == "D02Z03S20" && item == "QI40")
            {
				__result = true;
				return false;
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
				if (ItemFlags.itemsToNotRemove.Contains(objectIdStting))
				{
					__result = true;
					return false;
				}
				return true;
			}
		}
	}
}
