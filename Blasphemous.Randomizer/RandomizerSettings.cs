
namespace Blasphemous.Randomizer;

public class RandomizerSettings
{
    // Main Settings
    public int LogicDifficulty { get; set; }
    public int StartingLocation { get; set; }
    public int CustomSeed { get; set; }

    // General Settings
    public bool UnlockTeleportation { get; set; }
    public bool AllowHints { get; set; }
    public bool AllowPenitence { get; set; }

    // Item Rando
    public bool ShuffleReliquaries { get; set; }
    public bool ShuffleDash { get; set; }
    public bool ShuffleWallClimb { get; set; }
    public bool ShuffleBootsOfPleading { get; set; }
    public bool ShufflePurifiedHand { get; set; }

    public bool ShuffleSwordSkills { get; set; }
    public bool ShuffleThorns { get; set; }
    public bool JunkLongQuests { get; set; }
    public bool StartWithWheel { get; set; }

    // Enemy Rando
    public int EnemyShuffleType { get; set; }
    public bool MaintainClass { get; set; }
    public bool AreaScaling { get; set; }

    // Boss Rando
    public int BossShuffleType { get; set; }

    // Door Rando
    public int DoorShuffleType { get; set; }

    public static RandomizerSettings Default
    {
        get
        {
            RandomizerSettings settings = new()
            {
                LogicDifficulty = 1,
                StartingLocation = 0,
                CustomSeed = 0,

                UnlockTeleportation = true,
                AllowHints = true,
                AllowPenitence = false,

                ShuffleReliquaries = true,
                ShuffleBootsOfPleading = false,
                ShufflePurifiedHand = false,
                ShuffleDash = false,
                ShuffleWallClimb = false,

                ShuffleSwordSkills = true,
                ShuffleThorns = true,
                JunkLongQuests = true,
                StartWithWheel = false,

                EnemyShuffleType = 0,
                MaintainClass = true,
                AreaScaling = true,

                BossShuffleType = 0,

                DoorShuffleType = 0
            };

            return settings;
        }
    }

    public const int MAX_SEED = 99_999_999;
}
