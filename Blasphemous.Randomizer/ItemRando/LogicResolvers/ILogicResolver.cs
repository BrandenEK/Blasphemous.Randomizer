using Blasphemous.Randomizer.DoorRando;

namespace Blasphemous.Randomizer.ItemRando.LogicResolvers;

/// <summary>
/// Used to determine whether a logic token is a door or item location
/// </summary>
public interface ILogicResolver
{
    /// <summary>
    /// Checks whether the token is a door
    /// </summary>
    public bool IsDoor(string id);

    /// <summary>
    /// Returns the door based on the id
    /// </summary>
    public DoorLocation GetDoor(string id);

    /// <summary>
    /// Checks whether the token is an item
    /// </summary>
    public bool IsItem(string id);

    /// <summary>
    /// Returns the item based on the id
    /// </summary>
    public Item GetItem(string id);

    /// <summary>
    /// Checks whether the token is an item locations
    /// </summary>
    public bool IsItemLocation(string id);

    /// <summary>
    /// Returns the item location based on the id
    /// </summary>
    public ItemLocation GetItemLocation(string id);
}
