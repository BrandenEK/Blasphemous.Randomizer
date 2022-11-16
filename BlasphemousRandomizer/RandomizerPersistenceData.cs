using System;
using Framework.Managers;
using BlasphemousRandomizer.Config;

namespace BlasphemousRandomizer
{
    [Serializable]
    public class RandomizerPersistenceData : PersistentManager.PersistentData
    {
        public RandomizerPersistenceData() : base("ID_RANDOMIZER") { }

        public int seed;
        public int itemsCollected;
        public bool startedInRando;
        public MainConfig config;
    }
}
