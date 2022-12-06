using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class ItemConfig
	{
		public ItemConfig(int type, bool lung, bool death, bool wheel)
		{
			this.type = type;
			lungDamage = lung;
			disableNPCDeath = death;
			startWithWheel = wheel;
		}

		public int type;
		public bool lungDamage;
		public bool disableNPCDeath;
		public bool startWithWheel;
	}
}
