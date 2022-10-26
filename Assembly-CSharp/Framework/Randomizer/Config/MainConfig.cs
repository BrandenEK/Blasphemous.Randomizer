using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class MainConfig
	{
		public MainConfig(string ver, GeneralConfig generalConfig, ItemConfig itemConfig, EnemyConfig enemyConfig, PrayerConfig prayerConfig, RoomConfig roomConfig, DebugConfig debugConfig)
		{
			this.versionCreated = ver;
			this.general = generalConfig;
			this.items = itemConfig;
			this.enemies = enemyConfig;
			this.prayers = prayerConfig;
			this.rooms = roomConfig;
			this.debug = debugConfig;
		}

		public string versionCreated;

		public GeneralConfig general;

		public ItemConfig items;

		public EnemyConfig enemies;

		public PrayerConfig prayers;

		public RoomConfig rooms;

		public DebugConfig debug;
	}
}
