using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class MainConfig
	{
		public MainConfig(string ver, GeneralConfig generalConfig, ItemConfig itemConfig, EnemyConfig enemyConfig, PrayerConfig prayerConfig)
		{
			this.versionCreated = ver;
			this.general = generalConfig;
			this.items = itemConfig;
			this.enemies = enemyConfig;
			this.prayers = prayerConfig;
		}

		public string versionCreated;

		public GeneralConfig general;

		public ItemConfig items;

		public EnemyConfig enemies;

		public PrayerConfig prayers;
	}
}
