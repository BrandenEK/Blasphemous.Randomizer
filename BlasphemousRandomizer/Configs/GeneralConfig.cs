using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class GeneralConfig
	{
		public GeneralConfig(bool tele, bool skip, bool hints, bool pen, bool hard, int seed)
		{
			teleportationAlwaysUnlocked = tele;
			skipCutscenes = skip;
			allowHints = hints;
			enablePenitence = pen;
			hardMode = hard;
			customSeed = seed;
		}

		public bool teleportationAlwaysUnlocked;
		public bool skipCutscenes;
		public bool allowHints;
		public bool enablePenitence;
		public bool hardMode;
		public int customSeed;
	}
}
