using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;

namespace BlasphemousRandomizer.Shufflers
{
    public class DoorShuffle : IShuffle
    {
        private Dictionary<string, string> newDoors;
        private DoorFiller filler;

        public void Init()
        {
            filler = new DoorFiller();
        }

        public void Reset()
        {
            newDoors = null;
        }

        public void Shuffle(int seed)
        {
            // Shuffle doors
        }
    }
}
