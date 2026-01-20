using Blasphemous.ModdingAPI.Persistence;
using Blasphemous.Randomizer.Map;
using System.Collections.Generic;

namespace Blasphemous.Randomizer
{
    public class RandoSlotData : SlotSaveData
    {
        public Config config;

        public Dictionary<string, string> mappedItems;
        public Dictionary<string, string> mappedDoors;
        public Dictionary<string, string> mappedHints;
        public Dictionary<string, string> mappedEnemies;

        public Dictionary<string, ZoneCollection> collectionStatus;
    }
}
