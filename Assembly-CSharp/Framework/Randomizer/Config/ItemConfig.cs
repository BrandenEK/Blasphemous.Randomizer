using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class ItemConfig
	{
		public ItemConfig(int type, bool lung, string[] locations)
		{
			this.type = type;
			this.lungDamage = lung;
			this.randomizedLocations = ((locations == null) ? new string[0] : locations);
		}

		public int type;

		public bool lungDamage;

		public string[] randomizedLocations;
	}
}
