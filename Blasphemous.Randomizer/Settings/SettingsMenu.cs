using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Input;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Randomizer.Settings
{
    public class SettingsMenu
    {
        private const int UNIQUE_ID_SIZE = 7;
        private const int NUMBER_OF_OPTIONS = 25;

        readonly string[] uniqueSeedIcons = new string[] // 96 diff images (5 images = 32 bits, 6 images = 39 bits, 7 images = 46 bits)
        {
            "RE01", "RE02", "RE03", "RE04", "RE05", "RE07", "RE10",
            "RB01", "RB03", "RB04", "RB05", "RB06", "RB07", "RB08", "RB09", "RB10", "RB11", "RB12", "RB13", "RB14", "RB15", "RB16", "RB21", "RB30", "RB31", "RB32", "RB33", "RB35", "RB36", "RB37", "RB101", "RB102", "RB103", "RB105", "RB106", "RB107", "RB108", "RB201", "RB202", "RB203", "RB301",
            "HE01", "HE02", "HE03", "HE04", "HE05", "HE06", "HE07", "HE10", "HE11", "HE101", "HE201",
            "PR03", "PR04", "PR05", "PR07", "PR08", "PR09", "PR10", "PR11", "PR12", "PR14", "PR15", "PR16", "PR101", "PR201", "PR202", "PR203",
            "QI01", "QI06", "QI07", "QI08", "QI10", "QI11", "QI12", "QI19", "QI20", "QI37", "QI41", "QI44", "QI57", "QI58", "QI61", "QI68", "QI69", "QI70", "QI71", "QI75", "QI78", "QI81", "QI101", "QI110", "QI202", "QI203", "QI204", "QI301",
        };

        private GameObject settingsMenu;
        private GameObject slotsMenu;
        private RectTransform debugRect;
        private Vector3 scaling;

        private Camera camera;
        private Image[] uniqueImages;
        private Text descriptionText;

        private bool menuActive;
        private bool waiting;
        private string currentSeed;
        private int generatedSeed;
        private int currentSlot;

        private SettingsElement[] buttons;

        private SettingsCyclebox LogicDifficultyLeft { get { return buttons[0] as SettingsCyclebox; } set { buttons[0] = value; } }
        private SettingsCyclebox LogicDifficultyRight { get { return buttons[1] as SettingsCyclebox; } set { buttons[1] = value; } }
        private SettingsCyclebox StartingLocationLeft { get { return buttons[2] as SettingsCyclebox; } set { buttons[2] = value; } }
        private SettingsCyclebox StartingLocationRight { get { return buttons[3] as SettingsCyclebox; } set { buttons[3] = value; } }
        private SettingsCheckbox Teleportation { get { return buttons[4] as SettingsCheckbox; } set { buttons[4] = value; } }
        private SettingsCheckbox Hints { get { return buttons[5] as SettingsCheckbox; } set { buttons[5] = value; } }
        private SettingsCheckbox Penitence { get { return buttons[6] as SettingsCheckbox; } set { buttons[6] = value; } }

        private SettingsCheckbox Reliquaries { get { return buttons[7] as SettingsCheckbox; } set { buttons[7] = value; } }
        private SettingsCheckbox Dash { get { return buttons[8] as SettingsCheckbox; } set { buttons[8] = value; } }
        private SettingsCheckbox WallClimb { get { return buttons[9] as SettingsCheckbox; } set { buttons[9] = value; } }
        private SettingsCheckbox Boots { get { return buttons[10] as SettingsCheckbox; } set { buttons[10] = value; } }
        private SettingsCheckbox PurifiedHand { get { return buttons[11] as SettingsCheckbox; } set { buttons[11] = value; } }

        private SettingsCheckbox SwordSkills { get { return buttons[12] as SettingsCheckbox; } set { buttons[12] = value; } }
        private SettingsCheckbox Thorns { get { return buttons[13] as SettingsCheckbox; } set { buttons[13] = value; } }
        private SettingsCheckbox JunkQuests { get { return buttons[14] as SettingsCheckbox; } set { buttons[14] = value; } }
        private SettingsCheckbox Wheel { get { return buttons[15] as SettingsCheckbox; } set { buttons[15] = value; } }

        private SettingsCyclebox EnemiesLeft { get { return buttons[16] as SettingsCyclebox; } set { buttons[16] = value; } }
        private SettingsCyclebox EnemiesRight { get { return buttons[17] as SettingsCyclebox; } set { buttons[17] = value; } }
        private SettingsCheckbox MaintainClass { get { return buttons[18] as SettingsCheckbox; } set { buttons[18] = value; } }
        private SettingsCheckbox AreaScaling { get { return buttons[19] as SettingsCheckbox; } set { buttons[19] = value; } }

        private SettingsCyclebox BossesLeft { get { return buttons[20] as SettingsCyclebox; } set { buttons[20] = value; } }
        private SettingsCyclebox BossesRight { get { return buttons[21] as SettingsCyclebox; } set { buttons[21] = value; } }

        private SettingsCyclebox DoorsLeft { get { return buttons[22] as SettingsCyclebox; } set { buttons[22] = value; } }
        private SettingsCyclebox DoorsRight { get { return buttons[23] as SettingsCyclebox; } set { buttons[23] = value; } }

        private SettingsTextbox SeedText { get { return buttons[24] as SettingsTextbox; } set { buttons[24] = value; } }

        public void onLoad(string scene)
        {
            // Close menu if its open
            if (settingsMenu != null && settingsMenu.activeSelf)
                settingsMenu.SetActive(false);
        }

        public void update()
        {
            if (waiting)
            {
                waiting = false;
                return;
            }
            if (settingsMenu == null || !menuActive)
                return;

            // Find what box the mouse is currently over
            SettingsElement currBox = null;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (pointInsideRect(buttons[i].transform as RectTransform, Input.mousePosition)) // Only check if the mouse has moved
                {
                    currBox = buttons[i];
                    break;
                }
            }

            // Change description text
            if (currBox != null)
            {
                descriptionText.text = currBox.Description;
            }
            else
            {
                descriptionText.text = string.Empty;
            }

            // Check if a button was clicked
            if (Input.GetMouseButtonDown(0))
            {
                SeedText.Selected = currBox == SeedText;
                if (SeedText.Selected || currentSeed != string.Empty)
                    SeedText.TextContent = currentSeed;
                else
                    SeedText.TextContent = generatedSeed.ToString();

                if (currBox != null)
                {
                    currBox.onClick();
                    UpdateUniqueId();
                }
            }

            // Debug testing ui positions
            if (debugRect != null)
            {
                Vector2 movement = new Vector2();
                if (Input.GetKeyDown(KeyCode.LeftArrow)) movement.x -= 1;
                if (Input.GetKeyDown(KeyCode.RightArrow)) movement.x += 1;
                if (Input.GetKeyDown(KeyCode.DownArrow)) movement.y -= 1;
                if (Input.GetKeyDown(KeyCode.UpArrow)) movement.y += 1;
                
                if (movement != Vector2.zero)
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                        movement *= 5;
                    debugRect.anchoredPosition += movement;
                    Main.Randomizer.LogWarning("Moving rect to " + debugRect.anchoredPosition);
                }
            }

            // Keyboard input
            if (SeedText.Selected)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) processKeyInput(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) processKeyInput(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) processKeyInput(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) processKeyInput(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) processKeyInput(5);
                else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) processKeyInput(6);
                else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) processKeyInput(7);
                else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) processKeyInput(8);
                else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) processKeyInput(9);
                else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) processKeyInput(0);
                else if (Input.GetKeyDown(KeyCode.Backspace)) processKeyInput(-1);
            }

            // Game input
            if (Main.Randomizer.InputHandler.GetButtonDown(ButtonCode.UISubmit))
                beginGame();
            else if (Main.Randomizer.InputHandler.GetButtonDown(ButtonCode.UICancel))
                closeMenu();
        }

        private void processKeyInput(int num)
        {
            // Pressed backspace - delete last character
            if (num < 0)
            {
                if (currentSeed.Length > 0)
                    currentSeed = currentSeed.Substring(0, currentSeed.Length - 1);
            }
            // Pressed number 1-10 - add new character
            else if (int.TryParse(currentSeed + num.ToString(), out int newSeed) && newSeed <= Randomizer.MAX_SEED && (currentSeed.Length > 0 || num != 0))
            {
                currentSeed += num.ToString();
            }

            // Update text
            SeedText.TextContent = currentSeed;
            Main.Randomizer.playSoundEffect(2);

            UpdateUniqueId();
        }

        public void setConfigSettings(Config config)
        {
            if (settingsMenu == null)
                return;

            // Load config into buttons
            LogicDifficultyLeft.CurrentOption = config.LogicDifficulty;
            LogicDifficultyRight.CurrentOption = config.LogicDifficulty;
            StartingLocationLeft.CurrentOption = config.StartingLocation;
            StartingLocationRight.CurrentOption = config.StartingLocation;
            Teleportation.Selected = config.UnlockTeleportation;
            Hints.Selected = config.AllowHints;
            Penitence.Selected = config.AllowPenitence;

            Reliquaries.Selected = config.ShuffleReliquaries;
            Dash.Selected = config.ShuffleDash;
            WallClimb.Selected = config.ShuffleWallClimb;
            Boots.Selected = config.ShuffleBootsOfPleading;
            PurifiedHand.Selected = config.ShufflePurifiedHand;

            SwordSkills.Selected = config.ShuffleSwordSkills;
            Thorns.Selected = config.ShuffleThorns;
            JunkQuests.Selected = config.JunkLongQuests;
            Wheel.Selected = config.StartWithWheel;

            MaintainClass.Selected = config.MaintainClass;
            AreaScaling.Selected = config.AreaScaling;
            EnemiesLeft.CurrentOption = config.EnemyShuffleType;
            EnemiesRight.CurrentOption = config.EnemyShuffleType;

            BossesLeft.CurrentOption = config.BossShuffleType;
            BossesRight.CurrentOption = config.BossShuffleType;

            DoorsLeft.CurrentOption = config.DoorShuffleType;
            DoorsRight.CurrentOption = config.DoorShuffleType;

            // Load config into seed
            currentSeed = config.CustomSeed > 0 ? config.CustomSeed.ToString() : string.Empty;
            SeedText.TextContent = currentSeed != string.Empty ? currentSeed : generatedSeed.ToString();
            descriptionText.text = string.Empty;

            UpdateUniqueId();
        }

        public Config getConfigSettings()
        {
            Config config = new Config();
            if (settingsMenu == null)
                return config;

            // Load config from buttons
            config.LogicDifficulty = LogicDifficultyLeft.CurrentOption;
            config.StartingLocation = StartingLocationLeft.CurrentOption;
            config.UnlockTeleportation = Teleportation.Selected;
            config.AllowHints = Hints.Selected;
            config.AllowPenitence = Penitence.Selected;

            config.ShuffleReliquaries = Reliquaries.Selected;
            config.ShuffleDash = Dash.Selected;
            config.ShuffleWallClimb = WallClimb.Selected;
            config.ShuffleBootsOfPleading = Boots.Selected;
            config.ShufflePurifiedHand = PurifiedHand.Selected;

            config.ShuffleSwordSkills = SwordSkills.Selected;
            config.ShuffleThorns = Thorns.Selected;
            config.JunkLongQuests = JunkQuests.Selected;
            config.StartWithWheel = Wheel.Selected;

            config.EnemyShuffleType = EnemiesLeft.CurrentOption;
            config.MaintainClass = MaintainClass.Selected;
            config.AreaScaling = AreaScaling.Selected;

            config.BossShuffleType = BossesLeft.CurrentOption;
            config.DoorShuffleType = DoorsLeft.CurrentOption;

            // Load config from seed
            config.CustomSeed = currentSeed != string.Empty ? int.Parse(currentSeed) : generatedSeed;
            return config;
        }

        private void showSettingsMenu(bool value, bool changeMenuVisibility)
        {
            if (settingsMenu == null)
                createSettingsMenu();
            
            Main.Randomizer.Log("Showing settings menu: " + value);
            if (changeMenuVisibility) 
                settingsMenu.SetActive(value);
            slotsMenu.SetActive(!value);
            Cursor.visible = value;
            menuActive = value;
        }

        private void UpdateUniqueId()
        {
            // Set enabled status of various checkboxes whenever ui is updated
            Boots.Enabled = Main.Randomizer.InstalledBootsMod;
            PurifiedHand.Enabled = Main.Randomizer.InstalledDoubleJumpMod;
            MaintainClass.Enabled = EnemiesLeft.CurrentOption > 0;
            AreaScaling.Enabled = EnemiesLeft.CurrentOption > 0;
            Dash.Enabled = DoorsLeft.CurrentOption > 1 || StartingLocationLeft.CurrentOption != Randomizer.BROTHERHOOD_LOCATION && StartingLocationLeft.CurrentOption != Randomizer.SHIPYARD_LOCATION;
            WallClimb.Enabled = DoorsLeft.CurrentOption > 1 || StartingLocationLeft.CurrentOption != Randomizer.DEPTHS_LOCATION;

            // Get final seed based on seed & options
            long finalSeed = Main.Randomizer.ComputeFinalSeed(currentSeed != string.Empty ? int.Parse(currentSeed) : generatedSeed, getConfigSettings());

            // Fill images based on unique seed
            try
            {
                FillImages(finalSeed);
            }
            catch (System.Exception)
            {
                Main.Randomizer.LogError("Failed to generate image layout for unique seed " + finalSeed);
                for (int i = 0; i < uniqueImages.Length; i++)
                    uniqueImages[i].sprite = GetIcon(0);
            }

            void FillImages(long seed)
            {
                int numDigits = uniqueSeedIcons.Length, currDigit = 0;
                do
                {
                    int imgIdx = (int)(seed % numDigits);
                    seed /= numDigits;

                    SetDigitImage(currDigit, GetIcon(imgIdx));
                    currDigit++;
                }
                while (seed > 0);
                for ( ; currDigit < uniqueImages.Length; currDigit++)
                    SetDigitImage(currDigit, GetIcon(0));
            }

            Sprite GetIcon(int index)
            {
                string itemId = uniqueSeedIcons[index];
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
                uniqueImages[realIdx].sprite = image;
            }
        }

        public void beginGame()
        {
            if (!menuActive || waiting) return;

            Main.Randomizer.playSoundEffect(0);
            showSettingsMenu(false, false);
            waiting = true;
            Object.FindObjectOfType<SelectSaveSlots>().OnAcceptSlots(999 + currentSlot);
        }

        public void openMenu(int slot)
        {
            if (menuActive || waiting) return;

            currentSlot = slot;
            waiting = true;
            generatedSeed = new System.Random().Next(1, Randomizer.MAX_SEED);
            Main.Randomizer.Log("Generating default seed: " + generatedSeed);
            showSettingsMenu(true, true);
            setConfigSettings(new Config());
        }

        public void closeMenu()
        {
            if (!menuActive || waiting) return;

            Main.Randomizer.playSoundEffect(1);
            showSettingsMenu(false, true);
        }

        private bool pointInsideRect(RectTransform rect, Vector2 point)
        {
            Vector2 position = camera.WorldToScreenPoint(rect.position);
            position = new Vector2(position.x * scaling.x, position.y * scaling.y + scaling.z);
            Vector2 size = new Vector2(rect.rect.width * scaling.x / 2, rect.rect.height * scaling.y / 2);

            return point.x >= position.x - size.x && point.x <= position.x + size.x && point.y >= position.y - size.y && point.y <= position.y + size.y;
        }
    }
}
