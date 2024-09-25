using Blasphemous.Framework.Menus;
using Blasphemous.Framework.Menus.Options;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Blasphemous.Randomizer.Extensions;
using Framework.Managers;
using System.Linq;
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
    private ToggleOption _teleportation;
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

    private Image[] _uniqueImages;

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
                UnlockTeleportation = _teleportation.Toggled,
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
            _teleportation.Toggled = value.UnlockTeleportation;
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
        _dash.Enabled = doorType > 1 || startLocation != Config.BROTHERHOOD_LOCATION && startLocation != Config.SHIPYARD_LOCATION;
        _wallClimb.Enabled = doorType > 1 || startLocation != Config.DEPTHS_LOCATION;

        RefreshUniqueSeed();
    }

    /// <summary>
    /// Set each digit in the unique seed to its corresponding image
    /// </summary>
    private void RefreshUniqueSeed()
    {
        // Get final seed based on seed & options
        long finalSeed = MenuSettings.UniqueSeed;

        // Fill images based on unique seed
        try
        {
            FillImages(finalSeed);
        }
        catch
        {
            ModLog.Error("Failed to generate image layout for unique seed " + finalSeed);
            for (int i = 0; i < _uniqueImages.Length; i++)
                _uniqueImages[i].sprite = GetIcon(0);
        }

        void FillImages(long seed)
        {
            int numDigits = UNIQUE_ICONS.Length, currDigit = 0;
            do
            {
                int imgIdx = (int)(seed % numDigits);
                seed /= numDigits;

                SetDigitImage(currDigit, GetIcon(imgIdx));
                currDigit++;
            }
            while (seed > 0);
            for (; currDigit < _uniqueImages.Length; currDigit++)
                SetDigitImage(currDigit, GetIcon(0));
        }

        Sprite GetIcon(int index)
        {
            string itemId = UNIQUE_ICONS[index];
            InventoryManager.ItemType itemType = ItemHelper.GetItemTypeFromId(itemId);
            return Core.InventoryManager.GetBaseObject(itemId, itemType).picture;
        }

        void SetDigitImage(int digit, Sprite image)
        {
            // Need to update this when adding more images
            int realIdx = digit switch
            {
                0 => 1,
                1 => 5,
                2 => 0,
                3 => 3,
                4 => 6,
                5 => 4,
                6 => 2,
                _ => throw new System.Exception("Too many digits in unique seed!")
            };

            _uniqueImages[realIdx].sprite = image;
        }
    }

    /// <summary>
    /// Create all options for the menu
    /// </summary>
    protected override void CreateUI(Transform ui)
    {
        // Create unique seed images
        CreateUniqueSeed(ui);

        // Create sections
        RectTransform section1 = CreateSection(ui, 0);
        RectTransform section2 = CreateSection(ui, 1);
        RectTransform section3 = CreateSection(ui, 2);
        RectTransform section4 = CreateSection(ui, 3);

        // Create option creators
        ToggleCreator toggle = new(this);
        ArrowCreator arrow = new(this);
        TextCreator text = new(this)
        {
            TextSize = 50,
            LineSize = 200
        };

        // Create options
        int xOffset = -100;
        string[] standardOptions = new string[] { "distyp", "simtyp", "fultyp" }.Select(Main.Randomizer.LocalizationHandler.Localize).ToArray();

        // Section 0

        string seedName = Main.Randomizer.LocalizationHandler.Localize("menusd") + ":";
        _seedText = text.CreateOption("seed", ui, new Vector2(-20, 300), seedName, true, false, Config.MAX_SEED.GetDigits());

        // Section 1

        string logicName = Main.Randomizer.LocalizationHandler.Localize("lgname") + ":";
        string[] logicOptions = Enumerable.Range(1, 3).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lgtyp{x}")).ToArray();
        _logicDifficulty = arrow.CreateOption("Logic difficulty", section1, new Vector2(0, 200), logicName, logicOptions);

        string startName = Main.Randomizer.LocalizationHandler.Localize("loname") + ":";
        string[] startOptions = Enumerable.Range(1, 8).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lotyp{x}")).ToArray();
        _startingLocation = arrow.CreateOption("Starting location", section1, new Vector2(0, 100), startName, startOptions);

        string teleportName = Main.Randomizer.LocalizationHandler.Localize("tpname");
        _teleportation = toggle.CreateOption("Teleportation", section1, new Vector2(xOffset, 0), teleportName);

        string hintName = Main.Randomizer.LocalizationHandler.Localize("htname");
        _hints = toggle.CreateOption("Hints", section1, new Vector2(xOffset, -100), hintName);

        string penitenceName = Main.Randomizer.LocalizationHandler.Localize("pename");
        _penitence = toggle.CreateOption("Penitence", section1, new Vector2(xOffset, -200), penitenceName);

        // Section 2

        string doorName = Main.Randomizer.LocalizationHandler.Localize("drname") + ":";
        _doorShuffle = arrow.CreateOption("Door shuffle", section2, new Vector2(0, 200), doorName, standardOptions);

        string enemyName = Main.Randomizer.LocalizationHandler.Localize("enname") + ":";
        _enemyShuffle = arrow.CreateOption("Enemy shuffle", section2, new Vector2(0, 100), enemyName, standardOptions);

        string className = Main.Randomizer.LocalizationHandler.Localize("clname");
        _maintainClass = toggle.CreateOption("Maintain class", section2, new Vector2(xOffset, 0), className);

        string scalingName = Main.Randomizer.LocalizationHandler.Localize("scname");
        _areaScaling = toggle.CreateOption("Area scaling", section2, new Vector2(xOffset, -100), scalingName);

        // Section 3

        string reliqName = Main.Randomizer.LocalizationHandler.Localize("rqname");
        _reliquaries = toggle.CreateOption("Reliquaries", section3, new Vector2(xOffset, 200), reliqName);

        string dashName = Main.Randomizer.LocalizationHandler.Localize("dsname");
        _dash = toggle.CreateOption("Dash", section3, new Vector2(xOffset, 100), dashName);

        string wallClimbName = Main.Randomizer.LocalizationHandler.Localize("wcname");
        _wallClimb = toggle.CreateOption("Wall climb", section3, new Vector2(xOffset, 0), wallClimbName);

        string bootsName = Main.Randomizer.LocalizationHandler.Localize("sbname");
        _boots = toggle.CreateOption("Boots", section3, new Vector2(xOffset, -100), bootsName);

        string doubleJumpName = Main.Randomizer.LocalizationHandler.Localize("djname");
        _purifiedHand = toggle.CreateOption("Double jump", section3, new Vector2(xOffset, -200), doubleJumpName);

        // Section 4

        string swordSkillsName = Main.Randomizer.LocalizationHandler.Localize("ssname");
        _swordSkills = toggle.CreateOption("Sword skills", section4, new Vector2(xOffset, 200), swordSkillsName);

        string thornsName = Main.Randomizer.LocalizationHandler.Localize("thname");
        _thorns = toggle.CreateOption("Thorns", section4, new Vector2(xOffset, 100), thornsName);

        string junkName = Main.Randomizer.LocalizationHandler.Localize("jiname");
        _junkQuests = toggle.CreateOption("Junk inconvenitent", section4, new Vector2(xOffset, 0), junkName);

        string wheelName = Main.Randomizer.LocalizationHandler.Localize("whname");
        _wheel = toggle.CreateOption("Wheel", section4, new Vector2(xOffset, -100), wheelName);
    }

    private RectTransform CreateSection(Transform parent, int idx)
    {
        return UIModder.Create(new RectCreationOptions()
        {
            Name = $"Section {idx + 1}",
            Parent = parent,
            Pivot = new Vector2(0.5f, 1),
            XRange = new Vector2(idx * 0.25f, (idx + 1) * 0.25f),
            YRange = new Vector2(0, 0.85f),
            Size = Vector2.zero
        });
    }

    private void CreateUniqueSeed(Transform parent)
    {
        RectTransform holder = UIModder.Create(new RectCreationOptions()
        {
            Name = "Unique holder",
            Parent = parent,
            XRange = Vector2.one,
            YRange = Vector2.one,
            Pivot = new Vector2(1, 0.5f),
            Position = new Vector2(190, 205),
            Size = new Vector2(100, 96)
        });

        _uniqueImages = new Image[UNIQUE_ID_SIZE];
        for (int i = 0; i < UNIQUE_ID_SIZE; i++)
        {
            _uniqueImages[i] = UIModder.Create(new RectCreationOptions()
            {
                Name = $"Img{i}",
                Parent = holder,
                XRange = new Vector2(1, 1),
                YRange = new Vector2(0.5f, 0.5f),
                Pivot = new Vector2(1, 0.5f),
                Position = new Vector2(i * -60, 0),
                Size = new Vector2(64, 64)
            }).AddImage();
        }
    }

    private const int UNIQUE_ID_SIZE = 7;
    private const int NUMBER_OF_OPTIONS = 25;
    readonly string[] UNIQUE_ICONS = new string[] // 96 diff images (5 images = 32 bits, 6 images = 39 bits, 7 images = 46 bits)
    {
        "RE01", "RE02", "RE03", "RE04", "RE05", "RE07", "RE10",
        "RB01", "RB03", "RB04", "RB05", "RB06", "RB07", "RB08", "RB09", "RB10", "RB11", "RB12", "RB13", "RB14", "RB15", "RB16", "RB21", "RB30", "RB31", "RB32", "RB33", "RB35", "RB36", "RB37", "RB101", "RB102", "RB103", "RB105", "RB106", "RB107", "RB108", "RB201", "RB202", "RB203", "RB301",
        "HE01", "HE02", "HE03", "HE04", "HE05", "HE06", "HE07", "HE10", "HE11", "HE101", "HE201",
        "PR03", "PR04", "PR05", "PR07", "PR08", "PR09", "PR10", "PR11", "PR12", "PR14", "PR15", "PR16", "PR101", "PR201", "PR202", "PR203",
        "QI01", "QI06", "QI07", "QI08", "QI10", "QI11", "QI12", "QI19", "QI20", "QI37", "QI41", "QI44", "QI57", "QI58", "QI61", "QI68", "QI69", "QI70", "QI71", "QI75", "QI78", "QI81", "QI101", "QI110", "QI202", "QI203", "QI204", "QI301",
    };
}
