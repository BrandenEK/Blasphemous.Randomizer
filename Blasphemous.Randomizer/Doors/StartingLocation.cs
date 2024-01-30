using UnityEngine;

namespace Blasphemous.Randomizer.Doors;

public class StartingLocation(string room, string door, Vector3 position, bool facingRight)
{
    public string Room { get; } = room;
    public string Door { get; } = door;
    public Vector3 Position { get; } = position;
    public bool FacingRight { get; } = facingRight;
}
