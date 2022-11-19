using UnityEngine;
using Gameplay.UI;
using System.Diagnostics;
using BlasphemousRandomizer.Shufflers;
using BlasphemousRandomizer.Config;
using Framework.FrameworkCore;
using Framework.Managers;

namespace BlasphemousRandomizer
{
    public class Randomizer : PersistentInterface
    {
        // Shufflers
        public ItemShuffle itemShuffler;
        public EnemyShuffle enemyShuffler;
        public DoorShuffle doorShuffler;
        public HintShuffle hintShuffler;

        // Config
        public MainConfig gameConfig;
        private MainConfig fileConfig;

        // Save file info
        private int seed;
        public int itemsCollected;
        public int totalItems;
        private bool startedInRando;

        private bool inGame;
        private int lastLoadedSlot;

        private string[] cutsceneNames;

        public void Initialize()
        {
            // Load config
            fileConfig = MainConfig.Default();//FileUtil.loadConfig();
            Log(fileConfig.general.teleportationAlwaysUnlocked.ToString());
            if (!isConfigVersionValid(fileConfig.versionCreated))
            {
                fileConfig = MainConfig.Default();
                FileUtil.saveConfig(fileConfig);
            }
            gameConfig = fileConfig;

            // Create main shufflers
            itemShuffler = new ItemShuffle();
            enemyShuffler = new EnemyShuffle();
            doorShuffler = new DoorShuffle();
            hintShuffler = new HintShuffle();

            // Load external data
            if (!FileUtil.parseFiletoArray("cutscenes_names.dat", out cutsceneNames))
                cutsceneNames = new string[0];

            // Set up data
            Core.Persistence.AddPersistentManager(this);
            lastLoadedSlot = -1;
            Log("Randomizer has been initialized!");
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
                gameConfig = fileConfig;
                Log("Loaded invalid game!");
                //Display error message
                //Reset shufflers
            }

            inGame = true;
            lastLoadedSlot = PersistentManager.GetAutomaticSlot();
        }

        // When new game is started
        public void newGame()
        {
            seed = generateSeed();
            itemsCollected = 0;
            startedInRando = true;
            gameConfig = fileConfig;
            setUpExtras();
            Log("Generating new seed: " + seed);
            Randomize(true);

            inGame = true;
            lastLoadedSlot = PersistentManager.GetAutomaticSlot();
        }

        private int generateSeed()
        {
            return fileConfig.general.customSeed > 0 ? fileConfig.general.customSeed : new System.Random().Next();
        }

        // Returned to title screen
        public void ResetPersistence()
        {
            inGame = false;
        }

        private void Randomize(bool newGame)
        {
            Stopwatch watch = Stopwatch.StartNew();
            //Fill doors
            itemShuffler.Shuffle(seed);
            //Fill hints
            //Fill enemies
            if (newGame)
            {
                //Generate spoiler
            }
            watch.Stop();
            Log("Time to fill seed: " + watch.ElapsedMilliseconds + " ms");
        }


        // Set up a new game
        private void setUpExtras()
        {
            // Set flags relating to choosing a penitence
            if (!fileConfig.general.enablePenitence)
            {
                Core.Events.SetFlag("PENITENCE_EVENT_FINISHED", true, false);
                Core.Events.SetFlag("PENITENCE_NO_PENITENCE", true, false);
            }
            // Set flags relating to various cutscenes
            if (fileConfig.general.skipCutscenes && FileUtil.parseFiletoArray("cutscenes_flags.dat", out string[] flags))
            {
                foreach (string id in flags)
                {
                    Core.Events.SetFlag(id, true, false);
                }
            }
        }

        // Keyboard input
        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                UIController.instance.ShowPopUp("Current seed: " + seed, "", 0, false);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                UIController.instance.ShowPopUp("Test", "", 0, false);
            }
        }

        // Log message to file
        public void Log(string message)
        {
            if (fileConfig.debug.type > 0)
                FileUtil.writeLine("log.txt", message + "\n");
        }

        public bool shouldSkipCutscene(string id)
        {
            return gameConfig.general.skipCutscenes && FileUtil.arrayContains(cutsceneNames, id);
        }

        private bool isConfigVersionValid(string configVersion)
        {
            string version = MyPluginInfo.PLUGIN_VERSION;
            return version.Substring(version.IndexOf('.') + 1, 1) == configVersion.Substring(configVersion.IndexOf('.') + 1, 1);
        }

        public string GetPersistenID()
        {
            return "ID_RANDOMIZER";
        }

        public int GetOrder()
        {
            return 0;
        }
    }
}
