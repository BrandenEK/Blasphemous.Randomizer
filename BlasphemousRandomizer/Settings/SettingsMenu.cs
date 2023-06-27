using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others;
using ModdingAPI;
using UnityEngine;
using UnityEngine.UI;

namespace BlasphemousRandomizer.Settings
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
            if (Main.Randomizer.Input.GetButtonDown(InputHandler.ButtonCode.UISubmit)) beginGame();
            else if (Main.Randomizer.Input.GetButtonDown(InputHandler.ButtonCode.UICancel)) closeMenu();
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
            config.VersionCreated = Main.Randomizer.ModVersion;
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

        private void createSettingsMenu()
        {
            // Find objects
            float xScale = (float)Screen.width / 640;
            scaling = new Vector3(xScale, xScale, (Screen.height - 360 * xScale) * 0.5f);
            foreach (Camera cam in Object.FindObjectsOfType<Camera>())
                if (cam.name == "UICamera")
                    camera = cam;
            
            // Get menus
            Transform menu = Object.FindObjectOfType<NewMainMenu>().transform;
            slotsMenu = menu.GetChild(2).gameObject;

            // Get input buttons
            RectTransform begin = Object.Instantiate(slotsMenu.transform.GetChild(3).GetChild(0).GetChild(0).gameObject).GetComponent<RectTransform>();
            Object.Destroy(begin.GetComponent<HorizontalLayoutGroup>());
            Object.Destroy(begin.GetChild(1).GetComponent<I2.Loc.Localize>());
            RectTransform cancel = Object.Instantiate(slotsMenu.transform.GetChild(3).GetChild(1).gameObject).GetComponent<RectTransform>();
            Object.Destroy(cancel.GetComponent<HorizontalLayoutGroup>());
            Object.Destroy(cancel.GetChild(1).GetComponent<I2.Loc.Localize>());

            // Duplicate slot menu
            settingsMenu = Object.Instantiate(slotsMenu, menu);
            settingsMenu.name = "Settings Menu";
            Object.Destroy(settingsMenu.GetComponent<SelectSaveSlots>());
            Object.Destroy(settingsMenu.GetComponent<KeepFocus>());
            Object.Destroy(settingsMenu.GetComponent<CanvasGroup>());
            int childrenCount = settingsMenu.transform.childCount;
            for (int i = 2; i < childrenCount; i++)
                Object.Destroy(settingsMenu.transform.GetChild(i).gameObject);
            
            // Set rect of settings menu
            RectTransform rect = settingsMenu.GetComponent<RectTransform>(); // Is this necessary ??
            rect.SetParent(menu, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            // Create unique seed images
            RectTransform uniqueHolder = getNewRect("Img Holder", rect);
            uniqueHolder.sizeDelta = new Vector2(100, 32);
            uniqueHolder.pivot = Vector2.one;
            uniqueHolder.anchoredPosition = new Vector2(320, 180);

            uniqueImages = new Image[UNIQUE_ID_SIZE];
            for (int i = 0; i < uniqueImages.Length; i++)
            {
                RectTransform image = getNewImage("Img" + i, uniqueHolder, 32);
                image.pivot = new Vector2(1, 0.5f);
                image.anchorMin = new Vector2(1, 0.5f);
                image.anchorMax = new Vector2(1, 0.5f);
                image.anchoredPosition = new Vector2(-5 + i * -30, 0);
                uniqueImages[i] = image.GetComponent<Image>();
            }

            // Set header text
            Text headerText = settingsMenu.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            headerText.text = Main.Randomizer.Localize("chset");
            Font font = headerText.font;

            // Create main section
            int width = 630, height = 260;
            RectTransform mainSection = getNewRect("Main Section", settingsMenu.transform);
            mainSection.pivot = new Vector2(0.5f, 0);
            mainSection.anchorMin = new Vector2(0.5f, 0);
            mainSection.anchorMax = new Vector2(0.5f, 0);
            mainSection.sizeDelta = new Vector2(width, height);
            mainSection.anchoredPosition = new Vector2(0, 185);

            // Set up section buttons
            buttons = new SettingsElement[NUMBER_OF_OPTIONS];
            int top = height / 2 - 5;
            int left = -60;

            // Create seed textbox
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            Sprite lineSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);

            RectTransform seedTitle = getNewText("SeedTitle", rect, Main.Randomizer.Localize("menusd") + ": ", font, 16, Color.white, TextAnchor.MiddleLeft);
            seedTitle.pivot = new Vector2(0.5f, 0.5f);
            seedTitle.anchoredPosition = new Vector2(-250, 165);

            RectTransform seedBox = getNewTextbox("SeedBox", rect, font, lineSprite, 53);
            seedBox.pivot = new Vector2(0.5f, 0.5f);
            seedBox.anchoredPosition = new Vector2(-225, 165);
            SeedText = seedBox.GetComponent<SettingsTextbox>();

            // General section

            RectTransform generalSection = getNewRect("General Section", mainSection);
            generalSection.sizeDelta = new Vector2(width / 4, height);
            generalSection.anchoredPosition = new Vector2(-0.375f * width, 0);

            RectTransform logicText = getNewText("Logic Text", generalSection, Main.Randomizer.Localize("lgname") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            logicText.anchoredPosition = new Vector2(0, top);

            RectTransform logicOption = getNewCyclebox("Logic Option", generalSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("lgtyp1"), Main.Randomizer.Localize("lgtyp2"), Main.Randomizer.Localize("lgtyp3") },
                new string[] { Main.Randomizer.Localize("lgtyp1") + " - " + Main.Randomizer.Localize("lgdes1"),
                               Main.Randomizer.Localize("lgtyp2") + " - " + Main.Randomizer.Localize("lgdes2"),
                               Main.Randomizer.Localize("lgtyp3") + " - " + Main.Randomizer.Localize("lgdes3") }, 30);
            logicOption.anchoredPosition = new Vector2(0, top - 20);
            LogicDifficultyLeft = logicOption.GetChild(0).GetComponent<SettingsCyclebox>();
            LogicDifficultyRight = logicOption.GetChild(1).GetComponent<SettingsCyclebox>();

            RectTransform startLocationText = getNewText("Start Loc Text", generalSection, Main.Randomizer.Localize("loname") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            startLocationText.anchoredPosition = new Vector2(0, top - 50);

            string locationDesc = Main.Randomizer.Localize("lodesc");
            string[] locationNames = new string[8];
            string[] locationDescriptions = new string[8];
            for (int i = 0; i < 8; i++)
            {
                locationNames[i] = Main.Randomizer.Localize("lotyp" + (i + 1));
                locationDescriptions[i] = locationDesc;
            }

            RectTransform startLocationOption = getNewCyclebox("Start Loc Option", generalSection, font, 15, 16, locationNames, locationDescriptions, 50);
            startLocationOption.anchoredPosition = new Vector2(0, top - 70);
            StartingLocationLeft = startLocationOption.GetChild(0).GetComponent<SettingsCyclebox>();
            StartingLocationRight = startLocationOption.GetChild(1).GetComponent<SettingsCyclebox>();

            RectTransform teleportOption = getNewCheckbox("Teleport", generalSection, Main.Randomizer.Localize("tpname"), Main.Randomizer.Localize("tpdesc"), font, 15, 16);
            teleportOption.anchoredPosition = new Vector2(left + 10, top - 100);
            Teleportation = teleportOption.GetComponent<SettingsCheckbox>();

            RectTransform hintsOption = getNewCheckbox("Hints", generalSection, Main.Randomizer.Localize("htname"), Main.Randomizer.Localize("htdesc"), font, 15, 16);
            hintsOption.anchoredPosition = new Vector2(left + 10, top - 135);
            Hints = hintsOption.GetComponent<SettingsCheckbox>();

            RectTransform penitenceOption = getNewCheckbox("Penitence", generalSection, Main.Randomizer.Localize("pename"), Main.Randomizer.Localize("pedesc"), font, 15, 16);
            penitenceOption.anchoredPosition = new Vector2(left + 10, top - 170);
            Penitence = penitenceOption.GetComponent<SettingsCheckbox>();

            // Enemy/Boss/Door section

            RectTransform enemiesSection = getNewRect("Enemies Section", mainSection);
            enemiesSection.sizeDelta = new Vector2(width / 4, height);
            enemiesSection.anchoredPosition = new Vector2(-0.125f * width, 0);

            RectTransform doorsTitle = getNewText("Doors Text", enemiesSection, Main.Randomizer.Localize("drname") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            doorsTitle.anchoredPosition = new Vector2(0, top);

            RectTransform doorsOption = getNewCyclebox("Doors Option", enemiesSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("distyp"), Main.Randomizer.Localize("simtyp"), Main.Randomizer.Localize("fultyp") },
                new string[] { Main.Randomizer.Localize("distyp") + " - " + Main.Randomizer.Localize("drdes1"),
                               Main.Randomizer.Localize("simtyp") + " - " + Main.Randomizer.Localize("drdes2"),
                               Main.Randomizer.Localize("fultyp") + " - " + Main.Randomizer.Localize("drdes3"), }, 36);
            doorsOption.anchoredPosition = new Vector2(0, top - 20);
            DoorsLeft = doorsOption.GetChild(0).GetComponent<SettingsCyclebox>();
            DoorsRight = doorsOption.GetChild(1).GetComponent<SettingsCyclebox>();

            RectTransform bossTitle = getNewText("Boss Text", enemiesSection, Main.Randomizer.Localize("bsname") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            bossTitle.anchoredPosition = new Vector2(0, top - 50);

            RectTransform bossOption = getNewCyclebox("Boss Option", enemiesSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("distyp"), Main.Randomizer.Localize("simtyp"), Main.Randomizer.Localize("fultyp") },
                new string[] { Main.Randomizer.Localize("distyp") + " - " + Main.Randomizer.Localize("bsdes1"),
                               Main.Randomizer.Localize("simtyp") + " - " + Main.Randomizer.Localize("bsdes2"),
                               Main.Randomizer.Localize("fultyp") + " - " + Main.Randomizer.Localize("bsdes3"), }, 36);
            bossOption.anchoredPosition = new Vector2(0, top - 70);
            BossesLeft = bossOption.GetChild(0).GetComponent<SettingsCyclebox>();
            BossesRight = bossOption.GetChild(1).GetComponent<SettingsCyclebox>();

            RectTransform classOption = getNewCheckbox("Class", enemiesSection, Main.Randomizer.Localize("clname"), Main.Randomizer.Localize("cldesc"), font, 15, 16);
            classOption.anchoredPosition = new Vector2(left + 10, top - 150);
            MaintainClass = classOption.GetComponent<SettingsCheckbox>();

            RectTransform scalingOption = getNewCheckbox("Scaling", enemiesSection, Main.Randomizer.Localize("scname"), Main.Randomizer.Localize("scdesc"), font, 15, 16);
            scalingOption.anchoredPosition = new Vector2(left + 10, top - 185);
            AreaScaling = scalingOption.GetComponent<SettingsCheckbox>();

            RectTransform enemyTitle = getNewText("Enemy Text", enemiesSection, Main.Randomizer.Localize("enname") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            enemyTitle.anchoredPosition = new Vector2(0, top - 100);

            RectTransform enemiesType = getNewCyclebox("Type", enemiesSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("distyp"), Main.Randomizer.Localize("simtyp"), Main.Randomizer.Localize("fultyp") },
                new string[] { Main.Randomizer.Localize("distyp") + " - " + Main.Randomizer.Localize("endes1"),
                               Main.Randomizer.Localize("simtyp") + " - " + Main.Randomizer.Localize("endes2"),
                               Main.Randomizer.Localize("fultyp") + " - " + Main.Randomizer.Localize("endes3") }, 36);
            enemiesType.anchoredPosition = new Vector2(0, top - 120);
            EnemiesLeft = enemiesType.GetChild(0).GetComponent<SettingsCyclebox>();
            EnemiesRight = enemiesType.GetChild(1).GetComponent<SettingsCyclebox>();

            // Items section

            RectTransform itemsSection = getNewRect("Items Section", mainSection);
            itemsSection.sizeDelta = new Vector2(width / 4, height);
            itemsSection.anchoredPosition = new Vector2(0.125f * width, 0);

            RectTransform reliqOption = getNewCheckbox("Reliquaries", itemsSection, Main.Randomizer.Localize("rqname"), Main.Randomizer.Localize("rqdesc"), font, 15, 16);
            reliqOption.anchoredPosition = new Vector2(left, top);
            Reliquaries = reliqOption.GetComponent<SettingsCheckbox>();

            RectTransform dashOption = getNewCheckbox("Dash", itemsSection, Main.Randomizer.Localize("dsname"), Main.Randomizer.Localize("dsdesc"), font, 15, 16);
            dashOption.anchoredPosition = new Vector2(left, top - 35);
            Dash = dashOption.GetComponent<SettingsCheckbox>();

            RectTransform wallclimbOption = getNewCheckbox("Wall Climb", itemsSection, Main.Randomizer.Localize("wcname"), Main.Randomizer.Localize("wcdesc"), font, 15, 16);
            wallclimbOption.anchoredPosition = new Vector2(left, top - 70);
            WallClimb = wallclimbOption.GetComponent<SettingsCheckbox>();

            RectTransform bootsOption = getNewCheckbox("Boots", itemsSection, Main.Randomizer.Localize("sbname"), Main.Randomizer.Localize("sbdesc"), font, 15, 16);
            bootsOption.anchoredPosition = new Vector2(left, top - 105);
            Boots = bootsOption.GetComponent<SettingsCheckbox>();

            RectTransform purifedHandOption = getNewCheckbox("PurifiedHand", itemsSection, Main.Randomizer.Localize("djname"), Main.Randomizer.Localize("djdesc"), font, 15, 16);
            purifedHandOption.anchoredPosition = new Vector2(left, top - 140);
            PurifiedHand = purifedHandOption.GetComponent<SettingsCheckbox>();

            // Last section

            RectTransform lastSection = getNewRect("Last Section", mainSection);
            lastSection.sizeDelta = new Vector2(width / 4, height);
            lastSection.anchoredPosition = new Vector2(0.375f * width, 0);

            RectTransform swordskillsOption = getNewCheckbox("Sword Skills", lastSection, Main.Randomizer.Localize("ssname"), Main.Randomizer.Localize("ssdesc"), font, 15, 16);
            swordskillsOption.anchoredPosition = new Vector2(left, top);
            SwordSkills = swordskillsOption.GetComponent<SettingsCheckbox>();

            RectTransform thornsOption = getNewCheckbox("Thorns", lastSection, Main.Randomizer.Localize("thname"), Main.Randomizer.Localize("thdesc"), font, 15, 16);
            thornsOption.anchoredPosition = new Vector2(left, top - 35);
            Thorns = thornsOption.GetComponent<SettingsCheckbox>();

            RectTransform junkOptions = getNewCheckbox("Junk Quests", lastSection, Main.Randomizer.Localize("jiname"), Main.Randomizer.Localize("jidesc"), font, 15, 16);
            junkOptions.anchoredPosition = new Vector2(left, top - 70);
            JunkQuests = junkOptions.GetComponent<SettingsCheckbox>();

            RectTransform wheelOption = getNewCheckbox("Wheel", lastSection, Main.Randomizer.Localize("whname"), Main.Randomizer.Localize("whdesc"), font, 15, 16);
            wheelOption.anchoredPosition = new Vector2(left, top - 105);
            Wheel = wheelOption.GetComponent<SettingsCheckbox>();

            // Set begin/cancel buttons
            begin.SetParent(lastSection, false);
            begin.anchorMin = new Vector2(0.5f, 0.5f);
            begin.anchorMax = new Vector2(0.5f, 0.5f);
            begin.anchoredPosition = new Vector2(10, -50);
            begin.GetChild(1).GetComponent<Text>().text = " " + Main.Randomizer.Localize("begin");
            cancel.SetParent(lastSection, false);
            cancel.anchorMin = new Vector2(0.5f, 0.5f);
            cancel.anchorMax = new Vector2(0.5f, 0.5f);
            cancel.anchoredPosition = new Vector2(10, -80);
            cancel.GetChild(1).GetComponent<Text>().text = " " + Main.Randomizer.Localize("cancel");

            // Create description text
            RectTransform desc = getNewText("Description", mainSection, "", font, 16, Color.white, TextAnchor.MiddleLeft);
            desc.anchoredPosition = new Vector2(-170, top - 225);
            descriptionText = desc.GetComponent<Text>();

            // Hide menu
            Main.Randomizer.Log("Settings menu has been created");
            settingsMenu.SetActive(false);

            RectTransform getNewRect(string name, Transform parent)
            {
                GameObject obj = new GameObject(name, typeof(RectTransform));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.gameObject.layer = LayerMask.NameToLayer("UI");
                rect.SetParent(parent, false);
                return rect;
            }

            RectTransform getNewImage(string name, Transform parent, int size)
            {
                RectTransform rect = getNewRect(name, parent);
                rect.sizeDelta = new Vector2(size, size);
                rect.gameObject.AddComponent<Image>();
                return rect;
            }

            RectTransform getNewText(string name, Transform parent, string text, Font font, int size, Color color, TextAnchor alignment)
            {
                RectTransform rect = getNewRect(name, parent);
                Text display = rect.gameObject.AddComponent<Text>();
                display.text = text;
                display.font = font;
                display.color = color;
                display.fontSize = size;
                display.alignment = alignment;
                display.horizontalOverflow = HorizontalWrapMode.Overflow;
                return rect;
            }

            RectTransform getNewCheckbox(string name, Transform parent, string label, string desc, Font font, int boxSize, int fontSize)
            {
                RectTransform rect = getNewImage(name, parent, boxSize);

                RectTransform text = getNewText("Label", rect, label, font, fontSize, Color.white, TextAnchor.MiddleLeft);
                text.anchoredPosition = new Vector2(boxSize + 50, 0);

                SettingsCheckbox check = rect.gameObject.AddComponent<SettingsCheckbox>();
                check.onStart(desc);
                return rect;
            }

            RectTransform getNewCyclebox(string name, Transform parent, Font font, int boxSize, int fontSize, string[] options, string[] descs, int arrowDistance)
            {
                RectTransform rect = getNewText(name, parent, "", font, fontSize, Color.yellow, TextAnchor.MiddleCenter);

                RectTransform left = getNewImage("Left", rect, boxSize);
                left.anchoredPosition = new Vector2(-arrowDistance, 0);
                left.gameObject.AddComponent<SettingsCyclebox>().onStart(options, descs, false);

                RectTransform right = getNewImage("Right", rect, boxSize);
                right.anchoredPosition = new Vector2(arrowDistance, 0);
                right.gameObject.AddComponent<SettingsCyclebox>().onStart(options, descs, true);

                return rect;
            }

            RectTransform getNewTextbox(string name, Transform parent, Font font, Sprite lineSprite, int lineSize)
            {
                RectTransform text = getNewText(name, parent, string.Empty, font, 16, Color.white, TextAnchor.MiddleCenter);
                text.sizeDelta = new Vector2(lineSize, 15);
                RectTransform line = getNewImage("Line", text, 1);
                line.GetComponent<Image>().sprite = lineSprite;
                line.sizeDelta = new Vector2(lineSize, 1);
                line.pivot = new Vector2(0.5f, 0);
                line.anchoredPosition = new Vector2(0, -8);

                text.gameObject.AddComponent<SettingsTextbox>().OnStart();
                return text;
            }
        }
    }
}
