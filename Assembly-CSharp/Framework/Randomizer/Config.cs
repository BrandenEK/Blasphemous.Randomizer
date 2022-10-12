using System;
using System.Collections.Generic;

namespace Framework.Randomizer
{
	[Serializable]
	public class Config
	{
		public Config(bool pen, bool hard, bool skip, bool lung, bool tele, int seed, string version, List<string> set)
		{
			this.allowPenitence = pen;
			this.hardMode = hard;
			this.skipCutscenes = skip;
			this.lungDamageAllowed = lung;
			this.unlockTeleportation = tele;
			this.customSeed = seed;
			this.randomizerSettings = set;
			this.versionCreated = version;
			if (set == null)
			{
				this.randomizerSettings = new List<string>();
			}
		}

		public bool allowPenitence;

		public bool hardMode;

		public bool skipCutscenes;

		public bool lungDamageAllowed;

		public bool unlockTeleportation;

		public List<string> randomizerSettings;

		public int customSeed;

		public string versionCreated;
	}
}
