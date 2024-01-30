using System.Collections.Generic;
using Blasphemous.Randomizer.ItemRando;

namespace Blasphemous.Randomizer.HintRando
{
    public class HintFiller : Filler
    {
        // Must go in decreasing order
        private Dictionary<int, string> grtdHints = new Dictionary<int, string>() // move to data file ?
        {
            { 34, "QI201" },
            { 32, "QI202" },
            { 28, "PR201" },
            { 27, "Sword[D01Z05S24]" },
            { 18, "HE07" },
            { 17, "PR05" },
            { 14, "PR04" },
            { 8, "RB105" }
        };

        public void Fill(int seed, Dictionary<string, string> output)
        {
            initialize(seed);
            Config config = Main.Randomizer.GameSettings;

            // Get list of dialog ids & possible hint locations
            List<int> dialogIds = new List<int>();
            for (int i = 1; i <= 34; i++)
            {
                dialogIds.Add(i);
            }
            List<string> possibleLocations = new List<string>();
            foreach (ItemLocation location in Main.Randomizer.data.itemLocations.Values)
            {
                Item item = Main.Randomizer.itemShuffler.getItemAtLocation(location.Id);
                if (!location.IsVanilla(config) && item != null && item.progression && (item.type == 0 || item.type == 1 || item.type == 2 || item.type == 3 || item.type == 5 || item.type == 7 || item.type == 8 || item.type == 9 || item.type == 11))
                {
                    possibleLocations.Add(location.Id);
                }
            }
            possibleLocations.Remove("QI106");

            // Fill guaranteed hints
            foreach (int dialogId in grtdHints.Keys)
            {
                addHint(dialogId, grtdHints[dialogId], output);
                dialogIds.RemoveAt(dialogId - 1);
                possibleLocations.Remove(grtdHints[dialogId]);
            }

            // Fill random hints
            shuffleList(possibleLocations);
            while (dialogIds.Count > 0 && possibleLocations.Count > 0)
            {
                int randIdx = rand(dialogIds.Count);
                addHint(dialogIds[randIdx], possibleLocations[possibleLocations.Count - 1], output);
                possibleLocations.RemoveAt(possibleLocations.Count - 1);
                dialogIds.RemoveAt(randIdx);
            }

            // Show error if hint dictionary wasnt filled
            if (dialogIds.Count > 0)
            {
                Main.Randomizer.LogWarning("Error: Hint dictionary wasn't fully filled!");
            }
        }

        // Adds a hint to the output dictionary
        private void addHint(int id, string hint, Dictionary<string, string> output)
        {
            string key = "DLG_20" + id.ToString("00");
            output.Add(key, hint);
        }
    }
}
