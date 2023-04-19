using System.Collections.Generic;
using BlasphemousRandomizer.DoorRando;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class ItemFiller : Filler
    {
        private const string FIRST_DOOR = "D17Z01S01[E]";

        public bool Fill(int seed, Dictionary<string, string> mappedDoors, Dictionary<string, string> mappedItems)
        {
            // Initialize seed with empty lists
            initialize(seed);
            BlasphemousInventory inventory = new BlasphemousInventory();
            Dictionary<string, ItemLocation> allItemLocations = Main.Randomizer.data.itemLocations;
            Dictionary<string, DoorLocation> allDoorLocations = Main.Randomizer.data.doorLocations;

            List<DoorLocation> visibleDoors = new List<DoorLocation>();
            List<ItemLocation> visibleItems = new List<ItemLocation>();
            List<Item> progressionItems = new List<Item>()
            {
                new Item("", "", "", 0, false, 0){id = "RE01"},
                new Item("", "", "", 0, false, 0){id = "RE05"},
                new Item("", "", "", 0, false, 0){id = "RE07"},
                new Item("", "", "", 0, false, 0){id = "RE10"},
                new Item("", "", "", 0, false, 0){id = "QI58"},
                new Item("", "", "", 0, false, 0){id = "QI203"},
                new Item("", "", "", 0, false, 0){id = "QI204"},
            };
            shuffleList(progressionItems);

            // Make sure that there are the same number of items as locations
            if (allItemLocations.Count != progressionItems.Count)
                throw new System.Exception("Number of items is different than the number of locations!");
            int itemsRemaining = allItemLocations.Count;
            int doorsRemaining = allDoorLocations.Count;

            // Sort all item locations & doors into rooms
            Dictionary<string, List<string>> roomObjects = new Dictionary<string, List<string>>();
            foreach (ItemLocation itemLoc in allItemLocations.Values)
            {
                string scene = itemLoc.Room;
                if (!roomObjects.ContainsKey(scene))
                    roomObjects.Add(scene, new List<string>());
                roomObjects[scene].Add(itemLoc.Id);
            }
            foreach (DoorLocation door in allDoorLocations.Values)
            {
                string scene = door.Room;
                if (!roomObjects.ContainsKey(scene))
                    roomObjects.Add(scene, new List<string>());
                roomObjects[scene].Add(door.Id);
            }

            // Add first room to visible list
            inventory.AddItem(FIRST_DOOR);
            foreach (string obj in roomObjects[allDoorLocations[FIRST_DOOR].Room])
            {
                if (obj[0] == 'D')
                {
                    DoorLocation door = allDoorLocations[obj];
                    if (door.Direction != 5)
                        visibleDoors.Add(door);
                }
                else
                {
                    visibleItems.Add(allItemLocations[obj]);
                }
            }

            // While there are still doors or items to place, place them
            while (itemsRemaining > 0 || doorsRemaining > 0)
            {
                // Continue connecting and processing new doors until no more are reachable
                Stack<DoorLocation> newlyFoundDoors = new Stack<DoorLocation>(visibleDoors);
                visibleDoors.Clear();
                while (newlyFoundDoors.Count > 0)
                {
                    DoorLocation enterDoor = newlyFoundDoors.Pop();
                    if (mappedDoors.ContainsKey(enterDoor.Id)) continue;

                    if (enterDoor.Logic == null || Parser.EvaluateExpression(enterDoor.Logic, inventory))
                    {
                        // Connect the door and add to output
                        DoorLocation exitDoor = allDoorLocations[enterDoor.OriginalDoor];
                        mappedDoors.Add(enterDoor.Id, exitDoor.Id);
                        mappedDoors.Add(exitDoor.Id, enterDoor.Id);
                        inventory.AddItem(enterDoor.Id);
                        inventory.AddItem(exitDoor.Id);
                        doorsRemaining -= 2;

                        // Add everything in the new room
                        foreach (string obj in roomObjects[exitDoor.Room])
                        {
                            if (obj[0] == 'D')
                            {
                                // If this door hasn't already been processed, make it visible
                                DoorLocation newDoor = allDoorLocations[obj];
                                if (!mappedDoors.ContainsKey(newDoor.Id) && newDoor.Direction != 5)
                                {
                                    newlyFoundDoors.Push(newDoor);
                                }
                            }
                            else
                            {
                                // If this item location is new, add it to the visible list
                                ItemLocation itemLocation = allItemLocations[obj];
                                if (!mappedItems.ContainsKey(itemLocation.Id) && !visibleItems.Contains(itemLocation))
                                {
                                    visibleItems.Add(itemLocation);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Add to visible doors, but still not reachable yet
                        if (!visibleDoors.Contains(enterDoor))
                            visibleDoors.Add(enterDoor);
                    }
                }

                // Now that all reachable doors have been processed, place an item at a random reachable location
                List<ItemLocation> reachableLocations = new List<ItemLocation>();
                foreach (ItemLocation itemLocation in visibleItems)
                {
                    if (itemLocation.Logic == null || Parser.EvaluateExpression(itemLocation.Logic, inventory))
                        reachableLocations.Add(itemLocation);
                }
                if (reachableLocations.Count == 0)
                {
                    return false;
                }

                int locationIdx = rand(reachableLocations.Count);
                ItemLocation randomLocation = reachableLocations[locationIdx];
                int itemIdx = progressionItems.Count - 1;
                Item randomItem = progressionItems[itemIdx];

                visibleItems.Remove(randomLocation);
                progressionItems.RemoveAt(itemIdx);
                mappedItems.Add(randomLocation.Id, randomItem.id);
                inventory.AddItem(randomItem.id);
                itemsRemaining--;
            }

            // Seed is filled & validated
            return true;
        }
	}
}
