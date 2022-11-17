using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlasphemousRandomizer.Structures
{
    public class DoorLocation
    {
        // Permanent data
        public string id;
        public int direction;
        public int type;
        public string newDoors;

        // Temporary data
        public string targetDoor;
    }
}
