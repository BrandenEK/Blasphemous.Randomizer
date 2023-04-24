using System.Collections.Generic;
using BlasphemousRandomizer.DoorRando;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class ItemFiller : Filler
    {
        public bool Fill(int seed, Dictionary<string, string> mappedDoors, Dictionary<string, string> mappedItems)
        {
            // Initialize seed with empty lists
            initialize(seed);
            BlasphemousInventory inventory = new BlasphemousInventory();
            Config config = Main.Randomizer.gameConfig;
            mappedDoors.Clear();
            mappedItems.Clear();

            Dictionary<string, ItemLocation> allItemLocations = Main.Randomizer.data.itemLocations;
            Dictionary<string, DoorLocation> allDoorLocations = Main.Randomizer.data.doorLocations;
            Dictionary<string, Item> allItems = Main.Randomizer.data.items;

            // Sort all item locations & doors into rooms
            Dictionary<string, List<string>> roomObjects = new Dictionary<string, List<string>>();
            foreach (DoorLocation door in allDoorLocations.Values)
            {
                string scene = door.Room;
                if (!roomObjects.ContainsKey(scene))
                    roomObjects.Add(scene, new List<string>());
                roomObjects[scene].Add(door.Id);
            }
            foreach (ItemLocation itemLoc in allItemLocations.Values)
            {
                string scene = itemLoc.Room;
                if (!roomObjects.ContainsKey(scene))
                    roomObjects.Add(scene, new List<string>());
                roomObjects[scene].Add(itemLoc.Id);
            }

            int itemLocationsToShuffle = allItemLocations.Count;
            int doorsRemaining = allDoorLocations.Count;

            // Fill list of all items based on their counts
            List<Item> items = new List<Item>();
            foreach (Item item in allItems.Values)
            {
                if (item.count == 1)
                    items.Add(item);
                else
                    for (int i = 0; i < item.count; i++)
                        items.Add(item);
            }

            // Add/Remove any items from the pool based on config
            if (!config.ShuffleReliquaries)
            {
                items.Remove(allItems["RB101"]);
                items.Remove(allItems["RB102"]);
                items.Remove(allItems["RB103"]);
            }
            if (!config.ShuffleDash)
            {
                items.Remove(allItems["Slide"]);
                inventory.AddItem("Slide");
            }
            if (!config.ShuffleWallClimb)
            {
                items.Remove(allItems["WallClimb"]);
                inventory.AddItem("WallClimb");
            }
            if (!config.ShuffleBootsOfPleading)
            {
                items.Remove(allItems["RE401"]);
                ItemLocation bootsLocation = allItemLocations["RE401"];
                roomObjects[bootsLocation.Room].Remove("RE401");
                itemLocationsToShuffle--;
            }
            if (!config.ShufflePurifiedHand)
            {
                items.Remove(allItems["RE402"]);
                ItemLocation handLocation = allItemLocations["RE402"];
                roomObjects[handLocation.Room].Remove("RE402");
                itemLocationsToShuffle--;
            }

            // Add/Remove any extra items to even out the pool
            int extraItems = items.Count - itemLocationsToShuffle;
            if (extraItems > 0)
            {
                // Remove the lowest tear rewards from the item pool
                for (int i = 0; i < extraItems; i++)
                    items.RemoveAt(items.Count - 1);
            }
            else if (extraItems < 0)
            {
                // Add basic tear rewards to fill the item pool
                for (int i = 0; i < -extraItems; i++)
                    items.Add(allItems["Tears[500]"]);
            }

            // Make sure that there are the same number of items as locations
            if (itemLocationsToShuffle != items.Count)
                throw new System.Exception("Number of items is different than the number of locations!");

            // Remove any vanilla items from the pool
            foreach (ItemLocation itemLocation in allItemLocations.Values)
                if (itemLocation.IsVanilla(config))
                    items.Remove(allItems[itemLocation.OriginalItem]);

            // If starting with wheel, remove it from the item pool & set its location
            if (config.StartWithWheel)
            {
                items.Remove(allItems["RB203"]);
                mappedItems.Add("QI106", "RB203");
                inventory.AddItem("RB203");
            }

            // Fill the list of progression items with only progression items
            List<Item> progressionItems = new List<Item>();
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.progression)
                {
                    if (item.id != "QI38" && item.id != "QI39" && item.id != "QI40")
                        progressionItems.Add(item);
                    items.RemoveAt(i);
                    i--;
                }
            }
            shuffleList(progressionItems);

            // Move certain items to the end of the list to place them first
            progressionItems.Add(allItems["QI38"]);
            progressionItems.Add(allItems["QI39"]);
            progressionItems.Add(allItems["QI40"]);

            // Create and fill initially visible doors/items lists
            List<DoorLocation> visibleDoors = new List<DoorLocation>();
            List<ItemLocation> visibleItems = new List<ItemLocation>();
            DoorLocation startingDoor = allDoorLocations[Main.Randomizer.StartingDoor.Door];

            roomObjects["Initial"].AddRange(roomObjects[startingDoor.Room]);
            foreach (string obj in roomObjects["Initial"])
            {
                if (obj[0] == 'D')
                {
                    DoorLocation door = allDoorLocations[obj];
                    if (door.Direction != 5)
                        visibleDoors.Add(door);
                }
                else if (!config.StartWithWheel || obj != "QI106")
                {
                    visibleItems.Add(allItemLocations[obj]);
                }
            }
            visibleDoors.Add(allDoorLocations["D01Z02S07[E]"]);
            visibleItems.Add(allItemLocations["QI65"]);
            inventory.AddItem(startingDoor.Id);

            // While there are still doors or items to place, place them
            while (progressionItems.Count > 0 || doorsRemaining > 0)
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
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    ItemLocation itemLocation = visibleItems[i];
                    if (itemLocation.Logic == null || Parser.EvaluateExpression(itemLocation.Logic, inventory))
                    {
                        // If long quest item and forcing junk, make it never reachable
                        if (config.JunkLongQuests && itemLocation.Type == 8)
                        {
                            continue;
                        }
                        // If vanilla item, instantly set its item here
                        if (itemLocation.IsVanilla(config))
                        {
                            Item vanillaItem = allItems[itemLocation.OriginalItem];
                            visibleItems.RemoveAt(i);
                            i--;

                            mappedItems.Add(itemLocation.Id, vanillaItem.id);
                            inventory.AddItem(vanillaItem.id);
                            continue;
                        }

                        // If random item, add it to reachable list
                        reachableLocations.Add(itemLocation);
                    }
                }
                if (reachableLocations.Count == 0)
                {
                    return false;
                }
                if (progressionItems.Count == 0)
                {
                    // No more items to place but maybe need to finish connecting doors?
                    continue;
                }

                int locationIdx = rand(reachableLocations.Count);
                ItemLocation randomLocation = reachableLocations[locationIdx];
                int itemIdx = progressionItems.Count - 1;
                Item randomItem = progressionItems[itemIdx];

                visibleItems.Remove(randomLocation);
                progressionItems.RemoveAt(itemIdx);
                mappedItems.Add(randomLocation.Id, randomItem.id);
                inventory.AddItem(randomItem.id);
            }

            // Once all progression items have been placed and the doors are filled, place all filler items
            shuffleList(visibleItems);
            for (int i = 0; i < visibleItems.Count; i++)
            {
                ItemLocation currentLocation = visibleItems[i];
                Item itemToPlace;

                if (currentLocation.IsVanilla(config))
                {
                    itemToPlace = allItems[currentLocation.OriginalItem];
                }
                else
                {
                    if (items.Count < 1)
                        throw new System.Exception("Invalid number of filler items after main shuffle!");

                    itemToPlace = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);
                }
                mappedItems.Add(currentLocation.Id, itemToPlace.id);
            }
            if (items.Count > 0)
                throw new System.Exception("Invalid number of filler items after main shuffle!");

            // Seed is filled & validated
            return true;
        }
	}
}
