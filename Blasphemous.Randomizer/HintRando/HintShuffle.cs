using Blasphemous.Randomizer.Filling;
using Blasphemous.Randomizer.ItemRando;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.HintRando
{
    public class HintShuffle : IShuffle
    {
        private readonly HintFiller _filler;

        private Dictionary<string, string> newHints;

        public HintShuffle(HintFiller filler)
        {
            _filler = filler;
        }

        // Manage mapped hints
        public Dictionary<string, string> SaveMappedHints() => newHints;
        public void LoadMappedHints(Dictionary<string, string> mappedHints) => newHints = mappedHints;
        public void ClearMappedHints() => newHints = null;

        public string getHint(string id)
        {
            if (newHints != null && newHints.ContainsKey(id))
            {
                Main.Randomizer.Log("Retrieving hint: " + id);
                return getHintText(newHints[id]);
            }
            return "???";
        }

        // Returns the actual hint that describes the item and location
        private string getHintText(string location)
        {
            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(location);
            string locationHint = Main.Randomizer.data.itemLocations[location].Hint;
            string itemHint = item == null ? "???" : item.hint;

            string output = locationHint.Replace("*", itemHint);
            return char.ToUpper(output[0]).ToString() + output.Substring(1) + "...";
        }

        public bool Shuffle(int seed, Config config)
        {
            if (!config.AllowHints)
                return true;

            newHints = new Dictionary<string, string>();
            _filler.Fill(seed, config, newHints);
            Main.Randomizer.Log(newHints.Count + " hints have been shuffled!");
            return true;
        }
    }
}
