using System;
using Framework.Managers;
using Framework.Randomizer.Config;

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

		public MainConfig config;
	}
}
