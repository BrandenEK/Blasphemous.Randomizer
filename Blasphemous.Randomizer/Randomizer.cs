using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.Zones;
using Framework.Managers;
using System.Drawing.Printing;

namespace Blasphemous.Randomizer;

public class Randomizer : BlasMod
{
    public Randomizer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

    public DataHandler DataHandler { get; } = new();
    public ZoneHandler ZoneHandler { get; } = new();

    public RandomizerSettings CurrentSettings { get; private set; }

    public bool InstalledBootsMod => IsModLoaded("Blasphemous.BootsOfPleading", out var _);
    public bool InstalledDoubleJumpMod => IsModLoaded("Blasphemous.DoubleJump", out var _);
    public bool CanDash => !CurrentSettings.ShuffleDash || Core.Events.GetFlag("ITEM_Slide");
    public bool CanWallClimb => !CurrentSettings.ShuffleWallClimb || Core.Events.GetFlag("ITEM_WallClimb");
    public bool DashChecker { get; set; }

    protected override void OnInitialize()
    {
        LogError($"{ModInfo.MOD_NAME} has been initialized");

        DataHandler.Initialize();
    }
}
