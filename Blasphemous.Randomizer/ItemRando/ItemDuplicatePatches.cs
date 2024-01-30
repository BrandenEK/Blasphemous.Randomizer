using Framework.Managers;
using HarmonyLib;
using Tools.Playmaker2.Action;
using Tools.Playmaker2.Condition;

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
			// Get data about this flag check
			string flagName = __instance.flagName.Value.ToUpper().Replace(' ', '_');
			string objectName = __instance.Owner.name;
			string sceneName = Core.LevelManager.currentLevel.LevelName;

			bool flag = Core.Events.GetFlag(flagName);
			Main.Randomizer.Log($"Checking for flag: {flagName} ({objectName})");

			// Albero

			if (sceneName == "D01Z02S02" && flagName == "D09Z01S03_BOSSDEAD" && (!Core.Events.GetFlag("TIRSO_QI19_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI20_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI37_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI63_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI64_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI65_DELIVERED")))
			{
				// Sick house - Only give final reward once all herbs are delivered
				flag = false;
			}
			if (sceneName == "D01Z02S06" && flagName == "D01Z02S07_TELEPORT_ALBERO")
			{
				// Allows open albero warp room
				flag = true;
			}

			// Mercy

			if (sceneName == "D01BZ02S01" && flagName == "QI58_USED")
			{
				// Shop item
				flag = Core.Events.GetFlag("LOCATION_QI58");
			}

			// Cistern

			if (sceneName == "D01Z05S19" && flagName == "D01Z06S01_BOSSDEAD")
            {
				// Always open piedad visage door
				flag = true;
            }
			if (sceneName == "D01Z05S25" && flagName == "D03Z04S01_BOSSDEAD")
			{
				// Always open angustias visage door
				flag = true;
			}
			if (sceneName == "D01Z05S21")
			{
				// Shroud puzzle
				bool completePuzzle = Core.InventoryManager.IsRelicEquipped("RE04");
				if (flagName == "D01Z05S21_LEFTSHOW4" || flagName == "D01Z05S21_RIGHTSHOW2")
                {
					flag = completePuzzle;
                }
				else if (flagName == "D01Z05S21_LEFTSHOWNONE" || flagName == "D01Z05S21_LEFTSHOW1" || flagName == "D01Z05S21_LEFTSHOW2" ||
						 flagName == "D01Z05S21_LEFTSHOW3" || flagName == "D01Z05S21_RIGHTSHOWNONE" || flagName == "D01Z05S21_RIGHTSHOW1")
                {
					flag = !completePuzzle;
                }
			}

			// Olive Trees

			if (sceneName == "D02Z01S01")
            {
				if (flagName == "D02Z05S01_BOSSDEAD")
                {
					// Gemino - Dont require convent boss to be defeated if already have filled thimble
					flag = Core.InventoryManager.IsQuestItemOwned("QI57");
				}
				else if (flagName == "D01Z06S01_BOSSDEAD" || flagName == "D03Z04S01_BOSSDEAD" || flagName == "D08Z01S01_BOSSDEAD")
                {
					// Gemino - Don't freeze him even when bosses are dead
					flag = false;
				}
				else if (flagName == "GEMINO_RB10_REWARD")
                {
					// Spawn Frozen Olive in tree scene, not cave scene
					flag = Core.Events.GetFlag("LOCATION_RB10");
				}
				else if (flagName == "GEMINO_OFFERING_DONE")
                {
					// Spawn Dried Flowers in cave scene, not tree scene
					flag = true;
				}
			}
			if (sceneName == "D02Z01S04")
            {
				if (flagName == "GEMINO_RB10_REWARD")
                {
					// Spawn Frozen Olive in tree scene, not cave scene
					flag = true;
				}
				else if (flagName == "GEMINO_OFFERING_DONE")
                {
					// Spawn Dried Flowers in cave scene, not tree scene
					flag = Core.Events.GetFlag("LOCATION_QI68");
				}
			}

			// Graveyard

			if (sceneName == "D02BZ02S01" && flagName == "QI11_USED")
			{
				// Shop item
				flag = Core.Events.GetFlag("LOCATION_QI11");
			}

			// Mountains

			if (sceneName == "D03Z01S06" && flagName == "BROTHERS_EVENTPERPETVA_COMPLETED")
			{
				// Perpetua is always there until you actually defeat her
				flag = Core.Events.GetFlag("BROTHERS_PERPETUA_DEFEATED");
			}

			// Grievance

			if (sceneName == "D03Z03S10" && flagName == "ALTASGRACIAS_LAST_REWARD")
			{
				// Make Altasgracias cacoon check location flag instead
				flag = Core.Events.GetFlag("LOCATION_RB06");
			}
			if (sceneName == "D03Z03S11" && (flagName == "D03Z04S01_BOSSDEAD" || flagName == "CONTRITION_ALTAR_DONE"))
            {
				// Always try to load into finished boss room
				flag = true;
            }
			if (sceneName == "D03Z03S15" && flagName == "CONTRITION_ALTAR_DONE")
            {
				// Always try to load into finished boss room
				flag = true;
			}

			// Patio

			if (sceneName == "D04Z01S04" && flagName == "REDENTO_PATH4_UNLOCKED")
			{
				// If the Amanecida has been activated here auto clear path for Redento
				flag = flag || Core.Events.GetFlag("SANTOS_AMANECIDA_LOCATION3_ACTIVATED") || Core.Events.GetFlag("SANTOS_AMANECIDA_FACCATA_DEFEATED");
			}

			// Mothers

			if (sceneName == "D04BZ02S01" && flagName == "REDENTO_QI54_USED")
			{
				// Redento corpse will be removed once you obtain the item
				flag = Core.Events.GetFlag("LOCATION_QI54");
			}
			if (sceneName == "D04Z02S15" && flagName == "D04Z02S17_ARCHDEACON1ITEMTAKEN")
			{
				// Remove mask item after the location is picked up (Not necessary because of next if statement)
				flag = Core.Events.GetFlag("LOCATION_QI60");
			}
			if (sceneName == "D04Z02S22" && flagName == "D04Z04S01_DREAMVISITED")
            {
				// Never enter the dream in mask room
				flag = true;
            }
			if (sceneName == "D04Z03S02" && flagName == "D06Z01S25_BOSSDEAD")
			{
				// Fourth Visage will always give item, even after Crisanta is defeated
				flag = false;
			}

			// Canvases

			if (sceneName == "D05BZ02S01")
			{
				// Shop items
				if (flagName == "QI49_USED")
                {
					flag = Core.Events.GetFlag("LOCATION_QI49");
				}
				else if (flagName == "QI71_USED")
                {
					flag = Core.Events.GetFlag("LOCATION_QI71");
				}
			}

			// Rooftops

			if (sceneName == "D06Z01S01" && (flagName == "D04Z02S17_ARCHDEACON1ITEMTAKEN" || flagName == "D05Z01S15_ARCHDEACON2ITEMTAKEN" || flagName == "D02Z03S19_ARCHDEACON3ITEMTAKEN"))
			{
				// Elevator - Only unlock new floors when a mask is placed
				flag = false;
			}
			if (sceneName == "D06Z01S18" && flagName == "CLEOFAS_BURYING")
			{
				// If both cleofas & Jibrael are present prioritize Jibrael
				flag = flag && (!Core.Events.GetFlag("SANTOS_FIRSTCONVERSATION_DONE") || Core.Events.GetFlag("SANTOS_AMANECIDA_LOCATION3_ACTIVATED") || Core.Events.GetFlag("SANTOS_AMANECIDA_FACCATA_DEFEATED"));
			}

			// Bridge

			if (sceneName == "D08Z01S01")
			{
				// Make Esdras only appear after the 3 wounds are obtained
				if (flagName == "D01Z06S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI38");
				}
				else if (flagName == "D02Z05S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI39");
				}
				else if (flagName == "D03Z04S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI40");
				}
			}

			// Brotherhood

			if (sceneName == "D17Z01S14" && (flagName == "ESDRAS_CHAPEL" || flagName == "D06Z01S25_BOSSDEAD"))
			{
				// Open church once the scapular is obtained
				flag = Core.InventoryManager.IsQuestItemOwned("QI203");
			}
			if (sceneName == "D17Z01S15")
			{
				if (flagName == "ESDRAS_CHAPEL")
				{
					// Always spawn Esdras if he has been defeated
					flag = Core.Events.GetFlag("D08Z01S01_BOSSDEAD");
				}
				else if (flagName == "D06Z01S25_BOSSDEAD")
				{
					// Make Esdras give item even if Crisanta is defeated already
					if (objectName == "EsdrasNPC")
						flag = Core.Events.GetFlag("LOCATION_QI204");
				}
				else if (flagName == "CRISANTA_LIBERATED")
				{
					// Make Crisanta give item as long as you have the true heart
					flag = Core.Events.GetFlag("D06Z01S25_BOSSDEAD") && Core.InventoryManager.IsSwordOwned("HE201");
				}
			}

			// Resting Place

			if (sceneName == "D20Z03S01" && flagName == "D08Z01S01_BOSSDEAD")
			{
				// Allow Perpetua to give item even if Esdras is already defeated
				flag = false;
			}

			// Various

			if (flagName == "VIRIDIANA_REWARD_COMPLETED" && (sceneName == "D01Z04S18" || sceneName == "D02Z03S20" || sceneName == "D03Z03S15" || sceneName == "D04Z02S22" || sceneName == "D05Z02S14"))
			{
				// Make Viridiana reward use location flag instead (Not necessary with disable NPC death always on now)
				flag = Core.Events.GetFlag("LOCATION_PR08");
			}

			if (flagName == "SANTOS_LAUDES_ACTIVATED" && (sceneName == "D02Z02S14" || sceneName == "D03Z01S03" || sceneName == "D04Z01S04" || sceneName == "D09Z01S01"))
			{
				// Give final Amanecida item if all 4 have been defeated
				flag = Core.Events.GetFlag("LOCATION_QI110");
			}

            if (Main.Randomizer.GameSettings.DoorShuffleType > 0 && (sceneName == "D05Z01S02" && flagName == "D05Z01S02_PASSAGEUNVEILED" || sceneName == "D03Z01S01" && flagName == "D03Z01S01_PASSAGEUNVEILED"))
            {
				// In door shuffle, change one way passageways to use custom flag
				flag = Core.Events.GetFlag("HIDDEN_WALL_" + sceneName);
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
			// Get data about this item check
			string itemName = objectIdStting;
			string sceneName = Core.LevelManager.currentLevel.LevelName;
			Main.Randomizer.Log("Checking for item owned: " + itemName);

			// If any standard scene-item pair
			for (int i = 0; i < ItemFlags.duplicateScenes.Length; i++)
			{
				if (sceneName == ItemFlags.duplicateScenes[i] && itemName == ItemFlags.duplicateItems[i])
				{
					__result = Core.Events.GetFlag("LOCATION_" + itemName);
					return false;
				}
			}

			// Holy Line

			if (sceneName == "D01Z01S07" && (itemName == "QI38" || itemName == "QI39" || itemName == "QI40"))
			{
				// Remove Deosgracias once his location is collected
				__result = Core.Events.GetFlag("LOCATION_QI31");
				return false;
			}

			// Albero

			if (itemName == "QI66")
			{
				// Only allow collecting Tirso's first reward from sick house, not graveyard
				if (sceneName == "D01Z02S02")
				{
					__result = false;
					return false;
				}
				else if (sceneName == "D01Z02S05")
				{
					__result = true;
					return false;
				}
			}

			// Mercy

			if (sceneName == "D01Z04S18" && itemName == "QI38")
            {
				// Disable the piedad invisible wall
				__result = true;
				return false;
			}

			// Olive Trees

			if (sceneName == "D02Z01S01" && itemName == "QI57")
			{
				// Gemino - Only allow collecting second reward after the first one
				__result = Core.Events.GetFlag("LOCATION_QI59");
				return false;
			}

			// Convent

			if (sceneName == "D02Z03S20" && itemName == "QI40")
            {
				// Disable the charred lady invisible wall
				__result = true;
				return false;
			}

			// Deambulatory

			if (sceneName == "D07Z01S04" && itemName == "RB101" || itemName == "RB102" || itemName == "RB103")
			{
				// Never give the penitence bead rewards
				__result = true;
				return false;
			}

			// Bridge

			if (sceneName == "D08Z01S01" && itemName == "QI203")
			{
				// Don't allow the scapular to skip the Esdras fight
				__result = false;
				return false;
			}

			// Various

			if (sceneName == "D01Z04S08" && "RB17RB18RB19".Contains(itemName))
			{
				// First red candle
				__result = Core.Events.GetFlag("LOCATION_RB17");
				return false;
			}
			if (sceneName == "D02Z03S06" || sceneName == "D05Z01S02")
			{
				// Red candle upgrades
				if (itemName == "RB17")
				{
					__result = !Core.Events.GetFlag("LOCATION_RB18") && Core.Events.GetFlag("ITEM_RB17");
					return false;
				}
				if (itemName == "RB18")
				{
					__result = Core.Events.GetFlag("LOCATION_RB18");
					return false;
				}
			}

			if (sceneName == "D02Z03S17" && "RB24RB25RB26".Contains(itemName))
			{
				// First blue candle
				__result = Core.Events.GetFlag("LOCATION_RB24");
				return false;
			}
			if (sceneName == "D17Z01S04" || sceneName == "D01Z04S16")
			{
				// Blue candle upgrades
				if (itemName == "RB24")
				{
					__result = !Core.Events.GetFlag("LOCATION_RB25") && Core.Events.GetFlag("ITEM_RB24");
					return false;
				}
				if (itemName == "RB25")
				{
					__result = Core.Events.GetFlag("LOCATION_RB25");
					return false;
				}
			}

			if ((sceneName == "D02Z02S14" || sceneName == "D03Z01S03" || sceneName == "D04Z01S04" || sceneName == "D09Z01S01" || sceneName.Contains("D21"))
				&& (itemName == "PR101" || itemName == "QI107" || itemName == "QI108" || itemName == "QI109" || itemName == "QI110"))
            {
				// Amanecidas check location flag instead of owning verses
				__result = Core.Events.GetFlag("LOCATION_" + itemName);
				return false;
            }

			if (sceneName.Contains("D19") || ItemFlags.thornScenes.Contains(sceneName))
			{
				// Guilt arenas check location flag instead of owning thorns
				if (itemName == "QI31")
				{
					__result = !Core.Events.GetFlag("LOCATION_QI32");
					return false;
				}
				if (itemName == "QI32")
				{
					__result = Core.Events.GetFlag("LOCATION_QI32") && !Core.Events.GetFlag("LOCATION_QI33");
					return false;
				}
				if (itemName == "QI33")
				{
					__result = Core.Events.GetFlag("LOCATION_QI33") && !Core.Events.GetFlag("LOCATION_QI34");
					return false;
				}
				if (itemName == "QI34")
				{
					__result = Core.Events.GetFlag("LOCATION_QI34") && !Core.Events.GetFlag("LOCATION_QI35");
					return false;
				}
				if (itemName == "QI35")
				{
					__result = Core.Events.GetFlag("LOCATION_QI35") && !Core.Events.GetFlag("LOCATION_QI79");
					return false;
				}
				if (itemName == "QI79")
				{
					__result = Core.Events.GetFlag("LOCATION_QI79") && !Core.Events.GetFlag("LOCATION_QI80");
					return false;
				}
				if (itemName == "QI80")
				{
					__result = Core.Events.GetFlag("LOCATION_QI80") && !Core.Events.GetFlag("LOCATION_QI81");
					return false;
				}
				if (itemName == "QI81")
				{
					__result = Core.Events.GetFlag("LOCATION_QI81");
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
