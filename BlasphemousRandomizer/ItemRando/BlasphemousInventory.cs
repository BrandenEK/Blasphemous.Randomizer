using System.Collections.Generic;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class BlasphemousInventory : InventoryData
    {
        // Relics
        private bool blood = false;
        private bool root = false;
        private bool linen = false;
        private bool nail = false;
        private bool shroud = false;
        private bool lung = false;

        // Keys
        private bool bronzeKey = false;
        private bool silverKey = false;
        private bool goldKey = false;
        private bool peaksKey = false;
        private bool elderKey = false;
        private bool woodKey = false;

        // temp
        private bool scapular = false;

        // Doors
        private Dictionary<string, bool> doors = new Dictionary<string, bool>();

        protected override Variable GetVariable(string variable)
        {
            if (variable[0] == 'D')
            {
                return new BoolVariable(doors.ContainsKey(variable) && doors[variable]);
            }

            switch (variable)
            {
                case "blood": return new BoolVariable(blood);
                case "root": return new BoolVariable(root);
                case "linen": return new BoolVariable(linen);
                case "nail": return new BoolVariable(nail);
                case "shroud": return new BoolVariable(shroud);
                case "lung": return new BoolVariable(lung);

                case "bronzeKey": return new BoolVariable(bronzeKey);
                case "silverKey": return new BoolVariable(silverKey);
                case "goldKey": return new BoolVariable(goldKey);
                case "peaksKey": return new BoolVariable(peaksKey);
                case "elderKey": return new BoolVariable(elderKey);
                case "woodKey": return new BoolVariable(woodKey);

                case "scapular": return new BoolVariable(scapular);
                case "cherubs": return new IntVariable(0);
                default:
                    throw new System.Exception($"Error: Variable {variable} doesn't exist!");
            }
        }

        public void AddItem(string itemId)
        {
            if (itemId[0] == 'D')
            {
                if (!doors.ContainsKey(itemId))
                    doors.Add(itemId, true);
            }

            switch (itemId)
            {
                case "RE01": blood = true; break;
                case "RE03": nail = true; break;
                case "RE04": shroud = true; break;
                case "RE05": linen = true; break;
                case "RE07": lung = true; break;
                case "RE10": root = true; break;

                case "QI58": elderKey = true; break;
                case "QI69": bronzeKey = true; break;
                case "QI70": silverKey = true; break;
                case "QI71": goldKey = true; break;
                case "QI72": peaksKey = true; break;
                case "QI204": woodKey = true; break;

                case "QI203": scapular = true; break;
            }
        }
    }
}
