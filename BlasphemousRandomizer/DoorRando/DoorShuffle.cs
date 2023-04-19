using System.Collections.Generic;

namespace BlasphemousRandomizer.DoorRando
{
    public class DoorShuffle : IShuffle
    {
        private DoorFiller filler;

        

        public void Init()
        {
            filler = new DoorFiller();
        }

        public void Reset()
        {
            //newDoors = null;
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
    }
}
