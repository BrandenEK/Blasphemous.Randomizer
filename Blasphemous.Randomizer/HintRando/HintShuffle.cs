using Blasphemous.Randomizer.ItemRando;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.HintRando
{
    public class HintShuffle : IShuffle
    {
        private Dictionary<string, string> newHints;
        private HintFiller filler;

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
            string locationHint, itemHint;

            if (location == "SIERPES")
            {
                Item item1 = Main.Randomizer.itemShuffler.getItemAtLocation("BossTrigger[5000]");
                Item item2 = Main.Randomizer.itemShuffler.getItemAtLocation("QI202");
                locationHint = Main.Randomizer.data.itemLocations["QI202"].Hint;
                itemHint = $"{item1?.hint ?? "???"} and {item2?.hint ?? "???"}";
            }
            else
            {
                Item item = Main.Randomizer.itemShuffler.getItemAtLocation(location);
                locationHint = Main.Randomizer.data.itemLocations[location].Hint;
                itemHint = item?.hint ?? "???";
            }

            string output = locationHint.Replace("*", itemHint);
            return char.ToUpper(output[0]).ToString() + output.Substring(1) + "...";
        }

        public void Init()
        {
            filler = new HintFiller();
        }

        public void Shuffle(int seed)
        {
            if (!Main.Randomizer.GameSettings.AllowHints || !Main.Randomizer.data.isValid || !Main.Randomizer.itemShuffler.ValidSeed)
                return;

            newHints = new Dictionary<string, string>();
            filler.Fill(seed, newHints);
            Main.Randomizer.Log(newHints.Count + " hints have been shuffled!");
        }
    }
}
