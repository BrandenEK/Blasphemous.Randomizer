using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;

namespace BlasphemousRandomizer.Shufflers
{
    public class HintShuffle : IShuffle
    {
        private Dictionary<string, string> newHints;
        private HintFiller filler;

        public void Init()
        {
            filler = new HintFiller();
        }

        public void Reset()
        {
            newHints = null;
        }

        public void Shuffle(int seed)
        {
            //Shuffle hints
        }
    }
}
