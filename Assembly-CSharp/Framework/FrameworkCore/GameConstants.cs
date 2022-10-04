using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Framework.FrameworkCore
{
	public static class GameConstants
	{
		public const string UI_VERSION_NAME = "Steam";

		public const string UI_VERSION_DATE = "October 2021";

		public static Dictionary<string, float> LanguageLineSpacingFactor = new Dictionary<string, float>
		{
			{
				"ja",
				1.2f
			}
		};

		public static Dictionary<string, float> LanguageLineSpacingTextPro = new Dictionary<string, float>
		{
			{
				"ja",
				4f
			}
		};

		public static string DefaultFont = "Majestic";

		public static Dictionary<string, string> FontByLanguages = new Dictionary<string, string>
		{
			{
				"Russian",
				"RussianFont"
			},
			{
				"Chinese",
				"MSJhengHei"
			},
			{
				"Japanese",
				"KH-Dot"
			},
			{
				"Korean",
				"NeoDunggeunmo"
			}
		};

		public const float DOOR_FADEOUT_SECONDS = 0.2f;

		public const float DOOR_FADEIN_SECONDS = 0.6f;

		public const float CAMERA_SET_ELASTICS_SECONDS = 1f;

		public const float EXTRA_CAMERA_SET_ELASTICS_SECONDS = 2f;

		public const int TARGET_FPS = 60;

		public const float TIME_TO_WAIT_TO_SHOW_GUILTCHANGE = 2f;

		public const float SAFE_HORIZONTAL_ADDITION_TO_CHECK = 3.7f;

		public const float TUTORIAL_FADE_IN_TIME = 1f;

		public const float TUTORIAL_FADE_OUT_TIME = 1f;

		public const float TUTORIAL_FACTOR_TAKE_CONTROL_IN_FADE = 0.5f;

		public static readonly Dictionary<PersistentManager.PercentageType, float> PercentageValues = new Dictionary<PersistentManager.PercentageType, float>
		{
			{
				PersistentManager.PercentageType.BossDefeated_1,
				1f
			},
			{
				PersistentManager.PercentageType.BossDefeated_2,
				2f
			},
			{
				PersistentManager.PercentageType.Upgraded,
				0.5f
			},
			{
				PersistentManager.PercentageType.Exploration,
				2f
			},
			{
				PersistentManager.PercentageType.Teleport_A,
				1.5f
			},
			{
				PersistentManager.PercentageType.Teleport_B,
				3f
			},
			{
				PersistentManager.PercentageType.EndingA,
				1.75f
			},
			{
				PersistentManager.PercentageType.ItemAdded,
				0.25f
			},
			{
				PersistentManager.PercentageType.Map,
				38f
			},
			{
				PersistentManager.PercentageType.Map_NgPlus,
				2.5f
			},
			{
				PersistentManager.PercentageType.BossDefeated_NgPlus,
				2f
			},
			{
				PersistentManager.PercentageType.Penitence_NgPlus,
				12.5f
			}
		};

		public const string TEARS_GENERIC_OBJECT = "Inventory/TearObject";

		public const string NEWGAMEPLUS_SCENE = "D17Z01S01";

		public static readonly List<string> DistrictsWithoutName = new List<string>
		{
			"D13",
			"D14",
			"D18",
			"D19",
			"D21",
			"D22",
			"D25",
			"D24"
		};

		public static readonly List<string> FLAGS_ENDINGS_NEED_TO_BE_PERSISTENT = new List<string>
		{
			"_KILLED",
			"_OWNED",
			"_DISPLAYED",
			"_EXECUTED_FOR_AC27",
			"_BOSSDEAD_AC43"
		};

		public static readonly List<string> IGNORE_FLAG_TO_BE_PERSISTENT = new List<string>
		{
			"REDENTO_CAVE_ITEM_OWNED",
			"GEMINO_QI57_OWNED",
			"SOCORRO_STATE_KILLED"
		};
	}
}
