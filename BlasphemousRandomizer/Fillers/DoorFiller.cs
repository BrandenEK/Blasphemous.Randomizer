using System.Collections.Generic;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Fillers
{
    public class DoorFiller : Filler
    {
        public bool Fill(int seed, DoorConfig config, Dictionary<string, string> output)
        {
            // Initialize & create lists
   //         initialize(seed);
   //         List<DoorLocation> unconnected = new List<DoorLocation>(Main.Randomizer.data.doorLocations.Values);
   //         List<DoorLocation> connected = new List<DoorLocation>();
   //         List<DoorLocation> accessible = new List<DoorLocation>();

   //         // Reset temporary door data
   //         for (int i = 0; i < unconnected.Count; i++)
   //         {
   //             unconnected[i].available = false;
   //             unconnected[i].targetDoor = "";
   //         }

   //         // Test number of each direction - temp
   //         int[] directions = new int[7];
   //         for (int i = 0; i < unconnected.Count; i++)
   //         {
   //             directions[unconnected[i].direction]++;
   //         }
   //         for (int i = 0; i < directions.Length; i++)
   //         {
   //             Main.Randomizer.Log("Direction " + i + ": " + directions[i]);
   //         }

   //         //Add first room
   //         accessible.Add(unconnected[0]);
   //         unconnected[0].available = true;

   //         // Connect other vanilla connections

   //         //Shuffle unconnected list
   //         shuffleList(unconnected);


			//// Loop through unconnected doors and connect them
			//while (unconnected.Count > 0 && accessible.Count > 0)
			//{
			//	// Get random entrance door from all accessible ones
			//	int enterIdx = rand(accessible.Count);
			//	DoorLocation enterDoor = accessible[enterIdx];
			//	Main.Randomizer.Log("Enter door: " + enterDoor.id);

			//	// Get first valid door from unconnected ones
			//	int exitIdx = -1;
			//	int undesirableIdx = -1;
			//	int validDirections = getOppositeDirection(enterDoor);
			//	for (int i = unconnected.Count - 1; i >= 0; i--) // Insert at random spot or start differently to make sure you're not just doing the same doors?
			//	{
			//		// Invalid door if ...
			//		if (unconnected[i].direction != validDirections) continue; // Wrong direction
			//		if (enterDoor.id == unconnected[i].id) continue; // This is the same door

			//		// There are still more doors after these two
			//		if (unconnected.Count > 2)
			//		{
			//			// Find if this door will open any more up
			//			int newDoors = -1;
			//			if (unconnected[i].newDoors != null)
			//			{
			//				string[] doors = getNewDoors(unconnected[i]);
			//				for (int j = 0; j < doors.Length; j++)
			//				{
			//					if (!Main.Randomizer.data.doorLocations.ContainsKey(doors[j]))
			//						Main.Randomizer.LogError("Door does not exist: " + doors[j]);
			//					if (!Main.Randomizer.data.doorLocations[doors[j]].available)
			//					{
			//						newDoors++;
			//					}
			//				}
			//			}
			//			if (unconnected[i].available)
			//			{
			//				newDoors--;
			//				undesirableIdx = i;
			//				continue;
			//			}

			//			// If this connection will close the loop, dont do it
			//			if (accessible.Count + newDoors <= 0) continue;
			//		}

			//		// Can't come out on the other side of a one way door
			//		// If (unconnected[i].type == 3) continue; // Additional logic to ensure there is a dead end after this one

			//		exitIdx = i;
			//		break;
			//	}

			//	// There was no valid door to connect to this one
			//	if (exitIdx < 0)
			//	{
			//		// Try to swap doors up to a certain number of times to connect the rest
			//		if (undesirableIdx >= 0)
			//			exitIdx = undesirableIdx;
			//		else
			//			return false;
			//	}
			//	DoorLocation exitDoor = unconnected[exitIdx];
			//	Main.Randomizer.Log("Exit door: " + exitDoor.id);

			//	// Connect doors
			//	enterDoor.targetDoor = exitDoor.id;
			//	exitDoor.targetDoor = enterDoor.id;

			//	// Add & remove from lists
			//	connected.Add(enterDoor);
			//	connected.Add(exitDoor);
			//	accessible.RemoveAt(enterIdx);
			//	accessible.Remove(exitDoor);
			//	unconnected.RemoveAt(exitIdx);
			//	unconnected.Remove(enterDoor);

			//	// Add more accessible doors based on new exit
			//	if (exitDoor.newDoors != null)
			//	{
			//		string[] doors = getNewDoors(exitDoor);
			//		for (int i = 0; i < doors.Length; i++)
			//		{
			//			// If door is not already accessible/connected, mark it as so and add it
			//			if (!Main.Randomizer.data.doorLocations.ContainsKey(doors[i]))
			//				Main.Randomizer.LogError("Door does not exist: " + doors[i]);
			//			DoorLocation newDoor = Main.Randomizer.data.doorLocations[doors[i]];
			//			if (!newDoor.available)
			//			{
			//				Main.Randomizer.Log("New door is accessible: " + doors[i]);
			//				newDoor.available = true;
			//				accessible.Add(newDoor);
			//			}
			//		}
			//	}
			//	exitDoor.available = true;
			//}
			//if (unconnected.Count > 0 || accessible.Count > 0)
			//{
			//	// There were still doors that weren't added to connected list
			//	return false;
			//}

			//// Add doors to dictionary
			//for (int i = 0; i < connected.Count; i++)
			//{
			//	output.Add(connected[i].id, connected[i].targetDoor);
			//}
			return true;
		}

		//int getOppositeDirection(DoorLocation door)
  //      {
		//	if (door.direction == 0) return 3;
		//	if (door.direction == 3) return 0;
		//	if (door.direction == 1) return 2;
		//	if (door.direction == 2) return 1;
		//	if (door.direction == 4) return 4;
		//	if (door.direction == 5) return 6;
		//	if (door.direction == 6) return 5;
		//	return -1;
		//}

		//string[] getNewDoors(DoorLocation door)
  //      {
		//	return door.newDoors.Split(',');
  //      }

		//string getScene(DoorLocation door)
  //      {
		//	return door.id.Substring(0, door.id.IndexOf('['));
  //      }
    }
}
