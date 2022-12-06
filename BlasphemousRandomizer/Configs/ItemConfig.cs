using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class ItemConfig
	{
		public ItemConfig(int type, bool lung, bool death, bool wheel, string[] locations)
		{
			this.type = type;
			lungDamage = lung;
			disableNPCDeath = death;
			startWithWheel = wheel;
			randomizedLocations = ((locations == null) ? new string[0] : locations);
		}

		public int type;
		public bool lungDamage;
		public bool disableNPCDeath;
		public bool startWithWheel;
		public string[] randomizedLocations;
	}
}
