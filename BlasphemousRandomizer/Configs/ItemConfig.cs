using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class ItemConfig
	{
		public ItemConfig(int type, bool lung, bool death, string[] locations)
		{
			this.type = type;
			lungDamage = lung;
			disableNPCDeath = death;
			randomizedLocations = ((locations == null) ? new string[0] : locations);
		}

		public int type;
		public bool lungDamage;
		public string[] randomizedLocations;
		public bool disableNPCDeath;
	}
}
