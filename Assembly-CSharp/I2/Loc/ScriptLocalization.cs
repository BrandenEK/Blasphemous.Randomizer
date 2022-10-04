using System;
using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{
		public static string Get(string Term, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
		{
			return LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage);
		}

		public static class UI
		{
			public static string GET_GUILTDROP_TEXT
			{
				get
				{
					return ScriptLocalization.Get("UI/GET_GUILTDROP_TEXT", true, 0, true, false, null, null);
				}
			}

			public static string ENABLED_TEXT
			{
				get
				{
					return ScriptLocalization.Get("UI/ENABLED_TEXT", true, 0, true, false, null, null);
				}
			}

			public static string DISABLED_TEXT
			{
				get
				{
					return ScriptLocalization.Get("UI/DISABLED_TEXT", true, 0, true, false, null, null);
				}
			}

			public static string ISIDORA_MENU_FORBIDDEN
			{
				get
				{
					return ScriptLocalization.Get("UI/ISIDORA_MENU_FORBIDDEN", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_BossRush
		{
			public static string COURSE_A_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_A_1", true, 0, true, false, null, null);
				}
			}

			public static string COURSE_A_2
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_A_2", true, 0, true, false, null, null);
				}
			}

			public static string COURSE_A_3
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_A_3", true, 0, true, false, null, null);
				}
			}

			public static string COURSE_B_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_B_1", true, 0, true, false, null, null);
				}
			}

			public static string COURSE_C_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_C_1", true, 0, true, false, null, null);
				}
			}

			public static string COURSE_D_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COURSE_D_1", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_BESTTIME
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/TEXT_BESTTIME", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_HARD_UNLOCKED
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/TEXT_HARD_UNLOCKED", true, 0, true, false, null, null);
				}
			}

			public static string COMPLETED_SUFFIX
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/COMPLETED_SUFFIX", true, 0, true, false, null, null);
				}
			}

			public static string FAILEDED_SUFFIX
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/FAILEDED_SUFFIX", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_UNLOCK_COURSE_A_2
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/LABEL_UNLOCK_COURSE_A_2", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_UNLOCK_COURSE_A_3
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/LABEL_UNLOCK_COURSE_A_3", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_UNLOCK_COURSE_B_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/LABEL_UNLOCK_COURSE_B_1", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_UNLOCK_COURSE_C_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/LABEL_UNLOCK_COURSE_C_1", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_UNLOCK_COURSE_D_1
			{
				get
				{
					return ScriptLocalization.Get("UI_BossRush/LABEL_UNLOCK_COURSE_D_1", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_Extras
		{
			public static string BACKGROUND_1_LABEL
			{
				get
				{
					return ScriptLocalization.Get("UI_Extras/BACKGROUND_1_LABEL", true, 0, true, false, null, null);
				}
			}

			public static string BACKGROUND_0_LABEL
			{
				get
				{
					return ScriptLocalization.Get("UI_Extras/BACKGROUND_0_LABEL", true, 0, true, false, null, null);
				}
			}

			public static string BACKGROUND_2_LABEL
			{
				get
				{
					return ScriptLocalization.Get("UI_Extras/BACKGROUND_2_LABEL", true, 0, true, false, null, null);
				}
			}

			public static string BACKGROUND_3_LABEL
			{
				get
				{
					return ScriptLocalization.Get("UI_Extras/BACKGROUND_3_LABEL", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_Inventory
		{
			public static string TEXT_DOOR_NEED_OBJECT
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_DOOR_NEED_OBJECT", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_DOOR_USE_OBJECT
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_DOOR_USE_OBJECT", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_DOOR_CLOSED
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_DOOR_CLOSED", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_ITEM_FOUND
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_ITEM_FOUND", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_ITEM_FOUND_BEAD
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_ITEM_FOUND_BEAD", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_ITEM_FOUND_BEADS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_ITEM_FOUND_BEADS", true, 0, true, false, null, null);
				}
			}

			public static string GRID_LABEL_BEADS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/GRID_LABEL_BEADS", true, 0, true, false, null, null);
				}
			}

			public static string GRID_LABEL_QUESTITEM
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/GRID_LABEL_QUESTITEM", true, 0, true, false, null, null);
				}
			}

			public static string GRID_LABEL_RELICS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/GRID_LABEL_RELICS", true, 0, true, false, null, null);
				}
			}

			public static string GRID_LABEL_COLLECTIBLE
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/GRID_LABEL_COLLECTIBLE", true, 0, true, false, null, null);
				}
			}

			public static string MEACULPA_LABEL
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/MEACULPA_LABEL", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_ITEM_GIVE
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_ITEM_GIVE", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QUESTION_GIVE_ITEM
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QUESTION_GIVE_ITEM", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QUESTION_GIVE_PURGE
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QUESTION_GIVE_PURGE", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QUESTION_BUY_ITEM
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QUESTION_BUY_ITEM", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QI75_FILLS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QI75_FILLS", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QI76_FILLS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QI76_FILLS", true, 0, true, false, null, null);
				}
			}

			public static string TEXT_QI76_OR_QI77_UNFILLS
			{
				get
				{
					return ScriptLocalization.Get("UI_Inventory/TEXT_QI76_OR_QI77_UNFILLS", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_Map
		{
			public static string LABEL_BUTTON_APPLY
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_BUTTON_APPLY", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_BUTTON_ACCEPT
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_BUTTON_ACCEPT", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_MENU_VIDEO_SCALE
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_MENU_VIDEO_SCALE", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_MENU_VIDEO_PIXELPERFECT
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_MENU_VIDEO_PIXELPERFECT", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_MENU_VIDEO_WINDOWED
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_MENU_VIDEO_WINDOWED", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_MENU_VIDEO_FULLSCREEN
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_MENU_VIDEO_FULLSCREEN", true, 0, true, false, null, null);
				}
			}

			public static string LABEL_BUTTON_BACK
			{
				get
				{
					return ScriptLocalization.Get("UI_Map/LABEL_BUTTON_BACK", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_Penitences
		{
			public static string PE01_INFO
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE01_INFO", true, 0, true, false, null, null);
				}
			}

			public static string PE02_INFO
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE02_INFO", true, 0, true, false, null, null);
				}
			}

			public static string PE03_INFO
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE03_INFO", true, 0, true, false, null, null);
				}
			}

			public static string NO_PENITENCE_INFO
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/NO_PENITENCE_INFO", true, 0, true, false, null, null);
				}
			}

			public static string PE01_NAME
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE01_NAME", true, 0, true, false, null, null);
				}
			}

			public static string PE02_NAME
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE02_NAME", true, 0, true, false, null, null);
				}
			}

			public static string PE03_NAME
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/PE03_NAME", true, 0, true, false, null, null);
				}
			}

			public static string NO_PENITENCE
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/NO_PENITENCE", true, 0, true, false, null, null);
				}
			}

			public static string CHOOSE_PENITENCE_CONFIRMATION
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/CHOOSE_PENITENCE_CONFIRMATION", true, 0, true, false, null, null);
				}
			}

			public static string CHOOSE_NO_PENITENCE_CONFIRMATION
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/CHOOSE_NO_PENITENCE_CONFIRMATION", true, 0, true, false, null, null);
				}
			}

			public static string CHOOSE_PENITENCE_ABANDON
			{
				get
				{
					return ScriptLocalization.Get("UI_Penitences/CHOOSE_PENITENCE_ABANDON", true, 0, true, false, null, null);
				}
			}
		}

		public static class UI_Slot
		{
			public static string TEXT_SLOT_INFO
			{
				get
				{
					return ScriptLocalization.Get("UI_Slot/TEXT_SLOT_INFO", true, 0, true, false, null, null);
				}
			}
		}
	}
}
