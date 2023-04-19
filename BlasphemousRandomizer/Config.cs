
namespace BlasphemousRandomizer
{
    [System.Serializable]
    public class Config
    {
        // Main Settings
        public int LogicDifficulty { get; set; }
        public int StartingLocation { get; set; }
        public int CustomSeed { get; set; }
        public string VersionCreated { get; set; }

        // General Settings
        public bool UnlockTeleportation { get; set; }
        public bool AllowHints { get; set; }
        public bool AllowPenitence { get; set; }

        // Item Rando
        public bool ShuffleReliquaries { get; set; }
        public bool ShuffleBootsOfPleading { get; set; }
        public bool ShufflePurifiedHand { get; set; }
        public bool ShuffleSwordSkills { get; set; }
        public bool ShuffleThorns { get; set; }
        public bool ForceMiriamFiller { get; set; }
        public bool StartWithWheel { get; set; }

        // Enemy Rando
        public int EnemyShuffleType { get; set; }
        public bool MaintainClass { get; set; }
        public bool AreaScaling { get; set; }

        // Door Rando
        public int DoorShuffleType { get; set; }

        // Create Config with default options
        public Config()
        {
            LogicDifficulty = 1;
            StartingLocation = 0;
            CustomSeed = 0;
            VersionCreated = Main.Randomizer.ModVersion;

            UnlockTeleportation = true;
            AllowHints = true;
            AllowPenitence = false;

            ShuffleReliquaries = true;
            ShuffleBootsOfPleading = false;
            ShufflePurifiedHand = false;
            ShuffleSwordSkills = true;
            ShuffleThorns = true;
            ForceMiriamFiller = true;
            StartWithWheel = false;

            EnemyShuffleType = 0;
            MaintainClass = true;
            AreaScaling = true;

            DoorShuffleType = 0;
        }
    }
}
