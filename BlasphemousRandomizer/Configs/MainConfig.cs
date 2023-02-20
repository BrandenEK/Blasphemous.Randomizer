using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class MainConfig
	{
		public MainConfig(string ver, GeneralConfig generalConfig, ItemConfig itemConfig, EnemyConfig enemyConfig, PrayerConfig prayerConfig, DoorConfig doorConfig, DebugConfig debugConfig)
		{
			versionCreated = ver;
			general = generalConfig;
			items = itemConfig;
			enemies = enemyConfig;
			prayers = prayerConfig;
			doors = doorConfig;
			debug = debugConfig;
		}

		public static MainConfig Default()
        {
			return new MainConfig(Main.MOD_VERSION,
				new GeneralConfig(true, true, true, false, false, 0),
				new ItemConfig(1, false, true, false, true),
				new EnemyConfig(0, true, true),
				new PrayerConfig(0, false),
				new DoorConfig(0),
				new DebugConfig(0));
        }

		public string versionCreated;

		public GeneralConfig general;
		public ItemConfig items;
		public EnemyConfig enemies;
		public PrayerConfig prayers;
		public DoorConfig doors;
		public DebugConfig debug;
	}
}
