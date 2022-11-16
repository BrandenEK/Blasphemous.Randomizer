using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Shufflers
{
    public class ItemShuffle
    {
        private Dictionary<string, Item> newItems;

        public void giveItem(string locationId)
        {
            Main.Randomizer.Log("Giving item from location: " + locationId);
        }

    }
}
