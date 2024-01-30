using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.Bosses;
using Blasphemous.Randomizer.Doors;
using Blasphemous.Randomizer.Zones;
using Framework.Managers;

namespace Blasphemous.Randomizer;

public class Randomizer : BlasMod
{
    public Randomizer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public BossHandler BossHandler { get; } = new();
    public DataHandler DataHandler { get; } = new();
    public DoorHandler DoorHandler { get; } = new();
    public ZoneHandler ZoneHandler { get; } = new();

    public RandomizerSettings CurrentSettings { get; private set; }

    public bool InstalledBootsMod => IsModLoaded("Blasphemous.BootsOfPleading", out var _);
    public bool InstalledDoubleJumpMod => IsModLoaded("Blasphemous.DoubleJump", out var _);
    public bool CanDash => !CurrentSettings.ShuffleDash || Core.Events.GetFlag("ITEM_Slide");
    public bool CanWallClimb => !CurrentSettings.ShuffleWallClimb || Core.Events.GetFlag("ITEM_WallClimb");
    public bool DashChecker { get; set; }

    protected override void OnInitialize()
    {
        DataHandler.Initialize();
        // Init all shufflers ?

        CurrentSettings = RandomizerSettings.Default;
        Log("Randomizer has been initialized!");
    }

    protected override void OnLevelPreloaded(string oldLevel, string newLevel)
    {
        DoorHandler.LevelPreloaded(newLevel);
    }

    protected override void OnLevelLoaded(string oldLevel, string newLevel)
    {
        DoorHandler.LevelLoaded(newLevel);
        DoorHandler.FixRooftopsElevator(newLevel);
    }
}
