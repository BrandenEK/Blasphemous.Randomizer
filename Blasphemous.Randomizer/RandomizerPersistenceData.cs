using Blasphemous.ModdingAPI.Persistence;
using Blasphemous.Randomizer.Map;
using System;
using System.Collections.Generic;

namespace Blasphemous.Randomizer
{
    [Serializable]
    public class RandomizerPersistenceData : SaveData
    {
        public RandomizerPersistenceData() : base("ID_RANDOMIZER") { }

        public Config config;

        public Dictionary<string, string> mappedItems;
        public Dictionary<string, string> mappedDoors;
        public Dictionary<string, string> mappedHints;
        public Dictionary<string, string> mappedEnemies;

        public Dictionary<string, ZoneCollection> collectionStatus;
    }
}
