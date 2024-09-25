using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.ItemRando;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.Tests;

internal class TestInventory : BlasphemousInventory
{
    public TestInventory(Dictionary<string, ItemLocation> itemLocations, Dictionary<string, DoorLocation> doors)
    {
        _logicResolver = new TestLogicResolver(itemLocations, doors);
    }
}
