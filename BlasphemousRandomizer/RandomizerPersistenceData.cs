using System;
using ModdingAPI;
using System.Collections.Generic;
using BlasphemousRandomizer.Map;

namespace BlasphemousRandomizer
{
    [Serializable]
    public class RandomizerPersistenceData : ModPersistentData
    {
        public RandomizerPersistenceData() : base("ID_RANDOMIZER") { }

        public int seed;
        public int itemsCollected;
        public bool startedInRando;
        public Config config;

        public Dictionary<string, ZoneCollection> collectionStatus;
    }
}
