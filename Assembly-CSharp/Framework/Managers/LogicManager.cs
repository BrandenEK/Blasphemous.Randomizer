using System;
using System.Collections;
using System.Collections.ObjectModel;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes.Logic;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.NPCs.BloodDecals;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.Breakable;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Spawn;
using Gameplay.UI;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Managers
{
	public class LogicManager : GameSystem
	{
		public LogicStates CurrentState { get; private set; }

		public bool DebugExecutionEnabled { get; set; }

		public ScreenFreezeManager ScreenFreeze
		{
			get
			{
				return Object.FindObjectOfType<ScreenFreezeManager>();
			}
		}

		public CameraShakeManager CameraShakeManager { get; private set; }

		public EnemySpawner EnemySpawner { get; private set; }

		public ExecutionController ExecutionController { get; private set; }

		public BreakableManager BreakableManager { get; set; }

		public void SetState(LogicStates newState)
		{
			if (newState == this.CurrentState)
			{
				return;
			}
			this.previousState = this.CurrentState;
			this.CurrentState = newState;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(this.CurrentState);
			}
			Log.Trace("Logic", "Game state has been set to " + newState.ToString().ToUpper(), null);
		}

		public void SetPreviousState()
		{
			this.SetState(this.previousState);
		}

		public override void Initialize()
		{
			this.PermaBloodStore = new PermaBloodStorage();
			this.EnemySpawner = new EnemySpawner();
			this.BreakableManager = new BreakableManager();
			this.PenitentSpawner = new PenitentSpawner();
			this.ExecutionController = new ExecutionController();
			this.CameraShakeManager = new CameraShakeManager();
			this.FXVolumen = -1f;
			this.PlayerCurrentLife = -1f;
			this.IsPaused = false;
		}

		public LevelInitializer CurrentLevelConfig { get; private set; }

		public int CurrentLevelDifficult { get; set; }

		public int CurrentLevelNumber { get; set; }

		public bool IsMenuScene()
		{
			string a = string.Empty;
			if (Core.LevelManager.currentLevel != null)
			{
				a = Core.LevelManager.currentLevel.LevelName;
			}
			return a == "MainMenu";
		}

		public bool IsAttrackScene()
		{
			string name = SceneManager.GetActiveScene().name;
			return name == "AttrackMode";
		}

		public void LoadAttrackScene()
		{
			if (UIController.instance != null)
			{
				UIController.instance.HideBossHealth();
			}
			Core.LevelManager.ChangeLevel("AttrackMode", false, true, false, null);
		}

		public void LoadMenuScene(bool useFade = true)
		{
			if (LogicManager.GoToMainMenu != null)
			{
				LogicManager.GoToMainMenu();
			}
			if (UIController.instance != null)
			{
				UIController.instance.HideBossHealth();
			}
			LevelManager levelManager = Core.LevelManager;
			string levelName = "MainMenu";
			levelManager.ChangeLevel(levelName, false, useFade, false, null);
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.MENU);
		}

		public void LoadCreditsScene(bool useFade = true)
		{
			if (UIController.instance != null)
			{
				UIController.instance.HideBossHealth();
			}
			LevelManager levelManager = Core.LevelManager;
			string levelName = "D07Z01S04";
			levelManager.ChangeLevel(levelName, false, useFade, false, null);
		}

		public void ResetAllData()
		{
			Core.Persistence.ResetAll();
			Core.Logic.EnemySpawner.Reset();
			Core.Logic.BreakableManager.Reset();
		}

		public void SetCurrentLevelConfig(LevelInitializer newLevel)
		{
			this.CurrentLevelConfig = newLevel;
		}

		public void UsePrieDieu()
		{
			if (this.OnUsePrieDieu != null)
			{
				this.OnUsePrieDieu();
			}
		}

		public Penitent Penitent { get; set; }

		public PenitentSpawner PenitentSpawner { get; set; }

		public float PlayerCurrentLife { get; set; }

		public void ResetPlayer()
		{
		}

		public CameraManager CameraManager
		{
			get
			{
				return CameraManager.Instance;
			}
		}

		public PermaBloodStorage PermaBloodStore { get; private set; }

		public float FXVolumen { get; set; }

		public override void OnGUI()
		{
			base.DebugResetLine();
			if (this.Penitent == null)
			{
				return;
			}
			base.DebugDrawTextLine("******    Stats", 10, 1500);
			string format = "{0,-25} {1,10} {2,10} {3,10} {4,10}";
			base.DebugDrawTextLine(string.Format(format, new object[]
			{
				"Variable",
				"Base",
				"Current",
				"Bonus",
				"Final"
			}), 10, 1500);
			base.DebugDrawTextLine("------------------------------------------------------------------------------", 10, 1500);
			EntityStats stats = this.Penitent.Stats;
			string format2 = "{0,-25} {1,10:F2} {2,10:F2} {3,10:F2} {4,10:F2}";
			Array values = Enum.GetValues(typeof(EntityStats.StatsTypes));
			Array.Sort(values);
			IEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					EntityStats.StatsTypes nameType = (EntityStats.StatsTypes)obj;
					Framework.FrameworkCore.Attributes.Logic.Attribute byType = stats.GetByType(nameType);
					float num = 0f;
					if (byType.IsVariable())
					{
						VariableAttribute variableAttribute = (VariableAttribute)byType;
						num = variableAttribute.Current;
					}
					base.DebugDrawTextLine(string.Format(format2, new object[]
					{
						nameType.ToString(),
						this.MaxFloat(byType.Base),
						num,
						byType.Bonus,
						this.MaxFloat(byType.Final)
					}), 10, 1500);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			base.DebugResetLine();
			base.DebugDrawTextLine("******    Bonus", 600, 1500);
			IEnumerator enumerator2 = values.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj2 = enumerator2.Current;
					EntityStats.StatsTypes nameType2 = (EntityStats.StatsTypes)obj2;
					Framework.FrameworkCore.Attributes.Logic.Attribute byType2 = stats.GetByType(nameType2);
					ReadOnlyCollection<RawBonus> rawBonus = byType2.GetRawBonus();
					if (rawBonus.Count > 0 || byType2.PermanetBonus > 0f)
					{
						base.DebugDrawTextLine(nameType2.ToString() + " stat, permanet " + byType2.PermanetBonus.ToString(), 600, 1500);
						foreach (RawBonus rawBonus2 in rawBonus)
						{
							base.DebugDrawTextLine("...Base:" + rawBonus2.Base.ToString() + "  Multyplier:" + rawBonus2.Multiplier.ToString(), 600, 1500);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}

		private float MaxFloat(float value)
		{
			float num = 999999f;
			return (value <= num) ? value : num;
		}

		public bool IsPaused { get; private set; }

		public void PauseGame()
		{
			if (this.IsPaused)
			{
				return;
			}
			this.IsPaused = true;
			this.currentScale = 1f;
			this.CurrentLevelConfig.TimeScale = 0f;
			RuntimeManager.GetBus("bus:/ALLSFX/SFX").setPaused(true);
		}

		public void ResumeGame()
		{
			if (!this.IsPaused)
			{
				return;
			}
			this.IsPaused = false;
			RuntimeManager.GetBus("bus:/ALLSFX/SFX").setPaused(false);
			this.CurrentLevelConfig.TimeScale = this.currentScale;
			DOTween.defaultTimeScaleIndependent = false;
		}

		public bool IsSlowMode()
		{
			return Time.timeScale != 1f;
		}

		public static Core.SimpleEvent GoToMainMenu;

		public Core.SimpleEvent OnUsePrieDieu;

		public LogicManager.StateEvent OnStateChanged;

		private LogicStates previousState;

		public const string MENU_LEVEL_NAME = "MainMenu";

		private const string ATTRACK_NAME = "AttrackMode";

		public const string CREDITS_LEVEL_NAME = "D07Z01S04";

		public const string ARENA_LEVELS_PREFIX = "D19Z01";

		public const string LAKE_LEVEL_PREFIX = "D13Z01";

		public const string PAINTING_LEVEL_PREFIX = "D18Z01";

		public const string HW_LEVEL_PREFIX = "D24Z01";

		private float currentScale;

		public delegate void StateEvent(LogicStates state);
	}
}
