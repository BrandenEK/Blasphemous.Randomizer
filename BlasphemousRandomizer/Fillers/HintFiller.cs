using System.Collections.Generic;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class HintFiller : Filler
    {
        public void Fill(int seed, Dictionary<string, string> output)
        {
            initialize(seed);

            // Get list of dialog ids & possible hint locations
            List<int> dialogIds = new List<int>();
            for (int i = 1; i <= 34; i++)
            {
                dialogIds.Add(i);
            }
            List<string> possibleLocations = new List<string>();
            foreach (string location in Main.Randomizer.data.itemLocations.Keys)
            {
                Item item = Main.Randomizer.itemShuffler.getItemAtLocation(location);
                if (item.type == 0 || item.type == 1 || item.type == 2 || item.type == 3 || item.type == 5 || item.type == 7 || item.type == 8 || item.type == 9)
                {
                    possibleLocations.Add(location);
                }
            }
            possibleLocations.Remove("QI106");

            // Fill guaranteed hints
            Dictionary<int, string> grtdHints = new Dictionary<int, string>();
            fillGrtdHintLocations(grtdHints);
            foreach (int dialogId in grtdHints.Keys)
            {
                addHint(dialogId, getHintText(grtdHints[dialogId], Main.Randomizer.itemShuffler.getItemAtLocation(grtdHints[dialogId])), output);
                dialogIds.RemoveAt(dialogId - 1);
                possibleLocations.Remove(grtdHints[dialogId]);
            }

            // Fill random hints
            shuffleList(possibleLocations);
            while (dialogIds.Count > 0)
            {
                int randIdx = rand(dialogIds.Count);
                string location = possibleLocations[possibleLocations.Count - 1];
                addHint(dialogIds[randIdx], getHintText(location, Main.Randomizer.itemShuffler.getItemAtLocation(location)), output);
                possibleLocations.RemoveAt(possibleLocations.Count - 1);
                dialogIds.RemoveAt(randIdx);
            }
        }

        // Adds a hint to the output dictionary
        private void addHint(int id, string hint, Dictionary<string, string> output)
        {
            string key = "DLG_20" + id.ToString("00");
            output.Add(key, hint);
        }

        private string getHintText(string location, Item item)
        {
            //Get hints
            string locationHint = "", itemHint = "";
            if (!Main.Randomizer.data.locationHints.TryGetValue(location, out locationHint) || !Main.Randomizer.data.itemHints.TryGetValue(item.id, out itemHint))
            {
                return "???";
            }

            //Build hint text
            string output = locationHint.Replace("*", itemHint);
            return char.ToUpper(output[0]).ToString() + output.Substring(1) + "...";
        }

        // Fills a dictionary of hints that are guaranteed to be there
        private void fillGrtdHintLocations(Dictionary<int, string> grtd)
        {
            grtd.Clear();
            grtd.Add(34, "QI201");
            grtd.Add(32, "QI202");
            grtd.Add(28, "PR201");
            grtd.Add(27, "Sword[D01Z05S24]");
            grtd.Add(22, "RB11");
            grtd.Add(18, "HE07");
            grtd.Add(17, "PR05");
            grtd.Add(14, "PR04");
            grtd.Add(8, "RB105");
        }
    }
}
