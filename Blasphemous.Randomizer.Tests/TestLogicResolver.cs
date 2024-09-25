using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.ItemRando;
using Blasphemous.Randomizer.ItemRando.LogicResolvers;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.Tests;

internal class TestLogicResolver : ILogicResolver
{
    private readonly Dictionary<string, ItemLocation> _itemLocations;
    private readonly Dictionary<string, DoorLocation> _doors;

    public TestLogicResolver(Dictionary<string, ItemLocation> itemLocations, Dictionary<string, DoorLocation> doors)
    {
        _itemLocations = itemLocations;
        _doors = doors;
    }

    public DoorLocation GetDoor(string id)
    {
        return _doors[id];
    }

    public ItemLocation GetItemLocation(string id)
    {
        return _itemLocations[id];
    }

    public bool IsDoor(string id)
    {
        return _doors.ContainsKey(id);
    }

    public bool IsItemLocation(string id)
    {
        return _itemLocations.ContainsKey(id);
    }
}
