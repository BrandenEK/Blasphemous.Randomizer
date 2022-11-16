using System;
using System.Collections.Generic;

namespace BlasphemousRandomizer.Fillers
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
            List<T> list2 = new List<T>();
            while (list.Count > 0)
            {
                int index = rand(list.Count);
                list2.Add(list[index]);
                list.RemoveAt(index);
            }
            list.AddRange(list2);
        }
    }
}
