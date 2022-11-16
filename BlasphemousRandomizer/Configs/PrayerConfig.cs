using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class PrayerConfig
	{
		public PrayerConfig(int type, bool remove)
		{
			this.type = type;
			removeMirabis = remove;
		}

		public int type;
		public bool removeMirabis;
	}
}
