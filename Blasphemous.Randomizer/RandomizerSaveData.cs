using Blasphemous.ModdingAPI.Persistence;
using Blasphemous.Randomizer.Zones;
using System.Collections.Generic;

namespace Blasphemous.Randomizer;

public class RandomizerSaveData : SaveData
{
    public RandomizerSaveData() : base("ID_RANDOMIZER") { }

    public int seed;
    public RandomizerSettings settings;

    public Dictionary<string, string> mappedItems;
    public Dictionary<string, string> mappedDoors;
    public Dictionary<string, string> mappedHints;
    public Dictionary<string, string> mappedEnemies;

    public Dictionary<string, ZoneCollection> collectionStatus;
}
