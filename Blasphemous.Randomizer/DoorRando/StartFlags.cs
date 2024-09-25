
namespace Blasphemous.Randomizer.DoorRando;

/// <summary>
/// Flags that determine whether a location is allowed to start from
/// </summary>
[System.Flags]
public enum StartFlags
{
    /// <summary>
    /// This location requires nothing
    /// </summary>
    None = 0x00,
    /// <summary>
    /// Does this location require dash to start from
    /// </summary>
    RequiresDash = 0x01,
    /// <summary>
    /// Does this location require wall climb to start from
    /// </summary>
    RequiresWallClimb = 0x02,
    /// <summary>
    /// Does this location require normal or hard difficulty
    /// </summary>
    RequiresNormalMode = 0x04,
    /// <summary>
    /// Does this location require hard difficulty
    /// </summary>
    RequiresHardMode = 0x08
}
