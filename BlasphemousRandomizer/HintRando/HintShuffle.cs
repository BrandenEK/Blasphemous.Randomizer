using System.Collections.Generic;
using BlasphemousRandomizer.ItemRando;

namespace BlasphemousRandomizer.HintRando
{
    public class HintShuffle : IShuffle
    {
        private Dictionary<string, string> newHints;
        private HintFiller filler;

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
            if (item == null || !Main.Randomizer.data.locationHints.TryGetValue(location, out string locationHint)) // change to simply get location/item text from structure itself
                return "???";

            string output = locationHint.Replace("*", item.hint);
            return char.ToUpper(output[0]).ToString() + output.Substring(1) + "...";
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
            if (!Main.Randomizer.gameConfig.general.allowHints || !Main.Randomizer.data.isValid || !Main.Randomizer.itemShuffler.validSeed)
                return;

            newHints = new Dictionary<string, string>();
            filler.Fill(seed, newHints);
            Main.Randomizer.Log(newHints.Count + " hints have been shuffled!");
        }
    }
}
