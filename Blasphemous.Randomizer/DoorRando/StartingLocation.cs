using UnityEngine;

namespace Blasphemous.Randomizer.DoorRando;

/// <summary>
/// Represents a possible door that the game can start from
/// </summary>
public class StartingLocation(string room, string door, Vector3 position, bool facingRight, StartFlags flags)
{
    /// <summary>
    /// The room id
    /// </summary>
    public string Room { get; } = room;
    /// <summary>
    /// The door id
    /// </summary>
    public string Door { get; } = door;
    /// <summary>
    /// The xyz coordinates
    /// </summary>
    public Vector3 Position { get; } = position;
    /// <summary>
    /// The facing direction
    /// </summary>
    public bool FacingRight { get; } = facingRight;
    /// <summary>
    /// The flags to determine a valid start
    /// </summary>
    public StartFlags StartFlags { get; } = flags;
}
