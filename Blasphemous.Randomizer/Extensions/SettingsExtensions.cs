
namespace Blasphemous.Randomizer.Extensions;

public static class SettingsExtensions
{
    /// <summary>
    /// Calculates a unique ID based on the seed and all settings
    /// </summary>
    public static ulong CalculateUID(this Config settings)
    {
        ulong uid = 0;
        int idx = 0;
        bool flip = false;

        // Seed

        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_01) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_02) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_04) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_08) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_10) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_20) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_40) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_00_80) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_01_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_02_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_04_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_08_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_10_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_20_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_40_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_00_80_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_01_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_02_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_04_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_08_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_10_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_20_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_40_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x00_80_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x01_00_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x02_00_00_00) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.Seed & 0x04_00_00_00) > 0);

        // General

        SetBit(ref uid, ref idx, ref flip, (settings.LogicDifficulty + 1 & 0x01) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.LogicDifficulty + 1 & 0x02) > 0);

        SetBit(ref uid, ref idx, ref flip, (settings.StartingLocation + 1 & 0x01) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.StartingLocation + 1 & 0x02) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.StartingLocation + 1 & 0x04) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.StartingLocation + 1 & 0x08) > 0);

        SetBit(ref uid, ref idx, ref flip, settings.AllowHints);
        SetBit(ref uid, ref idx, ref flip, settings.AllowPenitence);

        // Items

        SetBit(ref uid, ref idx, ref flip, settings.ShuffleReliquaries);
        SetBit(ref uid, ref idx, ref flip, settings.ShuffleDash);
        SetBit(ref uid, ref idx, ref flip, settings.ShuffleWallClimb);
        SetBit(ref uid, ref idx, ref flip, settings.ShuffleBootsOfPleading);
        SetBit(ref uid, ref idx, ref flip, settings.ShufflePurifiedHand);

        SetBit(ref uid, ref idx, ref flip, settings.ShuffleSwordSkills);
        SetBit(ref uid, ref idx, ref flip, settings.ShuffleThorns);
        SetBit(ref uid, ref idx, ref flip, settings.JunkLongQuests);
        SetBit(ref uid, ref idx, ref flip, settings.StartWithWheel);

        // Enemies

        SetBit(ref uid, ref idx, ref flip, (settings.EnemyShuffleType + 1 & 0x01) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.EnemyShuffleType + 1 & 0x02) > 0);

        SetBit(ref uid, ref idx, ref flip, settings.MaintainClass);
        SetBit(ref uid, ref idx, ref flip, settings.AreaScaling);

        // Doors

        SetBit(ref uid, ref idx, ref flip, (settings.DoorShuffleType + 1 & 0x01) > 0);
        SetBit(ref uid, ref idx, ref flip, (settings.DoorShuffleType + 1 & 0x02) > 0);

        return uid;
    }

    /// <summary>
    /// Sets a bit for the UID based on the bit order and flip status
    /// </summary>
    private static void SetBit(ref ulong uid, ref int idx, ref bool flip, bool value)
    {
        byte bit = BIT_ORDER[idx++];
        flip = !flip;

        if (value ^ flip)
            uid |= (ulong)1 << bit;
    }

    private static readonly byte[] BIT_ORDER =
    [
        35, 30, 46, 34, 33, 12, 16, 25, 01, 22, 05, 07, 21, 36, 38,
        09, 20, 42, 24, 29, 59, 11, 54, 41, 40, 53, 15, 39, 52, 32,
        57, 02, 23, 45, 55, 43, 19, 27, 04, 44, 28, 58, 17, 13, 18,
        06, 47, 37, 14, 10, 26, 03, 56, 51, 08, 49, 48, 31, 00, 50,
    ];
}
