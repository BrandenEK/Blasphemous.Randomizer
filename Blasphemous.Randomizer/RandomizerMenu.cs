using Blasphemous.Framework.Menus;
using Blasphemous.Framework.Menus.Options;
using Blasphemous.Framework.UI;
using Blasphemous.ModdingAPI;
using Framework.Managers;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Randomizer;

public class RandomizerMenu : ModMenu
{
    public RandomizerMenu() : base("Randomizer Settings (Localize)", 10) { }

    private ArrowOption _logicDifficulty;
    private ArrowOption _startingLocation;
    private ToggleOption _teleportation;
    private ToggleOption _hints;
    private ToggleOption _penitence;

    private ToggleOption _reliquaries;
    private ToggleOption _dash;
    private ToggleOption _wallClimb;
    private ToggleOption _boots;
    private ToggleOption _purifiedHand;

    private ToggleOption _swordSkills;
    private ToggleOption _thorns;
    private ToggleOption _junkQuests;
    private ToggleOption _wheel;

    private ArrowOption _doorShuffle;
    private ArrowOption _enemyShuffle;
    private ToggleOption _maintainClass;
    private ToggleOption _areaScaling;

    private TextOption _seedText;

    private Image[] _uniqueImages;

    public override void OnShow()
    {
        RefreshUniqueSeed();
    }

    public override void OnOptionsChanged()
    {
        RefreshUniqueSeed();
    }

    private void RefreshUniqueSeed()
    {
        // Get final seed based on seed & options
        //long finalSeed = Main.Randomizer.ComputeFinalSeed(currentSeed != string.Empty ? int.Parse(currentSeed) : generatedSeed, getConfigSettings());
        long finalSeed = Main.Randomizer.ComputeFinalSeed(555, new Config());

        // Fill images based on unique seed
        try
        {
            FillImages(finalSeed);
        }
        catch (System.Exception)
        {
            Main.Randomizer.LogError("Failed to generate image layout for unique seed " + finalSeed);
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
            InventoryManager.ItemType itemType = ItemModder.GetItemTypeFromId(itemId);
            return Core.InventoryManager.GetBaseObject(itemId, itemType).picture;
        }

        void SetDigitImage(int digit, Sprite image) // Need to update this when adding more images
        {
            int realIdx = -1;
            if (digit == 0) realIdx = 1;
            else if (digit == 1) realIdx = 5;
            else if (digit == 2) realIdx = 0;
            else if (digit == 3) realIdx = 3;
            else if (digit == 4) realIdx = 6;
            else if (digit == 5) realIdx = 4;
            else if (digit == 6) realIdx = 2;

            if (realIdx < 0)
            {
                Main.Randomizer.LogError("Error: Too many digits in unique seed!");
                return;
            }
            _uniqueImages[realIdx].sprite = image;
        }
    }

    protected override void CreateUI(Transform ui)
    {
        // Create unique seed images
        CreateUniqueSeed(ui);

        // Create sections
        RectTransform section1 = CreateSection(ui, 0); section1.AddImage(new ImageCreationOptions() { Color = Color.red });
        RectTransform section2 = CreateSection(ui, 1); section2.AddImage(new ImageCreationOptions() { Color = Color.green });
        RectTransform section3 = CreateSection(ui, 2); section3.AddImage(new ImageCreationOptions() { Color = Color.blue });
        RectTransform section4 = CreateSection(ui, 3); section4.AddImage(new ImageCreationOptions() { Color = Color.white });

        // Create option creators
        ToggleCreator toggle = new(this)
        {

        };
        ArrowCreator arrow = new(this)
        {
            
        };
        TextCreator text = new(this)
        {
            TextSize = 50,
            LineSize = 200
        };

        // Create options
        _seedText = text.CreateOption("seed", ui, new Vector2(-20, 300), "Localize", true, false, 8);
        string[] standardOptions = new string[] { "distyp", "simtyp", "fultyp" }.Select(Main.Randomizer.LocalizationHandler.Localize).ToArray();

        // Section 1

        string logicName = Main.Randomizer.LocalizationHandler.Localize("lgname") + ":";
        string[] logicOptions = Enumerable.Range(1, 3).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lgtyp{x}")).ToArray();
        _logicDifficulty = arrow.CreateOption("Logic difficulty", section1, new Vector2(0, 200), logicName, logicOptions);

        string startName = Main.Randomizer.LocalizationHandler.Localize("loname") + ":";
        string[] startOptions = Enumerable.Range(1, 8).Select(x => Main.Randomizer.LocalizationHandler.Localize($"lotyp{x}")).ToArray();
        _startingLocation = arrow.CreateOption("Starting location", section1, new Vector2(0, 100), startName, startOptions);

        string teleportName = Main.Randomizer.LocalizationHandler.Localize("tpname");
        _teleportation = toggle.CreateOption("Teleportation", section1, new Vector2(0, 0), teleportName);

        string hintName = Main.Randomizer.LocalizationHandler.Localize("htname");
        _hints = toggle.CreateOption("Hints", section1, new Vector2(0, -100), hintName);

        string penitenceName = Main.Randomizer.LocalizationHandler.Localize("pename");
        _penitence = toggle.CreateOption("Penitence", section1, new Vector2(0, -200), penitenceName);

        // Section 2

        string doorName = Main.Randomizer.LocalizationHandler.Localize("drname") + ":";
        _doorShuffle = arrow.CreateOption("Door shuffle", section2, new Vector2(0, 200), doorName, standardOptions);

        string enemyName = Main.Randomizer.LocalizationHandler.Localize("enname") + ":";
        _enemyShuffle = arrow.CreateOption("Enemy shuffle", section2, new Vector2(0, 100), enemyName, standardOptions);

        string className = Main.Randomizer.LocalizationHandler.Localize("clname");
        _maintainClass = toggle.CreateOption("Maintain class", section2, new Vector2(0, 0), className);

        string scalingName = Main.Randomizer.LocalizationHandler.Localize("scname");
        _areaScaling = toggle.CreateOption("Area scaling", section2, new Vector2(0, -100), scalingName);

        // Section 3

        string reliqName = Main.Randomizer.LocalizationHandler.Localize("rqname");
        _reliquaries = toggle.CreateOption("Reliquaries", section3, new Vector2(0, 200), reliqName);

        string dashName = Main.Randomizer.LocalizationHandler.Localize("dsname");
        _dash = toggle.CreateOption("Dash", section3, new Vector2(0, 100), dashName);

        string wallClimbName = Main.Randomizer.LocalizationHandler.Localize("wcname");
        _wallClimb = toggle.CreateOption("Wall climb", section3, new Vector2(0, 0), wallClimbName);

        string bootsName = Main.Randomizer.LocalizationHandler.Localize("sbname");
        _boots = toggle.CreateOption("Boots", section3, new Vector2(0, -100), bootsName);

        string doubleJumpName = Main.Randomizer.LocalizationHandler.Localize("djname");
        _purifiedHand = toggle.CreateOption("Double jump", section3, new Vector2(0, -200), doubleJumpName);

        // Section 4

        string swordSkillsName = Main.Randomizer.LocalizationHandler.Localize("ssname");
        _swordSkills = toggle.CreateOption("Sword skills", section4, new Vector2(0, 200), swordSkillsName);

        string thornsName = Main.Randomizer.LocalizationHandler.Localize("thname");
        _thorns = toggle.CreateOption("Thorns", section4, new Vector2(0, 100), thornsName);

        string junkName = Main.Randomizer.LocalizationHandler.Localize("jiname");
        _junkQuests = toggle.CreateOption("Junk inconvenitent", section4, new Vector2(0, 0), junkName);

        string wheelName = Main.Randomizer.LocalizationHandler.Localize("whname");
        _wheel = toggle.CreateOption("Wheel", section4, new Vector2(0, -100), wheelName);
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
        Main.Randomizer.DebugRect = holder;

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
