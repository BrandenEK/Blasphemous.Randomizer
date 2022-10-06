using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Tools.Level;
using UnityEngine;

namespace Framework.Managers
{
	[DefaultExecutionOrder(-2)]
	public class Core : Singleton<Core>
	{
		public void PreInit()
		{
			if (Core.ready)
			{
				return;
			}
			Log.Raw("============================================== (Initialization: Framework)");
			this.ReadParameters();
			EventManager eventManager = new EventManager();
			Core.Events = eventManager;
			this.AddSystem(eventManager);
			MetricManager metricManager = new MetricManager();
			Core.Metrics = metricManager;
			this.AddSystem(metricManager);
			LogicManager logicManager = new LogicManager();
			Core.Logic = logicManager;
			this.AddSystem(logicManager);
			LevelManager levelManager = new LevelManager();
			Core.LevelManager = levelManager;
			this.AddSystem(levelManager);
			UIManager uimanager = new UIManager();
			Core.UI = uimanager;
			this.AddSystem(uimanager);
			InputManager inputManager = new InputManager();
			Core.Input = inputManager;
			this.AddSystem(inputManager);
			FMODAudioManager fmodaudioManager = new FMODAudioManager();
			Core.Audio = fmodaudioManager;
			this.AddSystem(fmodaudioManager);
			PersistentManager persistentManager = new PersistentManager();
			Core.Persistence = persistentManager;
			this.AddSystem(persistentManager);
			ScreenManager screenManager = new ScreenManager();
			Core.Screen = screenManager;
			this.AddSystem(screenManager);
			InventoryManager inventoryManager = new InventoryManager();
			Core.InventoryManager = inventoryManager;
			this.AddSystem(inventoryManager);
			SpawnManager spawnManager = new SpawnManager();
			Core.SpawnManager = spawnManager;
			this.AddSystem(spawnManager);
			LocalizationManager localizationManager = new LocalizationManager();
			Core.Localization = localizationManager;
			this.AddSystem(localizationManager);
			DialogManager dialogManager = new DialogManager();
			Core.Dialog = dialogManager;
			this.AddSystem(dialogManager);
			Cinematics cinematics = new Cinematics();
			Core.Cinematics = cinematics;
			this.AddSystem(cinematics);
			ExceptionHandler exceptionHandler = new ExceptionHandler();
			Core.ExceptionHandler = exceptionHandler;
			this.AddSystem(exceptionHandler);
			SkillManager skillManager = new SkillManager();
			Core.SkillManager = skillManager;
			this.AddSystem(skillManager);
			MapManager mapManager = new MapManager();
			Core.MapManager = mapManager;
			this.AddSystem(mapManager);
			NewMapManager newMapManager = new NewMapManager();
			Core.NewMapManager = newMapManager;
			this.AddSystem(newMapManager);
			CamerasManager camerasManager = new CamerasManager();
			Core.CamerasManager = camerasManager;
			this.AddSystem(camerasManager);
			GuiltManager guiltManager = new GuiltManager();
			Core.GuiltManager = guiltManager;
			this.AddSystem(guiltManager);
			DLCManager dlcmanager = new DLCManager();
			Core.DLCManager = dlcmanager;
			this.AddSystem(dlcmanager);
			AchievementsManager achievementsManager = new AchievementsManager();
			Core.AchievementsManager = achievementsManager;
			this.AddSystem(achievementsManager);
			ColorPaletteManager colorPaletteManager = new ColorPaletteManager();
			Core.ColorPaletteManager = colorPaletteManager;
			this.AddSystem(colorPaletteManager);
			TutorialManager tutorialManager = new TutorialManager();
			Core.TutorialManager = tutorialManager;
			this.AddSystem(tutorialManager);
			ControlRemapManager controlRemapManager = new ControlRemapManager();
			Core.ControlRemapManager = controlRemapManager;
			this.AddSystem(controlRemapManager);
			GameModeManager gameModeManager = new GameModeManager();
			Core.GameModeManager = gameModeManager;
			this.AddSystem(gameModeManager);
			PatchNotesManager patchNotesManager = new PatchNotesManager();
			Core.PatchNotesManager = patchNotesManager;
			this.AddSystem(patchNotesManager);
			PenitenceManager penitenceManager = new PenitenceManager();
			Core.PenitenceManager = penitenceManager;
			this.AddSystem(penitenceManager);
			SharedCommands sharedCommands = new SharedCommands();
			Core.SharedCommands = sharedCommands;
			this.AddSystem(sharedCommands);
			AlmsManager almsManager = new AlmsManager();
			Core.Alms = almsManager;
			this.AddSystem(almsManager);
			BossRushManager bossRushManager = new BossRushManager();
			Core.BossRushManager = bossRushManager;
			this.AddSystem(bossRushManager);
			DemakeManager demakeManager = new DemakeManager();
			Core.DemakeManager = demakeManager;
			this.AddSystem(demakeManager);
			Core.preinit = true;
		}

		public void Initialize()
		{
			if (Core.ready)
			{
				return;
			}
			if (!Core.preinit)
			{
				this.PreInit();
			}
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].AllPreInitialized();
			}
			for (int j = 0; j < this.systems.Count; j++)
			{
				this.systems[j].AllInitialized();
			}
			Log.Raw("============================================== (Framework Ready)");
			Core.ready = true;
		}

		public void SetDebug(string system, bool value)
		{
			foreach (GameSystem gameSystem in this.systems)
			{
				if (gameSystem.GetType().Name.ToUpper() == system.ToUpper())
				{
					gameSystem.ShowDebug = value;
				}
			}
		}

		public List<string> GetSystemsId()
		{
			return this.systems.ConvertAll<string>((GameSystem x) => x.GetType().Name);
		}

		private void AddSystem(GameSystem system)
		{
			this.systems.Add(system);
			system.Initialize();
		}

		private void ReadParameters()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i] == "-console")
				{
					this.debugMode = true;
				}
			}
		}

		private void Awake()
		{
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].Awake();
			}
		}

		private void Start()
		{
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].Start();
			}
		}

		private void Update()
		{
			if (!Core.ready)
			{
				return;
			}
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].Update();
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < this.systems.Count; i++)
			{
				this.systems[i].Dispose();
			}
		}

		private void OnGUI()
		{
			for (int i = 0; i < this.systems.Count; i++)
			{
				if (this.systems[i].ShowDebug)
				{
					this.systems[i].OnGUI();
				}
			}
		}

		public static LogicManager Logic { get; private set; }

		public static UIManager UI { get; private set; }

		public static InputManager Input { get; private set; }

		public static FMODAudioManager Audio { get; private set; }

		public static ScreenManager Screen { get; private set; }

		public static InventoryManager InventoryManager { get; private set; }

		public static SpawnManager SpawnManager { get; private set; }

		public static PersistentManager Persistence { get; private set; }

		public static LocalizationManager Localization { get; private set; }

		public static EventManager Events { get; private set; }

		public static MetricManager Metrics { get; private set; }

		public static DialogManager Dialog { get; private set; }

		public static Cinematics Cinematics { get; private set; }

		public static ExceptionHandler ExceptionHandler { get; private set; }

		public static SkillManager SkillManager { get; private set; }

		public static MapManager MapManager { get; private set; }

		public static NewMapManager NewMapManager { get; private set; }

		public static LevelManager LevelManager { get; private set; }

		public static CamerasManager CamerasManager { get; private set; }

		public static GuiltManager GuiltManager { get; private set; }

		public static DLCManager DLCManager { get; private set; }

		public static AchievementsManager AchievementsManager { get; private set; }

		public static ColorPaletteManager ColorPaletteManager { get; private set; }

		public static TutorialManager TutorialManager { get; private set; }

		public static ControlRemapManager ControlRemapManager { get; private set; }

		public static GameModeManager GameModeManager { get; private set; }

		public static PatchNotesManager PatchNotesManager { get; private set; }

		public static PenitenceManager PenitenceManager { get; private set; }

		public static SharedCommands SharedCommands { get; private set; }

		public static AlmsManager Alms { get; private set; }

		public static BossRushManager BossRushManager { get; private set; }

		public static DemakeManager DemakeManager { get; private set; }

		public bool debugMode;

		public static bool ready;

		public static bool preinit;

		private List<GameSystem> systems = new List<GameSystem>();

		public delegate void SimpleEvent();

		public delegate void SimpleEventParam(object param);

		public delegate void GenericEvent(Object param);

		public delegate void ObjectEvent(GameObject param);

		public delegate void StringEvent(string param);

		public delegate void EntityEvent(Entity entity);

		public delegate void InteractableEvent(Interactable entity);

		public delegate void RegionEvent(Region entity);
	}
}
