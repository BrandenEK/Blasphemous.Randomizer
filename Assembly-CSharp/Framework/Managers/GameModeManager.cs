using System;
using System.Collections.Generic;
using Framework.EditorScripts.BossesBalance;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore;
using UnityEngine;

namespace Framework.Managers
{
	public class GameModeManager : GameSystem, PersistentInterface
	{
		public override void Initialize()
		{
			this.currentMode = GameModeManager.GAME_MODES.MENU;
			this.newGameEnemiesBalanceChart = Resources.Load<EnemiesBalanceChart>("Enemies Balance Charts/EnemiesBalanceChartNewGame");
			this.newGamePlusEnemiesBalanceChart = Resources.Load<EnemiesBalanceChart>("Enemies Balance Charts/EnemiesBalanceChartNewGamePlus");
			this.newGameBossesBalanceChart = Resources.Load<BossesBalanceChart>("Bosses Balance Charts/BossesBalanceChartNewGame");
			this.newGamePlusBossesBalanceChart = Resources.Load<BossesBalanceChart>("Bosses Balance Charts/BossesBalanceChartNewGamePlus");
			this.bossRushNormalBalanceChart = Resources.Load<BossesBalanceChart>("Bosses Balance Charts/BossesBalanceChartBossRushNormal");
			this.bossRushHardBalanceChart = Resources.Load<BossesBalanceChart>("Bosses Balance Charts/BossesBalanceChartBossRushHard");
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public static List<string> GetAllGameModesNames()
		{
			return new List<string>(Enum.GetNames(typeof(GameModeManager.GAME_MODES)));
		}

		public GameModeManager.GAME_MODES GetCurrentGameMode()
		{
			return this.currentMode;
		}

		public string GetCurrentGameModeName()
		{
			return Enum.GetName(typeof(GameModeManager.GAME_MODES), this.currentMode);
		}

		public bool CheckGameModeActive(string mode)
		{
			return this.GameModeExists(mode) && this.currentMode == (GameModeManager.GAME_MODES)Enum.Parse(typeof(GameModeManager.GAME_MODES), mode, true);
		}

		public bool GameModeExists(string mode)
		{
			using (List<string>.Enumerator enumerator = GameModeManager.GetAllGameModesNames().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Equals(mode.ToUpperInvariant()))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsCurrentMode(GameModeManager.GAME_MODES mode)
		{
			return this.currentMode == mode;
		}

		public void ChangeMode(string newMode)
		{
			if (!this.GameModeExists(newMode))
			{
				Debug.Log("GameModeManager:: ChangeMode: A Game Mode with name: " + newMode + " doesn't exist!");
				return;
			}
			this.ChangeMode((GameModeManager.GAME_MODES)Enum.Parse(typeof(GameModeManager.GAME_MODES), newMode, true));
		}

		public void ChangeMode(GameModeManager.GAME_MODES newMode)
		{
			if (newMode == this.currentMode)
			{
				Debug.Log("GameModeManager:: ChangeMode: current game mode and new game mode are the same! Current game mode: " + this.currentMode);
				return;
			}
			GameModeManager.GAME_MODES game_MODES = this.currentMode;
			Debug.Log(string.Concat(new object[]
			{
				"GameModeManager:: ChangeMode: changing current mode to ",
				newMode,
				"! Old mode: ",
				game_MODES
			}));
			this.ExitMode(this.currentMode);
			this.currentMode = newMode;
			this.EnterMode(this.currentMode);
		}

		public int GetNewGamePlusUpgrades()
		{
			return this.NewGamePlusUpgrades;
		}

		public EnemiesBalanceChart GetCurrentEnemiesBalanceChart()
		{
			if (!Core.Randomizer.gameConfig.hardMode)
			{
				return this.newGameEnemiesBalanceChart;
			}
			return this.newGamePlusEnemiesBalanceChart;
		}

		public BossesBalanceChart GetCurrentBossesBalanceChart()
		{
			if (this.currentMode != GameModeManager.GAME_MODES.BOSS_RUSH)
			{
				if (!Core.Randomizer.gameConfig.hardMode)
				{
					return this.newGameBossesBalanceChart;
				}
				return this.newGamePlusBossesBalanceChart;
			}
			else
			{
				if (Core.BossRushManager.GetCurrentBossRushMode() == BossRushManager.BossRushCourseMode.NORMAL)
				{
					return this.bossRushNormalBalanceChart;
				}
				return this.bossRushHardBalanceChart;
			}
		}

		public bool CanConvertToNewGamePlus(PersistentManager.SnapShot snapshot)
		{
			return snapshot.sceneElements.ContainsKey("d07z01s03");
		}

		public void ConvertCurrentGameToPlus()
		{
			this.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
			this.NewGamePlusUpgrades++;
			Core.SpawnManager.PrepareForNewGamePlus("D17Z01S01");
			Core.Persistence.PrepareForNewGamePlus();
			Core.PenitenceManager.DeactivateCurrentPenitence();
			Core.Alms.ResetPersistence();
			Core.InventoryManager.PrepareForNewGamePlus();
			Core.GuiltManager.ResetGuilt(false);
			Core.Logic.Penitent.Stats.Purge.Current = 0f;
			Core.Logic.Penitent.Stats.Flask.SetPermanentBonus(0f);
			Core.Logic.Penitent.Stats.FlaskHealth.SetPermanentBonus(0f);
			Core.Logic.Penitent.Stats.BeadSlots.SetPermanentBonus(0f);
			Core.Logic.Penitent.Stats.Fervour.SetPermanentBonus(0f);
			Core.Logic.Penitent.Stats.Life.SetPermanentBonus(0f);
			Core.Events.PrepareForNewGamePlus();
			Core.NewMapManager.ResetPersistence();
			Core.AchievementsManager.PrepareForNewGamePlus();
		}

		public bool ShouldProgressAchievements()
		{
			return this.currentMode == GameModeManager.GAME_MODES.NEW_GAME || this.currentMode == GameModeManager.GAME_MODES.NEW_GAME_PLUS;
		}

		private void EnterMode(GameModeManager.GAME_MODES m)
		{
			switch (m)
			{
			case GameModeManager.GAME_MODES.MENU:
				this.OnEnterMenu();
				return;
			case GameModeManager.GAME_MODES.NEW_GAME:
				this.OnEnterNewGame();
				return;
			case GameModeManager.GAME_MODES.NEW_GAME_PLUS:
				this.OnEnterNewGamePlus();
				return;
			case GameModeManager.GAME_MODES.BOSS_RUSH:
				break;
			case GameModeManager.GAME_MODES.DEMAKE:
				this.OnEnterDemake();
				break;
			default:
				return;
			}
		}

		private void OnEnterMenu()
		{
			if (this.OnEnterMenuMode != null)
			{
				this.OnEnterMenuMode();
			}
		}

		private void OnEnterNewGame()
		{
			if (this.OnEnterNewGameMode != null)
			{
				this.OnEnterNewGameMode();
			}
		}

		private void OnEnterNewGamePlus()
		{
			if (this.OnEnterNewGamePlusMode != null)
			{
				this.OnEnterNewGamePlusMode();
			}
		}

		private void OnEnterDemake()
		{
			if (this.OnEnterDemakeMode != null)
			{
				this.OnEnterDemakeMode();
			}
		}

		private void ExitMode(GameModeManager.GAME_MODES m)
		{
			switch (m)
			{
			case GameModeManager.GAME_MODES.MENU:
				this.OnExitMenu();
				return;
			case GameModeManager.GAME_MODES.NEW_GAME:
				this.OnExitNewGame();
				return;
			case GameModeManager.GAME_MODES.NEW_GAME_PLUS:
				this.OnExitNewGamePlus();
				return;
			case GameModeManager.GAME_MODES.BOSS_RUSH:
				break;
			case GameModeManager.GAME_MODES.DEMAKE:
				this.OnExitDemake();
				break;
			default:
				return;
			}
		}

		private void OnExitMenu()
		{
			if (this.OnExitMenuMode != null)
			{
				this.OnExitMenuMode();
			}
		}

		private void OnExitNewGame()
		{
			if (this.OnExitNewGameMode != null)
			{
				this.OnExitNewGameMode();
			}
		}

		private void OnExitNewGamePlus()
		{
			if (this.OnExitNewGamePlusMode != null)
			{
				this.OnExitNewGamePlusMode();
			}
		}

		private void OnExitDemake()
		{
			if (this.OnExitDemakeMode != null)
			{
				this.OnExitDemakeMode();
			}
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_GAMEMMODE";
		}

		public void ResetPersistence()
		{
			this.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME);
			this.NewGamePlusUpgrades = 0;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new GameModeManager.GameModePersistenceData
			{
				gameMode = this.currentMode,
				NewGamePlusUpgrades = this.NewGamePlusUpgrades
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			GameModeManager.GameModePersistenceData gameModePersistenceData = (GameModeManager.GameModePersistenceData)data;
			this.NewGamePlusUpgrades = gameModePersistenceData.NewGamePlusUpgrades;
			this.ChangeMode(gameModePersistenceData.gameMode);
		}

		public Core.SimpleEvent OnEnterMenuMode;

		public Core.SimpleEvent OnEnterNewGameMode;

		public Core.SimpleEvent OnEnterNewGamePlusMode;

		public Core.SimpleEvent OnEnterDemakeMode;

		public Core.SimpleEvent OnExitMenuMode;

		public Core.SimpleEvent OnExitNewGameMode;

		public Core.SimpleEvent OnExitNewGamePlusMode;

		public Core.SimpleEvent OnExitDemakeMode;

		public const string SCENE_GAMEPLUS = "d07z01s03";

		private const string NEW_GAME_BALANCE_CHART_PATH = "Enemies Balance Charts/EnemiesBalanceChartNewGame";

		private const string NEW_GAME_PLUS_BALANCE_CHART_PATH = "Enemies Balance Charts/EnemiesBalanceChartNewGamePlus";

		private const string NEW_GAME_BOSS_CHART = "Bosses Balance Charts/BossesBalanceChartNewGame";

		private const string NEW_GAME_PLUS_BOSS_CHART = "Bosses Balance Charts/BossesBalanceChartNewGamePlus";

		private const string BOSS_RUSH_NORMAL_BOSS_CHART = "Bosses Balance Charts/BossesBalanceChartBossRushNormal";

		private const string BOSS_RUSH_HARD_BOSS_CHART = "Bosses Balance Charts/BossesBalanceChartBossRushHard";

		private EnemiesBalanceChart newGameEnemiesBalanceChart;

		private EnemiesBalanceChart newGamePlusEnemiesBalanceChart;

		private BossesBalanceChart newGameBossesBalanceChart;

		private BossesBalanceChart newGamePlusBossesBalanceChart;

		private BossesBalanceChart bossRushNormalBalanceChart;

		private BossesBalanceChart bossRushHardBalanceChart;

		private GameModeManager.GAME_MODES currentMode;

		private int NewGamePlusUpgrades;

		private const string PERSITENT_ID = "ID_GAMEMMODE";

		[Serializable]
		public enum GAME_MODES
		{
			MENU,
			NEW_GAME,
			NEW_GAME_PLUS,
			BOSS_RUSH,
			DEMAKE
		}

		[Serializable]
		public class GameModePersistenceData : PersistentManager.PersistentData
		{
			public GameModePersistenceData() : base("ID_GAMEMMODE")
			{
			}

			public GameModeManager.GAME_MODES gameMode = GameModeManager.GAME_MODES.NEW_GAME;

			public int NewGamePlusUpgrades;
		}
	}
}
