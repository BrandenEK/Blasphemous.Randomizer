using Basalt.LogicParser;
using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.ItemRando;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blasphemous.Randomizer.Tests
{
    [TestClass]
    public class LogicTests
    {
        private static readonly Dictionary<string, Item> _allItems = new();
        private static readonly Dictionary<string, ItemLocation> _allItemLocations = new();
        private static readonly Dictionary<string, DoorLocation> _allDoors = new();

        private BlasphemousInventory inventory;

        [ClassInitialize]
        public static void LoadJsonData(TestContext context)
        {
            string dataFolder = "../../../../resources/data/Randomizer/";

            string items = File.ReadAllText(dataFolder + "items.json");
            foreach (var item in JsonConvert.DeserializeObject<Item[]>(items))
                _allItems.Add(item.id, item);

            string itemLocations = File.ReadAllText(dataFolder + "locations_items.json");
            foreach (var itemLocation in JsonConvert.DeserializeObject<ItemLocation[]>(itemLocations))
                _allItemLocations.Add(itemLocation.Id, itemLocation);

            string doors = File.ReadAllText(dataFolder + "doors.json");
            foreach (var door in JsonConvert.DeserializeObject<DoorLocation[]>(doors))
                _allDoors.Add(door.Id, door);
        }

        [TestInitialize]
        public void CreateInventory()
        {
            inventory = new BlasphemousInventory();
            inventory.SetConfigSettings(new Config());
        }

        [TestMethod]
        public void FindErrorsInDoorLogic()
        {
            var sb = new StringBuilder(Environment.NewLine);
            bool invalid = false;

            foreach (var door in _allDoors.Values)
            {
                try
                {
                    inventory.Evaluate(door.Logic);
                }
                catch (LogicParserException e)
                {
                    sb.AppendLine($"[{door.Id}] {e.Message}");
                    invalid = true;
                }
            }

            if (invalid)
            {
                throw new LogicParserException(sb.ToString());
            }
        }

        [TestMethod]
        public void FindErrorsInLocationLogic()
        {
            var sb = new StringBuilder(Environment.NewLine);
            bool invalid = false;

            foreach (var itemLocation in _allItemLocations.Values)
            {
                try
                {
                    inventory.Evaluate(itemLocation.Logic);
                }
                catch (LogicParserException e)
                {
                    sb.AppendLine($"[{itemLocation.Id}] {e.Message}");
                    invalid = true;
                }
            }

            if (invalid)
            {
                throw new LogicParserException(sb.ToString());
            }
        }
    }
}