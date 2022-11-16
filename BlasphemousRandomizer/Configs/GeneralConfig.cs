using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class GeneralConfig
	{
		public GeneralConfig(bool tele, bool skip, bool pen, bool hard, int seed)
		{
			teleportationAlwaysUnlocked = tele;
			skipCutscenes = skip;
			enablePenitence = pen;
			hardMode = hard;
			customSeed = seed;
		}

		public bool teleportationAlwaysUnlocked;
		public bool skipCutscenes;
		public bool enablePenitence;
		public bool hardMode;
		public int customSeed;
	}
}
