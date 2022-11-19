using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;

namespace BlasphemousRandomizer.Shufflers
{
    public class EnemyShuffle : IShuffle
    {
        private Dictionary<string, string> newEnemies;
        private EnemyFiller filler;

        public void Init()
        {
            filler = new EnemyFiller();
        }

        public void Reset()
        {
            newEnemies = null;
        }

        public void Shuffle(int seed)
        {
            // Shuffle enemies
        }
    }
}
