using BlasphemousRandomizer.DoorRando;
using LogicParser;
using System.Collections.Generic;

namespace BlasphemousRandomizer.ItemRando
{
    public class ItemFiller : Filler
    {
        public bool Fill(int seed, Dictionary<string, string> mappedDoors, Dictionary<string, string> mappedItems)
        {
            // Initialize seed with empty lists
            initialize(seed);
            Config config = Main.Randomizer.GameSettings;
            BlasphemousInventory inventory = new BlasphemousInventory();
            inventory.SetConfigSettings(config);
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

            // Create list of all unconnected doors
            List<DoorLocation> unconnectedDoors = new List<DoorLocation>(allDoorLocations.Values);
            for (int i = unconnectedDoors.Count - 1; i >= 0; i--)
            {
                if (unconnectedDoors[i].ShouldBeVanillaDoor(config))
                {
                    unconnectedDoors.RemoveAt(i);
                }
            }
            shuffleList(unconnectedDoors);

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
            int itemLocationsToShuffle = allItemLocations.Count;
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
            string[] itemsToMove = new string[] { "QI38", "QI39", "QI40" };

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.progression)
                {
                    if (!itemsToMove.Contains(item.id))
                        progressionItems.Add(item);
                    items.RemoveAt(i);
                    i--;
                }
            }
            shuffleList(progressionItems);

            // Move certain items to the second half of the list to place them sooner
            foreach (string itemId in itemsToMove)
            {
                int halfwayIdx = progressionItems.Count / 2;
                int insertIdx = halfwayIdx + rand(halfwayIdx + 1);
                progressionItems.Insert(insertIdx, allItems[itemId]);
            }

            // Create and fill initially visible doors/items lists
            List<DoorLocation> visibleDoors = new List<DoorLocation>();
            List<ItemLocation> visibleItems = new List<ItemLocation>();
            DoorLocation startingDoor = allDoorLocations[Main.Randomizer.StartingDoor.Door];

            roomObjects["Initial"].AddRange(roomObjects[startingDoor.Room]); // Starting room is visible
            roomObjects["D02Z02S11"].AddRange(roomObjects["D01Z02S03"]); // Albero elevator room is also visible after graveyard elevator
            foreach (string obj in roomObjects["Initial"])
            {
                if (obj[0] == 'D')
                {
                    DoorLocation door = allDoorLocations[obj];
                    if (door.Direction != 5)
                        visibleDoors.Add(door); // Maybe instead check visibility flags
                }
                else if (!config.StartWithWheel || obj != "QI106")
                {
                    visibleItems.Add(allItemLocations[obj]);
                }
            }
            inventory.AddItem(startingDoor.Id);

            // While there are still doors or items to place, place them
            bool placedAllItems = false;
            while (progressionItems.Count > 0 || unconnectedDoors.Count > 0)
            {
                // Continue connecting and processing new doors until no more are reachable
                Stack<DoorLocation> newlyFoundDoors = new Stack<DoorLocation>(visibleDoors);
                visibleDoors.Clear();
                while (newlyFoundDoors.Count > 0)
                {
                    DoorLocation enterDoor = newlyFoundDoors.Pop();
                    if (mappedDoors.ContainsKey(enterDoor.Id)) continue;

                    if (Parser.EvaluateExpression(enterDoor.Logic, inventory))
                    {
                        // Connect the door to vanilla/random door and add to output
                        DoorLocation exitDoor = null;
                        if (!enterDoor.ShouldBeVanillaDoor(config))
                        {
                            // Get first valid door from unconnected ones
                            int exitIdx = -1, undesirableIdx = -1;
                            for (int i = unconnectedDoors.Count - 1; i >= 0; i--) // Insert at random spot or start differently to make sure you're not just doing the same doors?
                            {
                                DoorLocation currentDoor = unconnectedDoors[i];

                                // Invalid door if ...
                                if (currentDoor.Direction != enterDoor.OppositeDirection) continue; // Wrong direction
                                if (enterDoor.Id == currentDoor.Id) continue; // This is the same door (probably dont need this since the direction will always be opposite)

                                // If this door is in a room you already have access to, only connect it if no other option since it would take away an option for dead end rooms
                                if (newlyFoundDoors.Contains(currentDoor) || visibleDoors.Contains(currentDoor))
                                {
                                    undesirableIdx = i;
                                    continue;
                                }

                                // If there are still more doors after these two, need to make sure the loop doesn't close
                                if (unconnectedDoors.Count > 2)
                                {
                                    // Find if this door will open any more up
                                    int newDoors = -1;
                                    inventory.AddItem(currentDoor.Id);
                                    foreach (string obj in roomObjects[currentDoor.Room])
                                    {
                                        if (obj[0] != 'D' || mappedDoors.ContainsKey(obj) || currentDoor.Id == obj) continue;

                                        DoorLocation door = allDoorLocations[obj];
                                        if (newlyFoundDoors.Contains(door) || visibleDoors.Contains(door))
                                            continue;

                                        if (door.ShouldBeMadeVisible(config, inventory))
                                            newDoors++;
                                    }
                                    inventory.RemoveItem(currentDoor.Id);

                                    // Because the current enterDoor has been popped from the stack, there is actually one more visible door than it thinks
                                    if (newlyFoundDoors.Count + visibleDoors.Count + newDoors < 0) continue;
                                }

                                // Can't come out on the other side of a one way door
                                // If (unconnected[i].type == 3) continue; // Additional logic to ensure there is a dead end after this one

                                exitIdx = i;
                                break;
                            }

                            if (exitIdx >= 0)
                            {
                                // There was a valid door to connect to
                                exitDoor = unconnectedDoors[exitIdx];
                            }
                            else if (undesirableIdx >= 0)
                            {
                                // There was no valid door, but there was an undesirable one
                                exitDoor = unconnectedDoors[undesirableIdx];
                            }
                        }
                        else
                        {
                            exitDoor = allDoorLocations[enterDoor.OriginalDoor];
                        }

                        if (exitDoor == null)
                        {
                            // If this door is reachable but cant connect to anything without closing the loop, put it off until later and hope another door becomes reachable
                            if (!visibleDoors.Contains(enterDoor))
                                visibleDoors.Add(enterDoor);
                            continue;
                        }

                        mappedDoors.Add(enterDoor.Id, exitDoor.Id);
                        mappedDoors.Add(exitDoor.Id, enterDoor.Id);
                        inventory.AddItem(enterDoor.Id);
                        inventory.AddItem(exitDoor.Id);
                        unconnectedDoors.Remove(enterDoor);
                        unconnectedDoors.Remove(exitDoor);

                        // Add everything in the new room
                        foreach (string obj in roomObjects[exitDoor.Room])
                        {
                            if (obj[0] == 'D')
                            {
                                // If this door hasn't already been processed, make it visible
                                DoorLocation newDoor = allDoorLocations[obj];
                                if (!mappedDoors.ContainsKey(newDoor.Id) && newDoor.ShouldBeMadeVisible(config, inventory))
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
                    if (Parser.EvaluateExpression(itemLocation.Logic, inventory))
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
                    if (placedAllItems && unconnectedDoors.Count > 0)
                    {
                        // All items have been placed, but somehow not all doors are reachable even after another go through
                        return false;
                    }
                    placedAllItems = true;
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

            if (mappedDoors.Count != allDoorLocations.Count)
            {
                // Not all doors were mapped, most likely because the rooftops elevator was never reachable, meaning the vanilla doors & locations up there were never made accessible
                return false;
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
