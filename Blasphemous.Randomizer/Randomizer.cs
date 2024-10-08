﻿using Blasphemous.CheatConsole;
using Blasphemous.Framework.Credits;
using Blasphemous.Framework.Menus;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Helpers;
using Blasphemous.ModdingAPI.Persistence;
using Blasphemous.Randomizer.BossRando;
using Blasphemous.Randomizer.DoorRando;
using Blasphemous.Randomizer.EnemyRando;
using Blasphemous.Randomizer.Extensions;
using Blasphemous.Randomizer.HintRando;
using Blasphemous.Randomizer.ItemRando;
using Blasphemous.Randomizer.Map;
using Blasphemous.Randomizer.Services;
using Framework.Audio;
using Framework.Managers;
using Gameplay.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Tools.Level;
using Tools.Level.Actionables;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Randomizer
{
    /// <summary>
    /// Handles randomizing everything
    /// </summary>
    public class Randomizer : BlasMod, IPersistentMod
    {
        internal Randomizer() : base(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_AUTHOR, ModInfo.MOD_VERSION) { }

        private const bool SKIP_CUTSCENES = true;

        // Shufflers
        public ItemShuffle itemShuffler;
        public EnemyShuffle enemyShuffler;
        public BossShuffle bossShuffler;
        public HintShuffle hintShuffler;
        private IShuffle[] shufflers;

        // Save file info
        public Config GameSettings { get; set; }

        // Global info
        private bool inGame;
        private string errorOnLoad;
        internal bool ShrineEditMode { get; set; }

        public DataStorage data { get; private set; }
        public MapCollectionStatus MapCollection { get; private set; }
        public RandomizerMenu ModMenu { get; private set; }

        public string PersistentID => "ID_RANDOMIZER";

        public bool InstalledBootsMod => ModHelper.IsModLoadedByName("Boots of Pleading");
        public bool InstalledDoubleJumpMod => ModHelper.IsModLoadedByName("Double Jump");
        public bool CanDash => !GameSettings.ShuffleDash || Core.Events.GetFlag("ITEM_Slide");
        public bool CanWallClimb => !GameSettings.ShuffleWallClimb || Core.Events.GetFlag("ITEM_WallClimb");
        public bool DashChecker { get; set; }

        protected override void OnInitialize()
        {
            LocalizationHandler.RegisterDefaultLanguage("en");
            InputHandler.RegisterDefaultKeybindings(new Dictionary<string, KeyCode>()
            {
                {  "Seed", KeyCode.F8 }
            });

            // Create main shufflers
            itemShuffler = new ItemShuffle();
            enemyShuffler = new EnemyShuffle();
            bossShuffler = new BossShuffle();
            hintShuffler = new HintShuffle();
            shufflers = new IShuffle[] { itemShuffler, enemyShuffler, bossShuffler, hintShuffler };
            for (int i = 0; i < shufflers.Length; i++)
            {
                shufflers[i].Init();
            }

            // Load external data
            ModLog.Info("Randomizer has been initialized!");
            data = new DataStorage();
            if (!data.loadData())
                errorOnLoad = LocalizationHandler.Localize("daterr");

            // Set up data
            GameSettings = new Config();
            MapCollection = new MapCollectionStatus();
            ModMenu = new RandomizerMenu();
        }

        protected override void OnRegisterServices(ModServiceProvider provider)
        {
            provider.RegisterCommand(new RandomizerCommand());
            provider.RegisterCredits(new RandomizerCredits());
            provider.RegisterNewGameMenu(ModMenu);
        }

        public SaveData SaveGame()
        {
            return new RandomizerPersistenceData
            {
                config = GameSettings,
                mappedItems = itemShuffler.SaveMappedItems(),
                mappedDoors = itemShuffler.SaveMappedDoors(),
                mappedHints = hintShuffler.SaveMappedHints(),
                mappedEnemies = enemyShuffler.SaveMappedEnemies(),
                collectionStatus = MapCollection.CollectionStatus
            };
        }

        public void LoadGame(SaveData data)
        {
            RandomizerPersistenceData randomizerPersistenceData = data as RandomizerPersistenceData;

            GameSettings = randomizerPersistenceData.config;
            itemShuffler.LoadMappedItems(randomizerPersistenceData.mappedItems);
            itemShuffler.LoadMappedDoors(randomizerPersistenceData.mappedDoors);
            hintShuffler.LoadMappedHints(randomizerPersistenceData.mappedHints);
            enemyShuffler.LoadMappedEnemies(randomizerPersistenceData.mappedEnemies);
            MapCollection.CollectionStatus = randomizerPersistenceData.collectionStatus;

            ModLog.Info("Loaded seed: " + GameSettings.Seed);
        }

        protected override void OnNewGame()
        {
            ModLog.Info("Generating new seed: " + GameSettings.Seed);
            Randomize();
            MapCollection.ResetCollectionStatus(GameSettings);

            setUpExtras();
            Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
            Core.Events.SetFlag("CHERUB_RESPAWN", true);
        }

        public void ResetGame()
        {
            GameSettings = new Config();
            itemShuffler.ClearMappedItems();
            itemShuffler.ClearMappedDoors();
            hintShuffler.ClearMappedHints();
            enemyShuffler.ClearMappedEnemies();
            MapCollection.ResetCollectionStatus(GameSettings);
        }

        private void Randomize()
        {
            Stopwatch watch = Stopwatch.StartNew();

            // Shuffle everything
            for (int i = 0; i < shufflers.Length; i++)
            {
                try
                {
                    shufflers[i].Shuffle(GameSettings.Seed);
                }
                catch (System.Exception e)
                {
                    ModLog.Error($"Error with the {shufflers[i].GetType().Name} when shuffling seed {GameSettings.Seed}");
                    ModLog.Error("Error message: " + e.Message);
                    ResetGame();
                }
            }

            if (itemShuffler.ValidSeed)
            {
                // Generate spoiler on new game
                string path = $"{FileHandler.ContentFolder}spoiler{PersistentManager.GetAutomaticSlot() + 1}.txt";
                string spoiler = itemShuffler.GetSpoiler();
                File.WriteAllText(path, spoiler);
            }
            watch.Stop();
            ModLog.Info("Time to fill seed: " + watch.ElapsedMilliseconds + " ms");
        }

        // Before spawning player, might have to change the spawn point of a few doors
        protected override void OnLevelPreloaded(string oldLevel, string newLevel)
        {
            FixDoorsOnPreload(newLevel);
        }

        // Specific actions need to be taken when a certain scene is loaded
        protected override void OnLevelLoaded(string oldLevel, string newLevel)
        {
            if (newLevel == "MainMenu")
            {
                // Set gameplay status
                inGame = false;
                itemShuffler.LastDoor = null;
            }
            else if (newLevel == GameSettings.RealStartingLocation.Room)
            {
                // Give first item when starting a new game
                itemShuffler.giveItem("QI106", true);
            }
            else if (newLevel == "D01Z02S07")
            {
                // Activate Albero warp room when entering it
                Core.Events.SetFlag("ALBERO_WARP", true);
            }

            if (newLevel != "MainMenu")
            {
                // Control albero warp room activation
                inGame = true;
                Core.SpawnManager.SetTeleportActive("TELEPORT_D02", Core.Events.GetFlag("ALBERO_WARP"));

                // Validate save data
                if (!itemShuffler.ValidSeed)
                {
                    // Loaded an outdated rando or vanilla game
                    ModLog.Error("Loaded invalid game!");
                    errorOnLoad = LocalizationHandler.Localize("saverr");
                    ResetGame();
                }
            }

            // Display delayed error message
            if (errorOnLoad != null && errorOnLoad != "")
                UIController.instance.StartCoroutine(showErrorMessage(2.1f));

            // Misc functions
            FixDoorsOnLoad(newLevel); // Perform door fixes such as closing gates & revealing secrets
            FixRooftopsElevator(newLevel); // Keep rooftops elevator at top
            updateShops(); // Update shop menus
            bossShuffler.levelLoaded(newLevel); // Spawn boss stuff
            EnemyLoader.loadEnemies(); // Load enemies

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

        // Control position of Rooftops elevator
        private void FixRooftopsElevator(string scene)
        {
            if (GameSettings.DoorShuffleType <= 0) return;

            if (scene == "D06Z01S01")
            {
                // Keep the elevator set at position 4
                Core.Events.SetFlag("ELEVATOR_POSITION_1", false);
            }
            else if (scene == "D06Z01S19")
            {
                // Prevent the elevator crashing when returning to main room
                Core.Events.SetFlag("ELEVATOR_POSITION_FAKE", true);
            }
        }

        // Set up a new game
        private void setUpExtras()
        {
            // Set flags relating to choosing a penitence
            if (!GameSettings.AllowPenitence)
            {
                Core.Events.SetFlag("PENITENCE_EVENT_FINISHED", true, false);
                Core.Events.SetFlag("PENITENCE_NO_PENITENCE", true, false);
            }
            // Set flags relating to various cutscenes
            if (SKIP_CUTSCENES)
            {
                foreach (string id in data.CutsceneFlags)
                {
                    Core.Events.SetFlag(id, true, false);
                }
            }
            // Set randomized flags
            string majorVersion = ModInfo.MOD_VERSION;
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
                    if (data.ShopInteractables.ContainsKey(interactable.GetPersistenID()))
                    {
                        SpriteRenderer render = interactable.transform.parent.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                        Item item = Main.Randomizer.itemShuffler.getItemAtLocation(data.ShopInteractables[interactable.GetPersistenID()]);

                        if (render != null && item != null)
                        {
                            if (!item.UseDefaultImageScaling)
                                render.transform.localScale = new Vector3(0.9375f, 0.9375f, 0.9375f);
                            render.sprite = item.GetImage(true);
                        }
                    }
                }
            }
        }

        // Keyboard input
        protected override void OnUpdate()
        {
            if (InputHandler.GetKeyDown("Seed") && inGame)
            {
                ModLog.Display($"{LocalizationHandler.Localize("currsd")}: {GameSettings.Seed} [{GameSettings.UniqueSeed}]");
            }
            else if (InputHandler.GetKeyDown("Debug"))
            {
                //int succeed = 0, total = 100;
                //System.Random rng = new System.Random();
                //Dictionary<string, string> doors = new Dictionary<string, string>();
                //Dictionary<string, string> items = new Dictionary<string, string>();

                //for (int i = 0; i < total; i++)
                //{
                //    int testSeed = rng.Next(1, MAX_SEED);
                //    bool success = new ItemFiller().Fill(testSeed, doors, items);
                //    ModLog.Warn($"Trying seed {testSeed}: {success}");
                //    if (success)
                //        succeed++;
                //}
                //ModLog.Error($"Success rate: {succeed}/{total}");
            }
        }

        private IEnumerator showErrorMessage(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            ModLog.Display(errorOnLoad);
            errorOnLoad = string.Empty;
        }

        public bool shouldSkipCutscene(string id)
        {
            return SKIP_CUTSCENES && data.CutsceneNames.Contains(id);
        }

        private void FixDoorsOnPreload(string scene)
        {
            // If entering from a certain door, change the spawn point
            DoorLocation doorToEnter = itemShuffler.LastDoor;
            if (doorToEnter == null || scene != doorToEnter.Room || !data.FixedDoorPositions.TryGetValue(doorToEnter.Id, out Vector3 newPosition))
                return;

            string doorId = doorToEnter.IdentityName;
            Door[] doors = Object.FindObjectsOfType<Door>();
            foreach (Door door in doors)
            {
                if (door.identificativeName == doorId)
                {
                    ModLog.Warn($"Modifiying spawn point of {doorId} door");
                    door.spawnPoint.position = newPosition;
                    break;
                }
            }
        }

        private void FixDoorsOnLoad(string scene)
        {
            if (scene == "D17Z01S11" || scene == "D05Z02S14" || scene == "D01Z04S18")
            {
                // Disable right wall in Warden & Exposito & Piety boss room
                BossBoundaryStatus = false;
            }
            else if (scene == "D03BZ01S01" || scene == "D03Z03S15")
            {
                // Close Anguish boss fight gate when entering
                bool shouldBeOpen = scene == "D03Z03S15";
                Gate[] gates = Object.FindObjectsOfType<Gate>();
                foreach (Gate gate in gates)
                {
                    if (gate.IsOpenOrActivated() != shouldBeOpen)
                        gate.Use();
                }
            }

            // If entering from a certain door, remove the wall
            DoorLocation doorToEnter = itemShuffler.LastDoor;
            if (doorToEnter == null || scene != doorToEnter.Room || !data.FixedDoorWalls.TryGetValue(doorToEnter.Id, out string wallToRemove))
                return;

            GameObject parent = GameObject.Find("INTERACTABLES");
            if (parent == null) return;

            ModLog.Warn("Disabling hidden wall for " + doorToEnter.Id);
            parent.transform.Find(wallToRemove).gameObject.SetActive(false);
            Core.Events.SetFlag("HIDDEN_WALL_" + doorToEnter.Room, true);
        }

        public bool BossBoundaryStatus
        {
            set
            {
                string currentScene = Core.LevelManager.currentLevel.LevelName;
                try
                {
                    if (currentScene == "D17Z01S11")
                        GameObject.Find("CAMERAS").transform.Find("CombatBoundaries").gameObject.SetActive(value);
                    else if (currentScene == "D01Z04S18")
                        GameObject.Find("CAMERAS").transform.Find("CombatBoundaries").gameObject.SetActive(value);
                    else if (currentScene == "D02Z03S20")
                    {
                        GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatElements").gameObject.SetActive(value);
                        GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatElementsAfterCombat").gameObject.SetActive(value);
                        GameObject.Find("LOGIC").transform.Find("SCRIPTS/VisualFakeWallLeft").gameObject.SetActive(value);
                        Transform leftWall = GameObject.Find("LOGIC").transform.Find("SCRIPTS/VisualLeftDoorFrame");
                        leftWall.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                        leftWall.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else if (currentScene == "D05Z02S14")
                        GameObject.Find("LOGIC").transform.Find("SCRIPTS/CombatBoundaries").gameObject.SetActive(value);

                    ModLog.Warn($"Setting boss boundary status to {value}");
                }
                catch (System.NullReferenceException)
                {
                    ModLog.Error("Boss boundary path could not be found!");
                }
            }
        }
    }
}
