using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class ItemConfig
	{
		public ItemConfig(int type, bool lung, bool death, bool wheel, bool reliq)
		{
			this.type = type;
			lungDamage = lung;
			disableNPCDeath = death;
			startWithWheel = wheel;
			shuffleReliquaries = reliq;
		}

		public int type;
		public bool lungDamage;
		public bool disableNPCDeath;
		public bool startWithWheel;

		public bool shuffleReliquaries;
	}
}
