using System;
using BlasphemousRandomizer.Config;
using ModdingAPI;

namespace BlasphemousRandomizer
{
    [Serializable]
    public class RandomizerPersistenceData : ModPersistentData
    {
        public RandomizerPersistenceData() : base("ID_RANDOMIZER") { }

        public int seed;
        public int itemsCollected;
        public bool startedInRando;
        public MainConfig config;
    }
}
