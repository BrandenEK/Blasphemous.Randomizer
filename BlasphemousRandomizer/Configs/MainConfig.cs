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
			string[] random = new string[] { "item","cherub","lady","oil","sword","blessing","guiltArena","tirso","miriam","redento","jocinero","altasgracias","tentudia","gemino","guiltBead","ossuary","boss","visage","mask","herb","church","shop","thorn","candle","viridiana" };
			return new MainConfig(MyPluginInfo.PLUGIN_VERSION,
				new GeneralConfig(true, true, false, false, 0),
				new ItemConfig(1, true, true, random),
				new EnemyConfig(1, true),
				new PrayerConfig(1, false),
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
