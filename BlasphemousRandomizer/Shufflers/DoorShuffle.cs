using System;
using System.Collections.Generic;
using BlasphemousRandomizer.Fillers;

namespace BlasphemousRandomizer.Shufflers
{
    public class DoorShuffle : IShuffle
    {
        private Dictionary<string, string> newDoors;
        private DoorFiller filler;

        public bool getNewDoor(string doorId, out string targetScene, out string targetId)
        {
            if (newDoors != null && newDoors.ContainsKey(doorId))
            {
                Main.Randomizer.Log("Processing door " + doorId);
                string targetDoorId = newDoors[doorId];
                int bracket = targetDoorId.IndexOf('[');

                targetScene = targetDoorId.Substring(0, bracket);
                targetId = targetDoorId.Substring(bracket + 1, targetDoorId.Length - bracket - 2);
                return true;
            }
            targetScene = "";
            targetId = "";
            return false;
        }

        public void Init()
        {
            filler = new DoorFiller();
        }

        public void Reset()
        {
            newDoors = null;
        }

        public void Shuffle(int seed)
        {
            //if (!Main.Randomizer.data.isValid || Main.Randomizer.gameConfig.doors.type < 1)
            //    return;

            //newDoors = new Dictionary<string, string>();
            //int attempt = 0, maxAttempts = 100;
            //while (!filler.Fill(seed + attempt, Main.Randomizer.gameConfig.doors, newDoors) && attempt < maxAttempts)
            //{
            //    Main.Randomizer.LogError($"Seed {seed + attempt} was invalid! Trying next...");
            //    attempt++;
            //}
            //if (attempt >= maxAttempts)
            //{
            //    Main.Randomizer.LogError($"Error: Failed to fill doors in {maxAttempts} tries!");
            //    return;
            //}

            //Main.Randomizer.Log(newDoors.Count + " doors have been shuffled!");
        }

        public string GetSpoiler()
        {
            return "";
        }
    }
}
