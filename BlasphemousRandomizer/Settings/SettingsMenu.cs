using UnityEngine;
using UnityEngine.UI;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others;
using Framework.Managers;

namespace BlasphemousRandomizer.Settings
{
    public class SettingsMenu
    {
        private const int UNIQUE_ID_SIZE = 5;
        private const int NUMBER_OF_OPTIONS = 13;

        readonly string[] uniqueSeedIcons = new string[] // 42 diff images (Will have to be updated when adding new options)
        {
            "RB01", "RB03", "RB07", "RB08", "RB09", "RB10", "RB11", "RB12", "RB13", "RB21", "RB33", "RB35", "RB36", "RB101", "RB102", "RB103", "RB105", "RB107", "RB108", "RB201", "RB301",
            "RE01", "RE02", "RE03", "RE04", "RE07", "RE10",
            "HE101", "HE201",
            "QI01", "QI41", "QI44", "QI68", "QI69", "QI70", "QI71", "QI78", "QI81", "QI101", "QI110", "QI203", "QI301"
        };

        private GameObject settingsMenu;
        private GameObject slotsMenu;
        private Vector3 scaling;

        private Camera camera;
        private Image[] uniqueImages;
        private Text seedText;
        private Text descriptionText;

        private bool menuActive;
        private bool waiting;
        private string currentSeed;
        private int generatedSeed;
        private int currentSlot;

        private SettingsElement[] buttons;
        private SettingsCheckbox Teleportation { get { return buttons[0] as SettingsCheckbox; } set { buttons[0] = value; } }
        //private SettingsCheckbox Cutscenes { get { return buttons[1] as SettingsCheckbox; } set { buttons[1] = value; } }
        private SettingsCheckbox Hints { get { return buttons[1] as SettingsCheckbox; } set { buttons[1] = value; } }
        private SettingsCheckbox Penitence { get { return buttons[2] as SettingsCheckbox; } set { buttons[2] = value; } }

        private SettingsCyclebox ItemsLeft { get { return buttons[9] as SettingsCyclebox; } set { buttons[9] = value; } }
        private SettingsCyclebox ItemsRight { get { return buttons[10] as SettingsCyclebox; } set { buttons[10] = value; } }

        private SettingsCyclebox StartingLocationLeft { get { return buttons[3] as SettingsCyclebox; } set { buttons[3] = value; } }
        private SettingsCyclebox StartingLocationRight { get { return buttons[4] as SettingsCyclebox; } set { buttons[4] = value; } }
        //private SettingsCheckbox MistDamage { get { return buttons[3] as SettingsCheckbox; } set { buttons[3] = value; } }
        //private SettingsCheckbox NpcDeath { get { return buttons[4] as SettingsCheckbox; } set { buttons[4] = value; } }
        private SettingsCheckbox Wheel { get { return buttons[5] as SettingsCheckbox; } set { buttons[5] = value; } }
        private SettingsCheckbox Reliquaries { get { return buttons[6] as SettingsCheckbox; } set { buttons[6] = value; } }

        private SettingsCyclebox EnemiesLeft { get { return buttons[11] as SettingsCyclebox; } set { buttons[11] = value; } }
        private SettingsCyclebox EnemiesRight { get { return buttons[12] as SettingsCyclebox; } set { buttons[12] = value; } }
        private SettingsCheckbox MaintainClass { get { return buttons[7] as SettingsCheckbox; } set { buttons[7] = value; } }
        private SettingsCheckbox AreaScaling { get { return buttons[8] as SettingsCheckbox; } set { buttons[8] = value; } }

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
                descriptionText.text = currBox.getDescription();
            }
            else if (currentSeed == "")
            {
                descriptionText.text = Main.Randomizer.Localize("typesd");
            }
            else
            {
                descriptionText.text = "";
            }

            // Check if a button was clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (currBox != null)
                {
                    currBox.onClick();
                    UpdateUniqueId();
                }
            }

            // Keyboard input
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

            // Game input
            if (Main.Randomizer.Input.GetButtonDown(ModdingAPI.InputHandler.ButtonCode.UISubmit)) beginGame();
            else if (Main.Randomizer.Input.GetButtonDown(ModdingAPI.InputHandler.ButtonCode.UICancel)) closeMenu();
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
            seedText.text = Main.Randomizer.Localize("menusd") + ": " + (currentSeed != "" ? currentSeed : Main.Randomizer.Localize("menurd"));
            Main.Randomizer.playSoundEffect(2);

            UpdateUniqueId();
        }

        public void setConfigSettings(Config config)
        {
            if (settingsMenu == null)
                return;

            // Load config into buttons
            Teleportation.setSelected(config.UnlockTeleportation);
            //Cutscenes.setSelected(true);
            Hints.setSelected(config.AllowHints);
            Penitence.setSelected(config.AllowPenitence);
            //MistDamage.setSelected(true);
            //NpcDeath.setSelected(true);
            Wheel.setSelected(config.StartWithWheel);
            Reliquaries.setSelected(config.ShuffleReliquaries);
            MaintainClass.setSelected(config.MaintainClass);
            AreaScaling.setSelected(config.AreaScaling);
            ItemsLeft.setOption(1);
            ItemsRight.setOption(1);
            StartingLocationLeft.setOption(config.StartingLocation);
            StartingLocationRight.setOption(config.StartingLocation);
            EnemiesLeft.setOption(config.EnemyShuffleType);
            EnemiesRight.setOption(config.EnemyShuffleType);

            // Load config into seed
            currentSeed = config.CustomSeed > 0 ? config.CustomSeed.ToString() : "";
            seedText.text = Main.Randomizer.Localize("menusd") + ": " + (currentSeed != "" ? currentSeed : Main.Randomizer.Localize("menurd"));
            descriptionText.text = "";

            UpdateUniqueId();
        }

        public Config getConfigSettings()
        {
            Config config = new Config();
            if (settingsMenu == null)
                return config;

            // Load config from buttons
            config.UnlockTeleportation = Teleportation.getSelected();
            config.AllowHints = Hints.getSelected();
            config.AllowPenitence = Penitence.getSelected();
            config.StartWithWheel = Wheel.getSelected();
            config.ShuffleReliquaries = Reliquaries.getSelected();
            config.MaintainClass = MaintainClass.getSelected();
            config.AreaScaling = AreaScaling.getSelected();
            config.EnemyShuffleType = EnemiesLeft.getOption();
            config.StartingLocation = StartingLocationLeft.getOption();

            // Load config from seed
            config.CustomSeed = currentSeed != "" ? int.Parse(currentSeed) : generatedSeed;
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
            // Get final seed based on seed & options
            int finalSeed = Main.Randomizer.ComputeFinalSeed(currentSeed != "" ? int.Parse(currentSeed) : generatedSeed, ItemsRight.getOption(), true, Wheel.getSelected(), Reliquaries.getSelected());

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

            void FillImages(int seed)
            {
                int numDigits = uniqueSeedIcons.Length, currDigit = 0;
                do
                {
                    int imgIdx = seed % numDigits;
                    seed /= numDigits;

                    uniqueImages[currDigit].sprite = GetIcon(imgIdx);
                    currDigit++;
                }
                while (seed > 0);
                for ( ; currDigit < uniqueImages.Length; currDigit++)
                    uniqueImages[currDigit].sprite = GetIcon(0);
            }

            Sprite GetIcon(int index)
            {
                string id = uniqueSeedIcons[index];
                if (id.StartsWith("RB"))
                    return Core.InventoryManager.GetRosaryBead(id).picture;
                else if (id.StartsWith("RE"))
                    return Core.InventoryManager.GetRelic(id).picture;
                else if (id.StartsWith("HE"))
                    return Core.InventoryManager.GetSword(id).picture;
                else if (id.StartsWith("QI"))
                    return Core.InventoryManager.GetQuestItem(id).picture;
                return null;
            }
        }

        public void beginGame()
        {
            if (!menuActive || waiting) return;

            Main.Randomizer.playSoundEffect(0);
            Main.Randomizer.gameConfig = getConfigSettings();
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

            // General section

            RectTransform generalSection = getNewRect("General Section", mainSection);
            generalSection.sizeDelta = new Vector2(width / 4, height);
            generalSection.anchoredPosition = new Vector2(-0.375f * width, 0);

            RectTransform generalTitle = getNewText("General Title", generalSection, Main.Randomizer.Localize("genset") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            generalTitle.anchoredPosition = new Vector2(0, top - 5);

            RectTransform teleportOption = getNewCheckbox("Teleport", generalSection, Main.Randomizer.Localize("tpname"), Main.Randomizer.Localize("tpdesc"), font, 15, 16);
            teleportOption.anchoredPosition = new Vector2(left, top - 70);
            Teleportation = teleportOption.GetComponent<SettingsCheckbox>();

            //RectTransform cutscenesOption = getNewCheckbox("Cutscenes", generalSection, Main.Randomizer.Localize("ctname"), Main.Randomizer.Localize("ctdesc"), font, 15, 16);
            //cutscenesOption.anchoredPosition = new Vector2(left, top - 100);
            //Cutscenes = cutscenesOption.GetComponent<SettingsCheckbox>();

            RectTransform hintsOption = getNewCheckbox("Hints", generalSection, Main.Randomizer.Localize("htname"), Main.Randomizer.Localize("htdesc"), font, 15, 16);
            hintsOption.anchoredPosition = new Vector2(left, top - 130);
            Hints = hintsOption.GetComponent<SettingsCheckbox>();

            RectTransform penitenceOption = getNewCheckbox("Penitence", generalSection, Main.Randomizer.Localize("pename"), Main.Randomizer.Localize("pedesc"), font, 15, 16);
            penitenceOption.anchoredPosition = new Vector2(left, top - 160);
            Penitence = penitenceOption.GetComponent<SettingsCheckbox>();

            RectTransform seed = getNewText("Seed", generalSection, Main.Randomizer.Localize("menusd") + ": ", font, 16, Color.yellow, TextAnchor.MiddleCenter);
            seed.anchoredPosition = new Vector2(0, top - 25);
            seedText = seed.GetComponent<Text>();

            // Items section

            RectTransform itemsSection = getNewRect("Items Section", mainSection);
            itemsSection.sizeDelta = new Vector2(width / 4, height);
            itemsSection.anchoredPosition = new Vector2(-0.125f * width, 0);

            RectTransform itemsTitle = getNewText("Items Title", itemsSection, Main.Randomizer.Localize("itmset") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            itemsTitle.anchoredPosition = new Vector2(0, top - 5);

            //RectTransform lungOption = getNewCheckbox("Lung", itemsSection, Main.Randomizer.Localize("lgname"), Main.Randomizer.Localize("lgdesc"), font, 15, 16);
            //lungOption.anchoredPosition = new Vector2(left, top - 70);
            //MistDamage = lungOption.GetComponent<SettingsCheckbox>();

            //RectTransform deathOption = getNewCheckbox("Death", itemsSection, Main.Randomizer.Localize("dename"), Main.Randomizer.Localize("dedesc"), font, 15, 16);
            //deathOption.anchoredPosition = new Vector2(left, top - 100);
            //NpcDeath = deathOption.GetComponent<SettingsCheckbox>();

            RectTransform wheelOption = getNewCheckbox("Wheel", itemsSection, Main.Randomizer.Localize("whname"), Main.Randomizer.Localize("whdesc"), font, 15, 16);
            wheelOption.anchoredPosition = new Vector2(left, top - 130);
            Wheel = wheelOption.GetComponent<SettingsCheckbox>();

            RectTransform reliqOption = getNewCheckbox("Reliquaries", itemsSection, Main.Randomizer.Localize("rqname"), Main.Randomizer.Localize("rqdesc"), font, 15, 16);
            reliqOption.anchoredPosition = new Vector2(left, top - 160);
            Reliquaries = reliqOption.GetComponent<SettingsCheckbox>();

            RectTransform itemsType = getNewCyclebox("Type", itemsSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("dstype"), Main.Randomizer.Localize("entype") },
                new string[] { Main.Randomizer.Localize("dstype") + " - " + Main.Randomizer.Localize("dsides"),
                               Main.Randomizer.Localize("entype") + " - " + Main.Randomizer.Localize("endesc") },
                new SettingsElement[] { Wheel, Reliquaries }, 36);
            itemsType.anchoredPosition = new Vector2(0, top - 25);
            ItemsLeft = itemsType.GetChild(0).GetComponent<SettingsCyclebox>();
            ItemsRight = itemsType.GetChild(1).GetComponent<SettingsCyclebox>();

            RectTransform startLocationText = getNewText("Start Loc Text", itemsSection, "Starting Location:", font, 16, Color.white, TextAnchor.MiddleCenter);
            startLocationText.anchoredPosition = new Vector2(0, top - 70);

            RectTransform startLocationOption = getNewCyclebox("Start Loc Option", itemsSection, font, 15, 16,
                new string[] { "Brotherhood", /*"Mercy Dreams",*/ "Knot of Words", "Library" },
                new string[] { "Begin the game in the Brotherhood of the Silent Sorrow", /*"Begin the game in Mercy Dreams",*/ "Begin the game in the Knot of the Three Words", "Begin the game in the Library of the Negated Words" },
                new SettingsElement[] { }, 52);
            startLocationOption.anchoredPosition = new Vector2(0, top - 90);
            StartingLocationLeft = startLocationOption.GetChild(0).GetComponent<SettingsCyclebox>();
            StartingLocationRight = startLocationOption.GetChild(1).GetComponent<SettingsCyclebox>();

            // Enemies section

            RectTransform enemiesSection = getNewRect("Enemies Section", mainSection);
            enemiesSection.sizeDelta = new Vector2(width / 4, height);
            enemiesSection.anchoredPosition = new Vector2(0.125f * width, 0);

            RectTransform enemiesTitle = getNewText("Enemies Title", enemiesSection, Main.Randomizer.Localize("emyset") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            enemiesTitle.anchoredPosition = new Vector2(0, top - 5);

            RectTransform classOption = getNewCheckbox("Class", enemiesSection, Main.Randomizer.Localize("clname"), Main.Randomizer.Localize("cldesc"), font, 15, 16);
            classOption.anchoredPosition = new Vector2(left, top - 70);
            MaintainClass = classOption.GetComponent<SettingsCheckbox>();

            RectTransform scalingOption = getNewCheckbox("Scaling", enemiesSection, Main.Randomizer.Localize("scname"), Main.Randomizer.Localize("scdesc"), font, 15, 16);
            scalingOption.anchoredPosition = new Vector2(left, top - 100);
            AreaScaling = scalingOption.GetComponent<SettingsCheckbox>();

            RectTransform enemiesType = getNewCyclebox("Type", enemiesSection, font, 15, 16,
                new string[] { Main.Randomizer.Localize("dstype"), Main.Randomizer.Localize("sftype"), Main.Randomizer.Localize("rdtype") },
                new string[] { Main.Randomizer.Localize("dstype") + " - " + Main.Randomizer.Localize("dsedes"),
                               Main.Randomizer.Localize("sftype") + " - " + Main.Randomizer.Localize("sfdesc"),
                               Main.Randomizer.Localize("rdtype") + " - " + Main.Randomizer.Localize("rddesc") },
                new SettingsElement[] { MaintainClass, AreaScaling }, 38);
            enemiesType.anchoredPosition = new Vector2(0, top - 25);
            EnemiesLeft = enemiesType.GetChild(0).GetComponent<SettingsCyclebox>();
            EnemiesRight = enemiesType.GetChild(1).GetComponent<SettingsCyclebox>();

            // Doors section

            RectTransform doorsSection = getNewRect("Doors Section", mainSection);
            doorsSection.sizeDelta = new Vector2(width / 4, height);
            doorsSection.anchoredPosition = new Vector2(0.375f * width, 0);

            RectTransform doorsTitle = getNewText("Doors Title", doorsSection, Main.Randomizer.Localize("dorset") + ":", font, 16, Color.white, TextAnchor.MiddleCenter);
            doorsTitle.anchoredPosition = new Vector2(0, top - 5);

            RectTransform comingSoon = getNewText("Coming soon", doorsSection, Main.Randomizer.Localize("soon"), font, 16, Color.white, TextAnchor.MiddleCenter);
            comingSoon.anchoredPosition = new Vector2(0, top - 40);

            // Set begin/cancel buttons
            begin.SetParent(doorsSection, false);
            begin.anchorMin = new Vector2(0.5f, 0.5f);
            begin.anchorMax = new Vector2(0.5f, 0.5f);
            begin.anchoredPosition = new Vector2(10, -50);
            begin.GetChild(1).GetComponent<Text>().text = " " + Main.Randomizer.Localize("begin");
            cancel.SetParent(doorsSection, false);
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

            RectTransform getNewCyclebox(string name, Transform parent, Font font, int boxSize, int fontSize, string[] options, string[] descs, SettingsElement[] boxes, int arrowDistance)
            {
                RectTransform rect = getNewText(name, parent, "", font, fontSize, Color.yellow, TextAnchor.MiddleCenter);

                RectTransform left = getNewImage("Left", rect, boxSize);
                left.anchoredPosition = new Vector2(-arrowDistance, 0);
                left.gameObject.AddComponent<SettingsCyclebox>().onStart(options, descs, boxes, false);

                RectTransform right = getNewImage("Right", rect, boxSize);
                right.anchoredPosition = new Vector2(arrowDistance, 0);
                right.gameObject.AddComponent<SettingsCyclebox>().onStart(options, descs, boxes, true);

                return rect;
            }
        }
    }
}
