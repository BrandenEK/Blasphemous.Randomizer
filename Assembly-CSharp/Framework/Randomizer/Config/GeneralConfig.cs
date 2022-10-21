using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class GeneralConfig
	{
		public GeneralConfig(bool tele, bool skip, bool pen, bool hard, int seed)
		{
			this.teleportationAlwaysUnlocked = tele;
			this.skipCutscenes = skip;
			this.enablePenitence = pen;
			this.hardMode = hard;
			this.customSeed = seed;
		}

		public bool teleportationAlwaysUnlocked;

		public bool skipCutscenes;

		public bool enablePenitence;

		public bool hardMode;

		public int customSeed;
	}
}
