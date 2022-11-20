using System.Collections.Generic;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class HintFiller : Filler
    {
        private Dictionary<string, string> locationHints;
        private Dictionary<string, string> itemHints;

        public HintFiller()
        {
            locationHints = new Dictionary<string, string>();
            itemHints = new Dictionary<string, string>();
            FileUtil.parseFileToDictionary("hints_locations.dat", locationHints);
            FileUtil.parseFileToDictionary("hints_items.dat", itemHints);
        }

        public override bool isValid()
        {
            return locationHints.Count > 0 && itemHints.Count > 0;
        }

        public void Fill(int seed, Dictionary<string, string> output)
        {
            initialize(seed);
            Dictionary<string, Item> newItems = Main.Randomizer.itemShuffler.getNewItems();

            // Get list of dialog ids & possible hint locations
            List<int> dialogIds = new List<int>();
            for (int i = 1; i <= 34; i++)
            {
                dialogIds.Add(i);
            }
            List<string> possibleLocations = new List<string>();
            foreach (string location in newItems.Keys)
            {
                if (newItems[location].type == 0 || newItems[location].type == 1 || newItems[location].type == 2 || newItems[location].type == 3 || newItems[location].type == 5 || newItems[location].type == 7 || newItems[location].type == 8 || newItems[location].type == 9)
                {
                    possibleLocations.Add(location);
                }
            }
            possibleLocations.Remove("QI106");

            // Fill guaranteed hints
            Dictionary<int, string> grtdHints = new Dictionary<int, string>();
            fillGrtdHintLocations(grtdHints, newItems);
            foreach (int dialogId in grtdHints.Keys)
            {
                addHint(dialogId, getHintText(grtdHints[dialogId], newItems[grtdHints[dialogId]]), output);
                dialogIds.RemoveAt(dialogId - 1);
                possibleLocations.Remove(grtdHints[dialogId]);
            }

            // Fill random hints
            shuffleList(possibleLocations);
            while (dialogIds.Count > 0)
            {
                int randIdx = rand(dialogIds.Count);
                string location = possibleLocations[possibleLocations.Count - 1];
                addHint(dialogIds[randIdx], getHintText(location, newItems[location]), output);
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
            if (!locationHints.TryGetValue(location, out locationHint) || !itemHints.TryGetValue(item.name, out itemHint))
            {
                return "???";
            }

            //Build hint text
            string output = locationHint.Replace("*", itemHint);
            return char.ToUpper(output[0]).ToString() + output.Substring(1) + "...";
        }

        // Fills a dictionary of hints that are guaranteed to be there
        private void fillGrtdHintLocations(Dictionary<int, string> grtd, Dictionary<string, Item> newItems)
        {
            grtd.Clear();
            grtd.Add(34, getLocationFromItem("QI201", newItems));
            grtd.Add(32, getLocationFromItem("QI202", newItems));
            grtd.Add(30, getLocationFromItem("RB203", newItems));
            grtd.Add(28, "PR201");
            grtd.Add(27, "Sword[D01Z05S24]");
            grtd.Add(18, "HE07");
            grtd.Add(17, "PR05");
            grtd.Add(14, "PR04");
            grtd.Add(8, "RB105");
            grtd.Add(6, getLocationFromItem("HE201", newItems));
        }

        // Gets the locationId that holds a certain item
        private string getLocationFromItem(string itemName, Dictionary<string, Item> newItems)
        {
            foreach (string location in newItems.Keys)
            {
                if (newItems[location].name == itemName)
                    return location;
            }
            return "X";
        }
    }
}
