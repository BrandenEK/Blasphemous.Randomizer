using UnityEngine;
using Gameplay.UI;
using System.Diagnostics;
using System.Collections;
using BlasphemousRandomizer.Shufflers;
using BlasphemousRandomizer.Structures;
using BlasphemousRandomizer.Tracker;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.UI;
using Framework.Managers;
using Framework.Audio;
using Tools.Level;
using ModdingAPI;

namespace BlasphemousRandomizer
{
    public class Randomizer : PersistentMod
    {
        public const int MAX_SEED = 999999;

        // Shufflers
        public ItemShuffle itemShuffler;
        public EnemyShuffle enemyShuffler;
        public BossShuffle bossShuffler;
        public DoorShuffle doorShuffler;
        public HintShuffle hintShuffler;
        private IShuffle[] shufflers;

        // Save file info
        private int seed;
        public int itemsCollected;
        public int totalItems;
        private bool startedInRando;
        public MainConfig gameConfig;

        // Global info
        private bool inGame;
        private int lastLoadedSlot;
        private string errorOnLoad;
        public bool ShrineEditMode { get; set; }

        public DataStorage data { get; private set; }
        public AutoTracker tracker { get; private set; }
        public SettingsMenu settingsMenu;

        public override string PersistentID => "ID_RANDOMIZER";

        public Randomizer(string modId, string modName, string modVersion) : base(modId, modName, modVersion) { }

        protected override void Initialize()
        {
            // Create main shufflers
            itemShuffler = new ItemShuffle();
            enemyShuffler = new EnemyShuffle();
            bossShuffler = new BossShuffle();
            doorShuffler = new DoorShuffle();
            hintShuffler = new HintShuffle();
            shufflers = new IShuffle[] { itemShuffler, enemyShuffler, bossShuffler, doorShuffler, hintShuffler };
            for (int i = 0; i < shufflers.Length; i++)
            {
                shufflers[i].Init();
            }

            // Load external data
            Log("Randomizer has been initialized!");
            data = new DataStorage();
            if (!data.loadData(FileUtil))
                errorOnLoad = Localize("daterr");

            // Set up data
            gameConfig = MainConfig.Default();
            lastLoadedSlot = -1;
            settingsMenu = new SettingsMenu();

            tracker = new AutoTracker();
            RegisterCommand(new RandomizerCommand());
        }

        public override ModPersistentData SaveGame()
        {
            return new RandomizerPersistenceData
            {
                seed = seed,
                itemsCollected = itemsCollected,
                startedInRando = startedInRando,
                config = gameConfig
            };
        }

        public override void LoadGame(ModPersistentData data)
        {
            // Only reload data if coming from title screen and loading different save file
            if (inGame || PersistentManager.GetAutomaticSlot() == lastLoadedSlot)
            {
                return;
            }

            RandomizerPersistenceData randomizerPersistenceData = data == null ? null : (RandomizerPersistenceData)data;
            if (randomizerPersistenceData != null && randomizerPersistenceData.startedInRando && isConfigVersionValid(randomizerPersistenceData.config.versionCreated))
            {
                // Loaded a valid randomized game
                seed = randomizerPersistenceData.seed;
                itemsCollected = randomizerPersistenceData.itemsCollected;
                startedInRando = randomizerPersistenceData.startedInRando;
                gameConfig = randomizerPersistenceData.config;
                Log("Loading seed: " + seed);
                Randomize(false);
            }
            else
            {
                // Loaded a vanilla game or an outdated rando game
                seed = -1;
                itemsCollected = 0;
                totalItems = 0;
                startedInRando = false;
                gameConfig = MainConfig.Default();
                for (int i = 0; i < shufflers.Length; i++)
                {
                    shufflers[i].Reset();
                }
                LogError("Loaded invalid game!");
                errorOnLoad = Localize("vererr");
            }

            inGame = true;
            lastLoadedSlot = PersistentManager.GetAutomaticSlot();
        }

        public override void NewGame(bool NGPlus)
        {
            itemsCollected = 0;
            startedInRando = true;
            gameConfig = settingsMenu.getConfigSettings();
            seed = generateSeed();
            setUpExtras();
            Log("Generating new seed: " + seed);
            Randomize(true);

            inGame = true;
            lastLoadedSlot = PersistentManager.GetAutomaticSlot();
            Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
        }

        public override void ResetGame() { }

        private int generateSeed()
        {
            return gameConfig.general.customSeed > 0 ? gameConfig.general.customSeed : new System.Random().Next();
        }

        private void Randomize(bool newGame)
        {
            Stopwatch watch = Stopwatch.StartNew();

            // Shuffle everything
            for (int i = 0; i < shufflers.Length; i++)
            {
                try
                {
                    shufflers[i].Shuffle(seed);
                }
                catch (System.Exception)
                {
                    LogError($"Error with the {shufflers[i].GetType().Name} when shuffling seed {seed}");
                }
            }

            // Show error message if item shuffler failed
            if (!itemShuffler.validSeed)
                errorOnLoad = Localize("generr");

            // Generate spoiler on new game
            if (newGame)
            {
                string spoiler = "";
                for (int i = 0; i < shufflers.Length; i++)
                {
                    spoiler += shufflers[i].GetSpoiler();
                }
                FileUtil.saveTextFile($"spoiler{PersistentManager.GetAutomaticSlot() + 1}.txt", spoiler);
            }
            watch.Stop();
            Log("Time to fill seed: " + watch.ElapsedMilliseconds + " ms");
        }

        // Specific actions need to be taken when a certain scene is loaded
        protected override void LevelLoaded(string oldLevel, string newLevel)
        {
            // Set gameplay status
            if (newLevel == "MainMenu")
                inGame = false;

            // Update ui menus
            if (settingsMenu != null)
                settingsMenu.onLoad(newLevel);

            // Display delayed error message
            if (errorOnLoad != null && errorOnLoad != "")
                UIController.instance.StartCoroutine(showErrorMessage(2.1f));

            // Misc functions
            EnemyLoader.loadEnemies(); // Load enemies
            updateShops(); // Update shop menus
            bossShuffler.levelLoaded(newLevel); // Spawn boss stuff
            tracker.LevelLoaded(newLevel);

            // Reload enemy audio catalogs
            AudioLoader audio = Object.FindObjectOfType<AudioLoader>();
            if (audio != null)
            {
                enemyShuffler.audioCatalogs = new FMODAudioCatalog[audio.AudioCatalogs.Length];
                audio.AudioCatalogs.CopyTo(enemyShuffler.audioCatalogs, 0);
                GameObject obj = audio.gameObject;
                Object.Destroy(audio);
                obj.AddComponent<AudioLoader>();
            }
        }

        // Set up a new game
        private void setUpExtras()
        {
            // Set flags relating to choosing a penitence
            if (!gameConfig.general.enablePenitence)
            {
                Core.Events.SetFlag("PENITENCE_EVENT_FINISHED", true, false);
                Core.Events.SetFlag("PENITENCE_NO_PENITENCE", true, false);
            }
            // Set flags relating to various cutscenes
            if (gameConfig.general.skipCutscenes)
            {
                foreach (string id in data.cutsceneFlags)
                {
                    Core.Events.SetFlag(id, true, false);
                }
            }
            // Set randomized flags
            string majorVersion = Main.MOD_VERSION;
            majorVersion = majorVersion.Substring(0, majorVersion.LastIndexOf('.'));
            Core.Events.SetFlag("RANDOMIZED", true, false);
            Core.Events.SetFlag(majorVersion, true, false);
        }

        // Update candelaria shops when opened or when purchased
        public void updateShops()
        {
            string scene = Core.LevelManager.currentLevel.LevelName;
            // Shop scenes - search for each item pedestal
            if (scene == "D02BZ02S01" || scene == "D01BZ02S01" || scene == "D05BZ02S01")
            {
                foreach (Interactable interactable in Object.FindObjectsOfType<Interactable>())
                {
                    if (data.interactableIds.ContainsKey(interactable.GetPersistenID()))
                    {
                        SpriteRenderer render = interactable.transform.parent.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                        if (render != null)
                        {
                            Item item = Main.Randomizer.itemShuffler.getItemAtLocation(data.interactableIds[interactable.GetPersistenID()]);
                            render.sprite = item == null ? null : item.getRewardInfo(true).sprite;
                        }
                    }
                }
            }
        }

        // Keyboard input
        protected override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad6) && inGame)
            {
                LogDisplay($"{Localize("currsd")}: {seed} [{ComputeFinalSeed(seed, gameConfig.items.type, gameConfig.items.lungDamage, gameConfig.items.startWithWheel, gameConfig.items.shuffleReliquaries)}]");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //enemyShuffler.Shuffle(new System.Random().Next());
                //UIController.instance.ShowPopUp("Shuffling enemies temporarily!", "", 0, false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                //LogFile(EnemyShuffle.enemyData);
                //tracker.Connect();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                
            }

            // Update ui menus
            if (settingsMenu != null)
                settingsMenu.update();
        }

        private IEnumerator showErrorMessage(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            LogDisplay(errorOnLoad);
            errorOnLoad = "";
        }

        public bool shouldSkipCutscene(string id)
        {
            return gameConfig.general.skipCutscenes && Main.arrayContains(data.cutsceneNames, id);
        }

        public void playSoundEffect(int id)
        {
            if (id == 0) Core.Audio.PlayOneShot("event:/SFX/UI/EquipItem");
            else if (id == 1) Core.Audio.PlayOneShot("event:/SFX/UI/UnequipItem");
            else if (id == 2) Core.Audio.PlayOneShot("event:/SFX/UI/ChangeSelection");
            else if (id == 3) Core.Audio.PlayOneShot("event:/SFX/UI/FadeToWhite");
        }

        private bool isConfigVersionValid(string configVersion)
        {
            string version = Main.MOD_VERSION;
            return version.Substring(0, version.LastIndexOf('.')) == configVersion.Substring(0, configVersion.LastIndexOf('.'));
        }

        public int ComputeFinalSeed(int seed, int itemShuffle, bool mistDamage, bool startWithWheel, bool shuffleReliquaries)
        {
            // Generate unique int32 based on seed and important options
            int uniqueSeed = 0;

            // Remaining bits: 24, 25, 26, 27, 28, 29, 30, 31
            uniqueSeed |= (seed & (1 << 0)) << 18;
            uniqueSeed |= (seed & (1 << 1)) << 21;
            uniqueSeed |= (seed & (1 << 2)) << 10;
            uniqueSeed |= (seed & (1 << 3)) << 18;
            uniqueSeed |= (seed & (1 << 4)) >> 4;
            uniqueSeed |= (seed & (1 << 5)) << 1;
            uniqueSeed |= (seed & (1 << 6)) << 4;
            uniqueSeed |= (seed & (1 << 7)) << 2;
            uniqueSeed |= (seed & (1 << 8)) >> 5;
            uniqueSeed |= (seed & (1 << 9)) << 5;
            uniqueSeed |= (seed & (1 << 10)) << 10;
            uniqueSeed |= (seed & (1 << 11)) >> 3;
            uniqueSeed |= (seed & (1 << 12)) >> 8;
            uniqueSeed |= (seed & (1 << 13)) << 2;
            uniqueSeed |= (seed & (1 << 14)) >> 12;
            uniqueSeed |= (seed & (1 << 15)) >> 4;
            uniqueSeed |= (seed & (1 << 16)) << 3;
            uniqueSeed |= (seed & (1 << 17)) << 6;
            uniqueSeed |= (seed & (1 << 18)) >> 1;
            uniqueSeed |= (seed & (1 << 19)) >> 18;
            if (itemShuffle > 0)
            {
                uniqueSeed |= 1 << 7;
                if (mistDamage) uniqueSeed |= 1 << 13;
                if (startWithWheel) uniqueSeed |= 1 << 16;
                if (shuffleReliquaries) uniqueSeed |= 1 << 5;
            }
            return uniqueSeed;
        }
    }
}
