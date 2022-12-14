using UnityEngine;
using Gameplay.UI;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using BlasphemousRandomizer.Shufflers;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;
using BlasphemousRandomizer.UI;
using Framework.FrameworkCore;
using Framework.Managers;
using Tools.Level;
using Framework.Audio;

namespace BlasphemousRandomizer
{
    public class Randomizer : PersistentInterface
    {
        // Shufflers
        public ItemShuffle itemShuffler;
        public EnemyShuffle enemyShuffler;
        public DoorShuffle doorShuffler;
        public HintShuffle hintShuffler;
        private IShuffle[] shufflers;

        // Save file info
        private int seed;
        public int itemsCollected;
        public int totalItems;
        private bool startedInRando;
        public MainConfig gameConfig;

        private bool inGame;
        private int lastLoadedSlot;
        private string errorOnLoad;
        private SettingsMenu settingsMenu;

        // Randomizer data
        private string[] cutsceneNames;
        private string[] interactableIds;
        private Sprite[] customImages;
        private Sprite[] uiImages;
        private Sprite[] uiArrows;
        private bool debugMode;

        public void Initialize()
        {
            // Create main shufflers
            itemShuffler = new ItemShuffle();
            enemyShuffler = new EnemyShuffle();
            doorShuffler = new DoorShuffle();
            hintShuffler = new HintShuffle();
            shufflers = new IShuffle[] { itemShuffler, enemyShuffler, doorShuffler, hintShuffler };
            for (int i = 0; i < shufflers.Length; i++)
            {
                shufflers[i].Init();
            }

            // Load external data
            debugMode = FileUtil.read("debug.txt", false, out string text) && text == "true";
            if (!FileUtil.parseFiletoArray("cutscenes_names.dat", out cutsceneNames))
                Log("Error: Cutscene names could not be loaded!");
            if (!FileUtil.parseFiletoArray("interactable_ids.dat", out interactableIds))
                Log("Error: Interactable ids could not be loaded!");
            if (!FileUtil.loadImages("custom_images.png", 32, 32, 0, out customImages))
                Log("Error: Custom images could not be loaded!");
            if (!FileUtil.loadImages("ui.png", 36, 36, 0, out uiImages))
                Log("Error: UI images could not be loaded!");
            if (!FileUtil.loadImages("arrows.png", 15, 32, 0, out uiArrows))
                Log("Error: UI arrows could not be loaded!");

            // Set up data
            Core.Persistence.AddPersistentManager(this);
            LevelManager.OnLevelLoaded += onLevelLoaded;
            gameConfig = MainConfig.Default();
            lastLoadedSlot = -1;
            errorOnLoad = "";
            settingsMenu = new SettingsMenu();
            Log("\nRandomizer has been initialized!");
        }

        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
        }

        // When game is saved
        public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
        {
            return new RandomizerPersistenceData
            {
                seed = seed,
                itemsCollected = itemsCollected,
                startedInRando = startedInRando,
                config = gameConfig
            };
        }

        // When game is loaded
        public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
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
                Log("Loaded invalid game!");
                errorOnLoad = "This save file was not created in randomizer or used an older version.  Item locations are invalid!";
            }

            inGame = true;
            lastLoadedSlot = PersistentManager.GetAutomaticSlot();
        }

        // When new game is started
        public void newGame()
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
        }

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
                shufflers[i].Shuffle(seed);
            }

            // Show error message if item shuffler failed
            if (itemShuffler.getNewItems().Count == 0)
                errorOnLoad = "Item shuffler failed to generate valid game.  Item locations are invalid!";

            // Generate spoiler on new game
            if (newGame)
            {
                string spoiler = "";
                for (int i = 0; i < shufflers.Length; i++)
                {
                    spoiler += shufflers[i].GetSpoiler();
                }
                FileUtil.writeFull($"spoiler{PersistentManager.GetAutomaticSlot() + 1}.txt", spoiler);
            }
            watch.Stop();
            Log("Time to fill seed: " + watch.ElapsedMilliseconds + " ms");
        }

        // Specific actions need to be taken when a certain scene is loaded
        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            string scene = newLevel.LevelName;

            // Set gameplay status
            if (scene == "MainMenu")
                inGame = false;

            // Display delayed error message
            if (errorOnLoad != "")
                UIController.instance.StartCoroutine(showErrorMessage(2.1f));

            // Load enemies
            EnemyLoader.loadEnemies();

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
            if (gameConfig.general.skipCutscenes && FileUtil.parseFiletoArray("cutscenes_flags.dat", out string[] flags))
            {
                foreach (string id in flags)
                {
                    Core.Events.SetFlag(id, true, false);
                }
            }
            // Set randomized flags
            string majorVersion = MyPluginInfo.PLUGIN_VERSION;
            majorVersion = majorVersion.Substring(0, majorVersion.LastIndexOf('.'));
            Core.Events.SetFlag("RANDOMIZED", true, false);
            Core.Events.SetFlag(majorVersion, true, false);
        }

        // Keyboard input
        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad6) && inGame)
            {
                LogDisplay("Current seed: " + seed);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                //enemyShuffler.Shuffle(new System.Random().Next());
                //UIController.instance.ShowPopUp("Shuffling enemies temporarily!", "", 0, false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                //LogFile(EnemyShuffle.enemyData);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                
            }

            // Update ui menus
            if (settingsMenu != null)
                settingsMenu.update();
        }

        // Log message to file
        public void Log(string message)
        {
            if (debugMode)
                FileUtil.writeLine("log.txt", message + "\n");
        }

        // Log message to UI display
        public void LogDisplay(string message, bool block = false)
        {
            Log(message);
            UIController.instance.ShowPopUp(message, "", 0, block);
        }

        // Log data to file
        public void LogFile(string data)
        {
            if (debugMode)
                FileUtil.writeFull("data.txt", data);
        }

        private IEnumerator showErrorMessage(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            LogDisplay(errorOnLoad, true);
            errorOnLoad = "";
        }

        public bool shouldSkipCutscene(string id)
        {
            return gameConfig.general.skipCutscenes && FileUtil.arrayContains(cutsceneNames, id);
        }

        public bool isSpecialInteractable(string id)
        {
            return FileUtil.arrayContains(interactableIds, id);
        }

        public void playSoundEffect(int id)
        {
            if (id == 0) Core.Audio.PlayOneShot("event:/SFX/UI/EquipItem");
            else if (id == 1) Core.Audio.PlayOneShot("event:/SFX/UI/UnequipItem");
            else if (id == 2) Core.Audio.PlayOneShot("event:/SFX/UI/ChangeSelection");
        }

        public Sprite getImage(int idx)
        {
            return (idx >= 0 && idx < customImages.Length) ? customImages[idx] : null;
        }
        public Sprite getUI(int idx)
        {
            return (idx >= 0 && idx < uiImages.Length) ? uiImages[idx] : null;
        }
        public Sprite getUIArrow(int idx)
        {
            return (idx >= 0 && idx < uiArrows.Length) ? uiArrows[idx] : null;
        }

        public SettingsMenu getSettingsMenu()
        {
            return settingsMenu;
        }

        private bool isConfigVersionValid(string configVersion)
        {
            string version = MyPluginInfo.PLUGIN_VERSION;
            return version.Substring(0, version.LastIndexOf('.')) == configVersion.Substring(0, configVersion.LastIndexOf('.'));
        }

        public string GetPersistenID() { return "ID_RANDOMIZER"; }

        public int GetOrder() { return 0; }

        public void ResetPersistence() { }
    }
}
