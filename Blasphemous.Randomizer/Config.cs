
namespace Blasphemous.Randomizer
{
    /// <summary>
    /// Stores the settings for a randomizer
    /// </summary>
    public class Config
    {
        // Main Settings
        public int LogicDifficulty { get; set; } = 1;
        public int StartingLocation { get; set; } = 0;
        public int CustomSeed { get; set; } = 0;

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

        /// <summary>
        /// Gets a random seed within the valid range
        /// </summary>
        public static int RandomSeed => new System.Random().Next(1, MAX_SEED + 1);
        private const int MAX_SEED = 99_999_999;
    }
}
