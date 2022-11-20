using UnityEngine;
using Gameplay.UI;
using System.Diagnostics;
using BlasphemousRandomizer.Shufflers;
using BlasphemousRandomizer.Config;
using BlasphemousRandomizer.Structures;
using Framework.FrameworkCore;
using Framework.Managers;
using Tools.Level;

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

        // Randomizer data
        private string[] cutsceneNames;
        private Sprite[] customImages;          private bool forceSpoiler = true;

        public void Initialize()
        {
            // Load config
            fileConfig = FileUtil.loadConfig();
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
            shufflers = new IShuffle[] { itemShuffler, enemyShuffler, doorShuffler, hintShuffler };
            for (int i = 0; i < shufflers.Length; i++)
            {
                shufflers[i].Init();
            }

            // Load external data
            if (!FileUtil.parseFiletoArray("cutscenes_names.dat", out cutsceneNames))
                cutsceneNames = new string[0];
            loadCustomImages();

            // Set up data
            Core.Persistence.AddPersistentManager(this);
            LevelManager.OnLevelLoaded += onLevelLoaded;
            lastLoadedSlot = -1;
            Log("Randomizer has been initialized!");
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
                gameConfig = fileConfig;
                for (int i = 0; i < shufflers.Length; i++)
                {
                    shufflers[i].Reset();
                }
                Log("Loaded invalid game!");
                //Display error message
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
            //Fill hints
            //Fill enemies

            // Shuffle everything
            for (int i = 0; i < shufflers.Length; i++)
            {
                shufflers[i].Shuffle(seed);
            }

            // Generate spoiler on new game
            if (newGame || forceSpoiler)
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
            // Load enemies
            // Error on load

            // Update images of shop items
            if (scene == "D02BZ02S01" || scene == "D01BZ02S01" || scene == "D05BZ02S01")
            {
                foreach (SpriteRenderer spriteRenderer in Object.FindObjectsOfType<SpriteRenderer>())
                {
                    if (spriteRenderer.name == "Body" && spriteRenderer.sprite != null && "qi11rb02rb37qi58rb09rb05qi49rb12qi71".Contains(spriteRenderer.sprite.name))
                    {
                        Item item = itemShuffler.getItemAtLocation(spriteRenderer.sprite.name.ToUpper());
                        spriteRenderer.sprite = item == null ? null : item.getRewardInfo(true).sprite;
                    }
                }
            }
            // Give holy visage reward & disable altar
            else if (scene == "D01Z04S19")
            {
                itemShuffler.giveItem("QI38", true);
                Core.Events.SetFlag("ATTRITION_ALTAR_DONE", true, false);
                disableAltar("22c0f081-b3a0-4310-8a40-9506d4a1315c");
            }
            else if (scene == "D03Z03S16")
            {
                itemShuffler.giveItem("QI39", true);
                Core.Events.SetFlag("CONTRITION_ALTAR_DONE", true, false);
                disableAltar("27213fd3-b05b-4157-b067-5206321cacb7");
            }
            else if (scene == "D02Z03S21")
            {
                itemShuffler.giveItem("QI40", true);
                Core.Events.SetFlag("COMPUNCTION_ALTAR_DONE", true, false);
                disableAltar("bc2b17e1-5c8c-4a90-b7c8-160eacdd538d");
            }

            void disableAltar(string id)
            {
                foreach (Interactable interactable in Object.FindObjectsOfType<Interactable>())
                {
                    if (interactable.GetPersistenID() == id)
                    {
                        interactable.gameObject.SetActive(false);
                        return;
                    }
                }
            }
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

        public Sprite getImage(int idx)
        {
            return (customImages != null && idx >= 0 && idx < customImages.Length) ? customImages[idx] : null;
        }

        private bool isConfigVersionValid(string configVersion)
        {
            string version = MyPluginInfo.PLUGIN_VERSION;
            return version.Substring(version.IndexOf('.') + 1, 1) == configVersion.Substring(configVersion.IndexOf('.') + 1, 1);
        }

        private void loadCustomImages()
        {
            // Read bytes from file
            if (!FileUtil.readBytes("custom_images.png", out byte[] data))
            {
                Log("Error: Custom images could not be loaded!");
                return; 
            }

            // Convert to texture
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            int w = tex.width, h = tex.height;
            customImages = new Sprite[w * h / 1024];

            // Insert each 32x32 image into the array (T-B, L-R)
            int count = 0;
            for (int i = h - 32; i >= 0; i -= 32)
            {
                for (int j = 0; j < w; j += 32)
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(j, i, 32f, 32f), Vector2.zero);
                    customImages[count] = sprite;
                    count++;
                }
            }
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
