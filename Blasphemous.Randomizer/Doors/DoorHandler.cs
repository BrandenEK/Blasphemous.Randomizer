using Framework.Managers;
using Tools.Level.Actionables;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Randomizer.Doors;

public class DoorHandler
{
    // Before spawning player, might have to change the spawn point of a few doors
    public void LevelPreloaded(string level)
    {
        // If entering from a certain door, change the spawn point
        DoorData doorToEnter = itemShuffler.LastDoor;
        if (doorToEnter == null || level != doorToEnter.Room || !Main.Randomizer.DataHandler.FixedDoorPositions.TryGetValue(doorToEnter.Id, out Vector3 newPosition))
            return;

        string doorId = doorToEnter.IdentityName;
        Door[] doors = Object.FindObjectsOfType<Door>();
        foreach (Door door in doors)
        {
            if (door.identificativeName == doorId)
            {
                Main.Randomizer.LogWarning($"Modifiying spawn point of {doorId} door");
                door.spawnPoint.position = newPosition;
                break;
            }
        }
    }

    public void LevelLoaded(string level)
    {
        if (level == "D17Z01S11" || level == "D05Z02S14" || level == "D01Z04S18")
        {
            // Disable right wall in Warden & Exposito & Piety boss room
            Main.Randomizer.BossHandler.BossBoundaryStatus = false;
        }
        else if (level == "D03BZ01S01" || level == "D03Z03S15")
        {
            // Close Anguish boss fight gate when entering
            bool shouldBeOpen = level == "D03Z03S15";
            Gate[] gates = Object.FindObjectsOfType<Gate>();
            foreach (Gate gate in gates)
            {
                if (gate.IsOpenOrActivated() != shouldBeOpen)
                    gate.Use();
            }
        }

        // If entering from a certain door, remove the wall
        DoorData doorToEnter = itemShuffler.LastDoor;
        if (doorToEnter == null || level != doorToEnter.Room || !Main.Randomizer.DataHandler.FixedDoorWalls.TryGetValue(doorToEnter.Id, out string wallToRemove))
            return;

        GameObject parent = GameObject.Find("INTERACTABLES");
        if (parent == null) return;

        Main.Randomizer.LogWarning("Disabling hidden wall for " + doorToEnter.Id);
        parent.transform.Find(wallToRemove).gameObject.SetActive(false);
        Core.Events.SetFlag("HIDDEN_WALL_" + doorToEnter.Room, true);
    }

    // Control position of Rooftops elevator
    public void FixRooftopsElevator(string scene)
    {
        if (Main.Randomizer.CurrentSettings.DoorShuffleType <= 0)
            return;

        if (scene == "D06Z01S01")
        {
            // Keep the elevator set at position 4
            Core.Events.SetFlag("ELEVATOR_POSITION_1", false);
        }
        else if (scene == "D06Z01S19")
        {
            // Prevent the elevator crashing when returning to main room
            Core.Events.SetFlag("ELEVATOR_POSITION_FAKE", true);
        }
    }
}
