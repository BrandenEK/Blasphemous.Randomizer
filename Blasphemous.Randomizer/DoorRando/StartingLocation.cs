using UnityEngine;

namespace Blasphemous.Randomizer.DoorRando
{
    public class StartingLocation
    {
        public string Room { get; }
        public string Door { get; }
        public Vector3 Position { get; }
        public bool FacingRight { get; }

        public StartingLocation(string room, string door, Vector3 position, bool facingRight)
        {
            Room = room;
            Door = door;
            Position = position;
            FacingRight = facingRight;
        }
    }
}
