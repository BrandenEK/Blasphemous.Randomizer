using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class PrayerConfig
	{
		public PrayerConfig(int type, bool remove)
		{
			this.type = type;
			this.removeMirabis = remove;
		}

		public int type;

		public bool removeMirabis;
	}
}
