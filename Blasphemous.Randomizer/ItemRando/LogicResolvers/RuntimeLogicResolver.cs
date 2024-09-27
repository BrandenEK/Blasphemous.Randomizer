using Blasphemous.Randomizer.DoorRando;

namespace Blasphemous.Randomizer.ItemRando.LogicResolvers;

internal class RuntimeLogicResolver : ILogicResolver
{
    public DoorLocation GetDoor(string id)
    {
        return Main.Randomizer.data.doorLocations[id];
    }

    public Item GetItem(string id)
    {
        return Main.Randomizer.data.items[id];
    }

    public ItemLocation GetItemLocation(string id)
    {
        return Main.Randomizer.data.itemLocations[id];
    }

    public bool IsDoor(string id)
    {
        return Main.Randomizer.data.doorLocations.ContainsKey(id);
    }

    public bool IsItem(string id)
    {
        return Main.Randomizer.data.items.ContainsKey(id);
    }

    public bool IsItemLocation(string id)
    {
        return Main.Randomizer.data.itemLocations.ContainsKey(id);
    }
}
