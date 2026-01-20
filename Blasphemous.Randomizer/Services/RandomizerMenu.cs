using Blasphemous.Framework.Menus;
using Blasphemous.Framework.Menus.Options;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.Randomizer.Extensions;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Randomizer.Services;

/// <summary>
/// Handles choosing settings for the randomizer
/// </summary>
public class RandomizerMenu : ModMenu
{
    /// <summary>
    /// This menu should open near the very end
    /// </summary>
    protected override int Priority { get; } = 1_000_000;

    private ArrowOption _logicDifficulty;
    private ArrowOption _startingLocation;
    private ToggleOption _hints;
    private ToggleOption _penitence;
    private ArrowOption _doorShuffle;
    private ArrowOption _enemyShuffle;
    private ToggleOption _maintainClass;
    private ToggleOption _areaScaling;
    private ToggleOption _reliquaries;
    private ToggleOption _dash;
    private ToggleOption _wallClimb;
    private ToggleOption _boots;
    private ToggleOption _purifiedHand;
    private ToggleOption _swordSkills;
    private ToggleOption _thorns;
    private ToggleOption _junkQuests;
    private ToggleOption _wheel;
    private TextOption _seedText;

    private Text _idText;

    private int _generatedSeed;

    /// <summary>
    /// Gets or sets the current status of the menu options
    /// </summary>
    public Config MenuSettings
    {
        get
        {
            return new Config()
            {
                LogicDifficulty = _logicDifficulty.CurrentOption,
                StartingLocation = _startingLocation.CurrentOption,
                AllowHints = _hints.Toggled,
                AllowPenitence = _penitence.Toggled,

                DoorShuffleType = _doorShuffle.CurrentOption,
                BossShuffleType = 0,
                EnemyShuffleType = _enemyShuffle.CurrentOption,
                MaintainClass = _maintainClass.Toggled,
                AreaScaling = _areaScaling.Toggled,

                ShuffleReliquaries = _reliquaries.Toggled,
                ShuffleDash = _dash.Toggled,
                ShuffleWallClimb = _wallClimb.Toggled,
                ShuffleBootsOfPleading = _boots.Toggled,
                ShufflePurifiedHand = _purifiedHand.Toggled,

                ShuffleSwordSkills = _swordSkills.Toggled,
                ShuffleThorns = _thorns.Toggled,
                JunkLongQuests = _junkQuests.Toggled,
                StartWithWheel = _wheel.Toggled,

                Seed = _seedText.CurrentValue != string.Empty ? _seedText.CurrentNumericValue : _generatedSeed
            };
        }
        set
        {
            _logicDifficulty.CurrentOption = value.LogicDifficulty;
            _startingLocation.CurrentOption = value.StartingLocation;
            _hints.Toggled = value.AllowHints;
            _penitence.Toggled = value.AllowPenitence;

            _doorShuffle.CurrentOption = value.DoorShuffleType;
            _enemyShuffle.CurrentOption = value.EnemyShuffleType;
            _maintainClass.Toggled = value.MaintainClass;
            _areaScaling.Toggled = value.AreaScaling;

            _reliquaries.Toggled = value.ShuffleReliquaries;
            _dash.Toggled = value.ShuffleDash;
            _wallClimb.Toggled = value.ShuffleWallClimb;
            _boots.Toggled = value.ShuffleBootsOfPleading;
            _purifiedHand.Toggled = value.ShufflePurifiedHand;

            _swordSkills.Toggled = value.ShuffleSwordSkills;
            _thorns.Toggled = value.ShuffleThorns;
            _junkQuests.Toggled = value.JunkLongQuests;
            _wheel.Toggled = value.StartWithWheel;

            _seedText.CurrentValue = value.Seed > 0 ? value.Seed.ToString() : string.Empty;
            OnOptionsChanged();
        }
    }

    /// <summary>
    /// When first opening the menus, generate a random seed and fill in the options
    /// </summary>
    public override void OnStart()
    {
        _generatedSeed = Config.RandomSeed;
        ModLog.Info($"Generating default seed: {_generatedSeed}");

        MenuSettings = new Config();
    }

    /// <summary>
    /// When closing the menus, store the game settings
    /// </summary>
    public override void OnFinish()
    {
        Config settings = MenuSettings;

        ModLog.Info($"Storing menu settings: {settings.Seed}");
        Main.Randomizer.GameSettings = settings;
    }

    /// <summary>
    /// Whenever an option is changed, update enabled status and the unique seed
    /// </summary>
    public override void OnOptionsChanged()
    {
        int doorType = _doorShuffle.CurrentOption;
        int startLocation = _startingLocation.CurrentOption;

        _boots.Enabled = Main.Randomizer.InstalledBootsMod;
        _purifiedHand.Enabled = Main.Randomizer.InstalledDoubleJumpMod;
        _maintainClass.Enabled = _enemyShuffle.CurrentOption > 0;
        _areaScaling.Enabled = _enemyShuffle.CurrentOption > 0;
        _dash.Enabled = Config.DoesLocationAllowDash(startLocation, doorType);
        _wallClimb.Enabled = Config.DoesLocationAllowWallClimb(startLocation, doorType);

        UpdateUniqueIdText(MenuSettings.CalculateUID());
    }

    /// <summary>
    /// Updates the UID text with the specified id
    /// </summary>
    private void UpdateUniqueIdText(ulong id)
    {
        var sb = new StringBuilder();
        ulong targetBase = (ulong)ID_CHARS.Length;
        int idx = 0;

        do
        {
            sb.Append($" {ID_CHARS[(int)(id % targetBase)]}");
            id /= targetBase;

            if (++idx % 4 == 0 && idx != ID_DIGITS)
                sb.Append(" -");
        }
        while (id > 0);

        while (sb.Length < ID_DIGITS * 2)
        {
            sb.Append(" 0");

            if (++idx % 4 == 0 && idx != ID_DIGITS)
                sb.Append(" -");
        }

        _idText.text = $"Unique ID:<color=#B3E5B3>{sb}</color>";
    }

    /// <summary>
    /// Create all options for the menu
    /// </summary>
    protected override void CreateUI(Transform ui)
    {
        // Setup

        ToggleCreator toggle = new(this);
        ArrowCreator arrow = new(this);
        TextCreator text = new(this)
        {
            TextSize = 50,
            LineSize = 200
        };

        int xOffset = -100;
        int yOffset = 300;
        Transform currSection = CreateSection(ui, 0);
        string[] standardOptions = new string[] { "distyp", "simtyp", "fultyp" }.Select(Main.Randomizer.LocalizationHandler.Localize).ToArray();

        // Seed

        string seedName = Main.Randomizer.LocalizationHandler.Localize("menusd") + ":";
        _seedText = text.CreateOption("seed", ui, new Vector2(-20, 300), seedName, true, false, Config.MAX_SEED.GetDigits());

        // General section

        string logicName = Main.Randomizer.LocalizationHandler.Localize("lgname") + ":";
        string[] logicOptions = Enumerable.Range(1, 3).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lgtyp{x}")).ToArray();
        _logicDifficulty = arrow.CreateOption("Logic difficulty", currSection, new Vector2(0, yOffset -= 100), logicName, logicOptions);

        string startName = Main.Randomizer.LocalizationHandler.Localize("loname") + ":";
        string[] startOptions = Enumerable.Range(1, 8).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lotyp{x}")).ToArray();
        _startingLocation = arrow.CreateOption("Starting location", currSection, new Vector2(0, yOffset -= 100), startName, startOptions);

        string hintName = Main.Randomizer.LocalizationHandler.Localize("htname");
        _hints = toggle.CreateOption("Hints", currSection, new Vector2(xOffset, yOffset -= 100), hintName);

        string penitenceName = Main.Randomizer.LocalizationHandler.Localize("pename");
        _penitence = toggle.CreateOption("Penitence", currSection, new Vector2(xOffset, yOffset -= 100), penitenceName);

        CreateDivider(currSection);
        currSection = CreateSection(ui, 1);
        yOffset = 300;

        // Item first section

        string reliqName = Main.Randomizer.LocalizationHandler.Localize("rqname");
        _reliquaries = toggle.CreateOption("Reliquaries", currSection, new Vector2(xOffset, yOffset -= 100), reliqName);

        string dashName = Main.Randomizer.LocalizationHandler.Localize("dsname");
        _dash = toggle.CreateOption("Dash", currSection, new Vector2(xOffset, yOffset -= 100), dashName);

        string wallClimbName = Main.Randomizer.LocalizationHandler.Localize("wcname");
        _wallClimb = toggle.CreateOption("Wall climb", currSection, new Vector2(xOffset, yOffset -= 100), wallClimbName);

        string bootsName = Main.Randomizer.LocalizationHandler.Localize("sbname");
        _boots = toggle.CreateOption("Boots", currSection, new Vector2(xOffset, yOffset -= 100), bootsName);

        string doubleJumpName = Main.Randomizer.LocalizationHandler.Localize("djname");
        _purifiedHand = toggle.CreateOption("Double jump", currSection, new Vector2(xOffset, yOffset -= 100), doubleJumpName);

        currSection = CreateSection(ui, 2);
        yOffset = 300;

        // Item second section

        string swordSkillsName = Main.Randomizer.LocalizationHandler.Localize("ssname");
        _swordSkills = toggle.CreateOption("Sword skills", currSection, new Vector2(xOffset, yOffset -= 100), swordSkillsName);

        string thornsName = Main.Randomizer.LocalizationHandler.Localize("thname");
        _thorns = toggle.CreateOption("Thorns", currSection, new Vector2(xOffset, yOffset -= 100), thornsName);

        string junkName = Main.Randomizer.LocalizationHandler.Localize("jiname");
        _junkQuests = toggle.CreateOption("Junk inconvenitent", currSection, new Vector2(xOffset, yOffset -= 100), junkName);

        string wheelName = Main.Randomizer.LocalizationHandler.Localize("whname");
        _wheel = toggle.CreateOption("Wheel", currSection, new Vector2(xOffset, yOffset -= 100), wheelName);

        CreateDivider(currSection);
        currSection = CreateSection(ui, 3);
        yOffset = 300;

        // Enemy section

        string enemyName = Main.Randomizer.LocalizationHandler.Localize("enname") + ":";
        _enemyShuffle = arrow.CreateOption("Enemy shuffle", currSection, new Vector2(0, yOffset -= 100), enemyName, standardOptions);

        string className = Main.Randomizer.LocalizationHandler.Localize("clname");
        _maintainClass = toggle.CreateOption("Maintain class", currSection, new Vector2(xOffset, yOffset -= 100), className);

        string scalingName = Main.Randomizer.LocalizationHandler.Localize("scname");
        _areaScaling = toggle.CreateOption("Area scaling", currSection, new Vector2(xOffset, yOffset -= 100), scalingName);

        CreateDivider(currSection);
        currSection = CreateSection(ui, 4);
        yOffset = 300;

        // Door section

        string doorName = Main.Randomizer.LocalizationHandler.Localize("drname") + ":";
        _doorShuffle = arrow.CreateOption("Door shuffle", currSection, new Vector2(0, yOffset -= 100), doorName, standardOptions);

        // Exclude linen drop doors

        // Lower section

        _idText = UIModder.Create(new RectCreationOptions()
        {
            Name = "UniqueID",
            Parent = ui,
            Position = new Vector2(0, -165),
            Pivot = Vector2.zero,
            XRange = Vector2.zero,
            YRange = Vector2.zero,
        }).AddText(new TextCreationOptions()
        {
            Contents = "Unique ID: ---",
            Color = new Color32(192, 192, 192, 255),
            Alignment = TextAnchor.MiddleLeft,
            FontSize = 42,
        });
        _idText.supportRichText = true;
    }

    private RectTransform CreateSection(Transform parent, int idx)
    {
        float size = (FAR_RIGHT - FAR_LEFT) / NUM_SECTIONS;

        return UIModder.Create(new RectCreationOptions()
        {
            Name = $"Section {idx + 1}",
            Parent = parent,
            Pivot = new Vector2(0.5f, 1),
            XRange = new Vector2(FAR_LEFT + idx * size, FAR_LEFT + (idx + 1) * size),
            YRange = new Vector2(0, 0.85f),
            Size = Vector2.zero
        });
    }

    private Image CreateDivider(Transform section)
    {
        return UIModder.Create(new RectCreationOptions()
        {
            Name = "Divider",
            Parent = section,
            XRange = Vector2.one,
            YRange = new Vector2(0.15f, 0.85f),
            Size = new Vector2(2, 100),
        }).AddImage(new ImageCreationOptions()
        {
            Color = new Color32(99, 68, 57, 255)
        });
    }

    private const float FAR_LEFT = -0.15f;
    private const float FAR_RIGHT = 1.15f;
    private const float NUM_SECTIONS = 5;
    private const int ID_DIGITS = 12;
    private const string ID_CHARS = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
}
