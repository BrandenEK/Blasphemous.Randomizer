using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;

namespace BlasphemousRandomizer.Shufflers
{
    public class HintShuffle : IShuffle
    {
        private Dictionary<string, string> newHints;
        private HintFiller filler;

        public string getHint(string id)
        {
            if (newHints != null && newHints.ContainsKey(id))
            {
                return newHints[id];
            }
            return "Error!";
        }

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
            if (!filler.isValid())
            {
                Main.Randomizer.Log("Error: Hint data could not be loaded!");
                return;
            }
            newHints = new Dictionary<string, string>();
            filler.Fill(seed, newHints);
            Main.Randomizer.Log(newHints.Count + " hints have been shuffled!");
        }

        public string GetSpoiler()
        {
            return "";
        }
    }
}
