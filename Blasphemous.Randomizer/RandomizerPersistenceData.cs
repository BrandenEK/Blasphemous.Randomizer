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
        public Config config;

        public Dictionary<string, string> mappedItems;
        public Dictionary<string, string> mappedDoors;
        public Dictionary<string, string> mappedHints;
        public Dictionary<string, string> mappedEnemies;

        public Dictionary<string, ZoneCollection> collectionStatus;
    }
}
