using System;
using Framework.Managers;

namespace Framework.Randomizer
{
	[Serializable]
	public class RandomizerPersistenceData : PersistentManager.PersistentData
	{
		public RandomizerPersistenceData() : base("ID_RANDOMIZER")
		{
		}

		public int seed;

		public int itemsCollected;

		public bool startedInRando;

		public Config randomizerSettings;
	}
}
