using System;
using System.Collections.Generic;

namespace Blasphemous.Randomizer
{
    public abstract class Filler
    {
        private Random rng;

        protected void initialize(int seed)
        {
            rng = new Random(seed);
        }

        protected int rand(int max)
        {
            return rng.Next(max);
        }

        protected void shuffleList<T>(List<T> list)
        {
            int upperIdx = list.Count;
            while (upperIdx > 1)
            {
                upperIdx--;
                int randIdx = rand(upperIdx + 1);
                T value = list[randIdx];
                list[randIdx] = list[upperIdx];
                list[upperIdx] = value;
            }
        }
    }
}
