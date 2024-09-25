using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.DoorRando;
using System.Linq;
using UnityEngine;

namespace Blasphemous.Randomizer
{
    /// <summary>
    /// Stores the settings for a randomizer
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets a random Seed within the valid range
        /// </summary>
        public static int RandomSeed => new System.Random().Next(1, MAX_SEED + 1);

        // Main Settings
        public int Seed { get; set; } = 0;
        public int LogicDifficulty { get; set; } = 1;
        public int StartingLocation { get; set; } = 0;

        // General Settings
        public bool UnlockTeleportation { get; set; } = true;
        public bool AllowHints { get; set; } = true;
        public bool AllowPenitence { get; set; } = false;

        // Item Rando
        public bool ShuffleReliquaries { get; set; } = true;
        public bool ShuffleDash { get; set; } = false;
        public bool ShuffleWallClimb { get; set; } = false;
        public bool ShuffleBootsOfPleading { get; set; } = false;
        public bool ShufflePurifiedHand { get; set; } = false;

        public bool ShuffleSwordSkills { get; set; } = true;
        public bool ShuffleThorns { get; set; } = true;
        public bool JunkLongQuests { get; set; } = true;
        public bool StartWithWheel { get; set; } = false;

        // Enemy Rando
        public int EnemyShuffleType { get; set; } = 0;
        public bool MaintainClass { get; set; } = true;
        public bool AreaScaling { get; set; } = true;

        // Boss Rando
        public int BossShuffleType { get; set; } = 0;

        // Door Rando
        public int DoorShuffleType { get; set; } = 0;

        public StartingLocation RealStartingLocation
        {
            get
            {
                // Return the chosen starting location
                if (StartingLocation <= STARTING_LOCATIONS.Length - 1)
                    return STARTING_LOCATIONS[StartingLocation];

                // Return a random starting location
                var possibleLocations = STARTING_LOCATIONS
                    .Where(x => !ShuffleDash || DoorShuffleType > 1 || (x.StartFlags & StartFlags.RequiresDash) == 0)
                    .Where(x => !ShuffleWallClimb || DoorShuffleType > 1 || (x.StartFlags & StartFlags.RequiresWallClimb) == 0)
                    .Where(x => LogicDifficulty >= 2 || (x.StartFlags & StartFlags.RequiresHardMode) == 0);

                ModLog.Info($"Choosing random starting location from {possibleLocations.Count()} options");
                int randLocation = new System.Random(Seed).Next(0, possibleLocations.Count());
                return possibleLocations.ElementAt(randLocation);
            }
        }

        /// <summary>
        /// A unique int64 based on seed and all important options
        /// </summary>
        public long UniqueSeed
        {
            get
            {
                long uniqueSeed = 0;

                if ((Seed & (1 << 0)) == 0) SetBit(8);
                if ((Seed & (1 << 1)) == 0) SetBit(19);
                if ((Seed & (1 << 2)) > 0) SetBit(11);
                if ((Seed & (1 << 3)) > 0) SetBit(23);
                if ((Seed & (1 << 4)) == 0) SetBit(41);
                if ((Seed & (1 << 5)) > 0) SetBit(38);
                if ((Seed & (1 << 6)) > 0) SetBit(16);
                if ((Seed & (1 << 7)) > 0) SetBit(29);
                if ((Seed & (1 << 8)) > 0) SetBit(32);
                if ((Seed & (1 << 9)) > 0) SetBit(36);
                if ((Seed & (1 << 10)) > 0) SetBit(18);
                if ((Seed & (1 << 11)) > 0) SetBit(12);
                if ((Seed & (1 << 12)) == 0) SetBit(3);
                if ((Seed & (1 << 13)) == 0) SetBit(45);
                if ((Seed & (1 << 14)) == 0) SetBit(42);
                if ((Seed & (1 << 15)) == 0) SetBit(28);
                if ((Seed & (1 << 16)) > 0) SetBit(13);
                if ((Seed & (1 << 17)) > 0) SetBit(35);
                if ((Seed & (1 << 18)) == 0) SetBit(20);
                if ((Seed & (1 << 19)) == 0) SetBit(31);
                if ((Seed & (1 << 20)) > 0) SetBit(10);
                if ((Seed & (1 << 21)) == 0) SetBit(6);
                if ((Seed & (1 << 22)) > 0) SetBit(24);
                if ((Seed & (1 << 23)) > 0) SetBit(0);
                if ((Seed & (1 << 24)) == 0) SetBit(5);
                if ((Seed & (1 << 25)) > 0) SetBit(1);
                if ((Seed & (1 << 26)) > 0) SetBit(22);

                if ((LogicDifficulty & 1) == 0) SetBit(4);
                if ((LogicDifficulty & 2) > 0) SetBit(30);
                if ((StartingLocation & 1) > 0) SetBit(9);
                if ((StartingLocation & 2) > 0) SetBit(39);
                if ((StartingLocation & 4) == 0) SetBit(33);
                if ((StartingLocation & 8) == 0) SetBit(25);

                if (ShuffleReliquaries) SetBit(7);
                if (ShuffleDash) SetBit(37);
                if (!ShuffleWallClimb) SetBit(27);
                if (ShuffleBootsOfPleading) SetBit(15);
                if (!ShufflePurifiedHand) SetBit(44);

                if (!ShuffleSwordSkills) SetBit(2);
                if (!ShuffleThorns) SetBit(21);
                if (JunkLongQuests) SetBit(14);
                if (!StartWithWheel) SetBit(40);

                if (EnemyShuffleType > 0) SetBit(17);
                //if ((BossShuffleType & 1) == 0) SetBit(17);
                //if ((BossShuffleType & 2) > 0) SetBit(43);
                if ((DoorShuffleType & 1) > 0) SetBit(26);
                if ((DoorShuffleType & 2) == 0) SetBit(34);

                return uniqueSeed;

                void SetBit(byte digit)
                {
                    uniqueSeed |= ((long)1) << digit;
                }
            }
        }

        /// <summary>
        /// The maximum seed allowed by the randomizer
        /// </summary>
        public const int MAX_SEED = 99_999_999;

        /// <summary>
        /// Does this starting location allow shuffleDash to be true
        /// </summary>
        public static bool DoesLocationAllowDash(int startingLocation, int doorShuffle)
        {
            if (startingLocation >= STARTING_LOCATIONS.Length)
                return true;

            StartFlags flags = STARTING_LOCATIONS[startingLocation].StartFlags;
            return doorShuffle > 1 || (flags & StartFlags.RequiresDash) == 0;
        }

        /// <summary>
        /// Does this starting location allow shuffleWallClimb to be true
        /// </summary>
        public static bool DoesLocationAllowWallClimb(int startingLocation, int doorShuffle)
        {
            if (startingLocation >= STARTING_LOCATIONS.Length)
                return true;
            
            StartFlags flags = STARTING_LOCATIONS[startingLocation].StartFlags;
            return doorShuffle > 1 || (flags & StartFlags.RequiresWallClimb) == 0;
        }

        private static readonly StartingLocation[] STARTING_LOCATIONS = new StartingLocation[]
        {
            //new StartingLocation("D01Z04S01", "D01Z04S01[W]", new Vector3(-121, -27, 0), true),
            //new StartingLocation("D05Z01S03", "D05Z01S03[W]", new Vector3(318, -4, 0), false),
            new StartingLocation("D17Z01S01", "D17Z01S01[E]", new Vector3(-988, 20, 0), true, StartFlags.RequiresDash),
            new StartingLocation("D01Z02S01", "D01Z02S01[E]", new Vector3(-512, 11, 0), false, StartFlags.None),
            new StartingLocation("D02Z03S09", "D02Z03S09[E]", new Vector3(-577, 250, 0), true, StartFlags.None),
            new StartingLocation("D03Z03S11", "D03Z03S11[E]", new Vector3(-551, -236, 0), true, StartFlags.RequiresWallClimb),
            new StartingLocation("D04Z03S01", "D04Z03S01[W]", new Vector3(353, 19, 0), false, StartFlags.RequiresHardMode),
            new StartingLocation("D06Z01S09", "D06Z01S09[W]", new Vector3(374, 175, 0), false, StartFlags.RequiresHardMode),
            new StartingLocation("D20Z02S09", "D20Z02S09[W]", new Vector3(130, -136, 0), true, StartFlags.RequiresDash | StartFlags.RequiresHardMode),
        };
    }
}
