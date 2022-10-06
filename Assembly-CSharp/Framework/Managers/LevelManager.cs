using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.FrameworkCore;
using Framework.Map;
using Framework.Util;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Gameplay.UI.Console;
using Gameplay.UI.Widgets;
using Tools.Level.Effects;
using Tools.Level.Interactables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Framework.Managers
{
	public class LevelManager : GameSystem
	{
		public bool InsideChangeLevel { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LevelManager.EmptyDelegate OnMenuLoaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LevelManager.EmptyDelegate OnGenericsElementsLoaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LevelManager.LevelDelegate OnBeforeLevelLoad;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LevelManager.LevelDelegate OnLevelPreLoaded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LevelManager.LevelDelegate OnLevelLoaded;

		public Level currentLevel { get; private set; }

		public Level lastLevel { get; private set; }

		public Door PendingrDoorToExit { get; set; }

		public Vector3 LastSafePosition { get; private set; }

		public string LastSafeLevel { get; private set; }

		public Vector3 LastSafeGuiltPosition { get; private set; }

		public string LastSafeGuiltLevel { get; private set; }

		public CellKey LastSafeGuiltCellKey { get; private set; }

		public LevelManager.CinematicsChangeLevel InCinematicsChangeLevel { get; private set; }

		public override void AllPreInitialized()
		{
			this.LastSafePosition = Vector3.zero;
			this.LastSafeLevel = string.Empty;
			this.LastSafeGuiltPosition = Vector3.zero;
			this.LastSafeGuiltLevel = string.Empty;
			this.LastSafeGuiltCellKey = null;
			this.currentLevel = null;
			this.lastLevel = null;
			this.preloadLevel = new Level("Preload_" + this.GetPlatformName(), false);
			this.mustWaitToCacheScene = false;
			this.levelEffectsDatabase = (Resources.Load("LevelEffectsDatabase") as ScriptableLevelEffects);
			this.InjectGenericElements();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
				string text = fileNameWithoutExtension.Split(new char[]
				{
					'_'
				})[0];
				if (!this.levels.ContainsKey(text))
				{
					if (!(text == "GenericElements") && !text.StartsWith("Preload_"))
					{
						this.levels[text] = new Level(text, true);
						this.levels[text].Distance = 999;
					}
				}
			}
			Application.backgroundLoadingPriority = 0;
			this.InCinematicsChangeLevel = LevelManager.CinematicsChangeLevel.No;
			this.InsideChangeLevel = false;
		}

		public void ChangeLevel(string levelName, bool startFromEditor = false, bool useFade = true, bool forceDeactivate = false, Color? background = null)
		{
			if (!this.InsideChangeLevel)
			{
				Singleton<Core>.Instance.StartCoroutine(this.ChangeLevelCorrutine(levelName, string.Empty, startFromEditor, useFade, forceDeactivate, background));
			}
			else
			{
				Debug.LogError("Calling a ChangeLevel when a change level is in progress");
			}
		}

		public void ChangeLevelAndPlayEvent(string levelName, string eventName, bool hideplayer = true, bool useFade = true, bool forceDeactivate = false, Color? background = null, bool Miriam = false)
		{
			if (!this.InsideChangeLevel)
			{
				this.InCinematicsChangeLevel = LevelManager.CinematicsChangeLevel.Intro;
				Core.SpawnManager.SetCurrentToCustomSpawnData(Miriam);
				Core.SpawnManager.HidePlayerInNextSpawn = hideplayer;
				Singleton<Core>.Instance.StartCoroutine(this.ChangeLevelCorrutine(levelName, eventName, false, useFade, forceDeactivate, background));
			}
			else
			{
				Debug.LogError("Calling a ChangeLevel when a change level is in progress");
			}
		}

		public void RestoreFromChangeLevelAndPlayEvent(bool useFade = true, Color? background = null)
		{
			this.InCinematicsChangeLevel = LevelManager.CinematicsChangeLevel.Outro;
			Core.SpawnManager.SpawnFromCustom(useFade, background);
		}

		private IEnumerator ChangeLevelCorrutine(string levelName, string eventName, bool startFromEditor = false, bool useFade = true, bool forceDeactivate = false, Color? background = null)
		{
			this.InsideChangeLevel = true;
			if (!startFromEditor && !this.levels.ContainsKey(levelName))
			{
				Debug.LogError("LevelManager: Try to load level '" + levelName + "' that it's not in build");
			}
			else
			{
				Level level = this.levels[levelName];
				if (LevelManager.OnBeforeLevelLoad != null)
				{
					LevelManager.OnBeforeLevelLoad(this.currentLevel, level);
				}
				Core.Persistence.OnBeforeLevelLoad(this.currentLevel, level);
				if (this.currentLevel != null && (level != this.currentLevel || forceDeactivate))
				{
					if (this.currentLevel.CurrentStatus != LevelManager.LevelStatus.Activated)
					{
						Debug.LogError("LevelManager: Load new level and current status is " + this.currentLevel.CurrentStatus);
					}
					else
					{
						yield return this.DeactivateCurrentAndLoadNew(level, useFade, background);
					}
				}
				else
				{
					yield return this.LoadAndActivateLevel(level, useFade, background);
				}
				if (eventName != string.Empty)
				{
					PlayMakerFSM.BroadcastEvent(eventName);
				}
			}
			this.InsideChangeLevel = false;
			yield break;
		}

		public void ActivatePrecachedScene()
		{
			this.mustWaitToCacheScene = true;
		}

		public List<string> DEBUG_GetAllLevelsName()
		{
			return new List<string>(this.levels.Keys);
		}

		public string GetLastSceneName()
		{
			return (this.lastLevel == null) ? string.Empty : this.lastLevel.LevelName;
		}

		public LevelColorEffectData GetLevelEffects(LEVEL_COLOR_CONFIGS configType)
		{
			LevelColorEffectData result = default(LevelColorEffectData);
			if (this.levelEffectsDatabase.levelColorConfigs.TryGetValue(configType, out result))
			{
				return result;
			}
			this.levelEffectsDatabase.levelColorConfigs.TryGetValue(LEVEL_COLOR_CONFIGS.NONE, out result);
			return result;
		}

		public void SetPlayerSafePosition(Vector3 safePosition)
		{
			this.LastSafePosition = safePosition;
			this.LastSafeLevel = this.currentLevel.LevelName;
			if (Core.Logic.CurrentLevelConfig.UseDefaultGuiltSystem)
			{
				this.LastSafeGuiltPosition = safePosition;
				this.LastSafeGuiltLevel = this.currentLevel.LevelName;
				this.LastSafeGuiltCellKey = Core.NewMapManager.GetPlayerCell();
			}
			else if (Core.Logic.CurrentLevelConfig.OverrideGuiltPosition)
			{
				this.LastSafeGuiltPosition = Core.Logic.CurrentLevelConfig.guiltPositionOverrider.position;
				this.LastSafeGuiltLevel = this.currentLevel.LevelName;
				this.LastSafeGuiltCellKey = Core.NewMapManager.GetCellKeyFromPosition(this.LastSafeGuiltLevel, this.LastSafeGuiltPosition);
			}
		}

		private string GetPlatformName()
		{
			string result = Application.platform.ToString();
			RuntimePlatform platform = Application.platform;
			switch (platform)
			{
			case 0:
			case 1:
				result = "OSX";
				break;
			case 2:
			case 7:
				result = "Windows";
				break;
			default:
				switch (platform)
				{
				case 13:
				case 16:
					result = "Linux";
					break;
				}
				break;
			}
			return result;
		}

		private IEnumerator WaitForPreloadCacheScene()
		{
			while (this.preloadLevel.CurrentStatus != LevelManager.LevelStatus.Activated)
			{
				LevelManager.LevelStatus currentStatus = this.preloadLevel.CurrentStatus;
				switch (currentStatus)
				{
				case LevelManager.LevelStatus.Unloaded:
					yield return this.preloadLevel.Load(false);
					continue;
				default:
					if (currentStatus != LevelManager.LevelStatus.Deactivated)
					{
						yield return null;
						continue;
					}
					break;
				case LevelManager.LevelStatus.Loaded:
					break;
				}
				yield return this.preloadLevel.Activate();
			}
			yield break;
		}

		private IEnumerator DeactivateCurrentAndLoadNew(Level level, bool useFade, Color? background = null)
		{
			UIController.instance.ShowLoad(true, background);
			yield return new WaitForEndOfFrame();
			yield return this.currentLevel.DeActivate();
			yield return this.LoadAndActivateLevel(level, useFade, background);
			yield break;
		}

		private IEnumerator LoadAndActivateLevel(Level level, bool useFade, Color? background = null)
		{
			Debug.Log("---> LevelManager:  Load and activate level " + level.LevelName);
			yield return new WaitForEndOfFrame();
			this.ClearOldLevelParams();
			Core.Logic.SetState(LogicStates.Unresponsive);
			this.lastLevel = this.currentLevel;
			this.currentLevel = level;
			this.currentLevel.Distance = 0;
			if (this.lastLevel != null)
			{
				this.lastZoneName = this.lastLevel.LevelName.Substring(0, 6);
			}
			if (this.mustWaitToCacheScene)
			{
				yield return this.WaitForPreloadCacheScene();
			}
			Core.UI.ShowGamePlayUI = true;
			while (level.CurrentStatus != LevelManager.LevelStatus.Activated)
			{
				LevelManager.LevelStatus currentStatus = level.CurrentStatus;
				switch (currentStatus)
				{
				case LevelManager.LevelStatus.Unloaded:
					yield return level.Load(false);
					continue;
				default:
					if (currentStatus != LevelManager.LevelStatus.Deactivated)
					{
						yield return null;
						continue;
					}
					break;
				case LevelManager.LevelStatus.Loaded:
					break;
				}
				yield return level.Activate();
			}
			this.InitializeLevel();
			if (!level.LevelName.Equals("D07Z01S04") && !level.LevelName.StartsWith("D19Z01"))
			{
				Singleton<Core>.Instance.StartCoroutine(this.StreamingLevels());
				Core.Input.SetBlocker("BLOCK_UNTIL_FPS_STABLE", true);
				float timeWaiting = 0f;
				while (1f / Time.smoothDeltaTime < 50f && timeWaiting < 3f)
				{
					timeWaiting += Time.smoothDeltaTime;
					yield return null;
				}
				Core.Input.SetBlocker("BLOCK_UNTIL_FPS_STABLE", false);
			}
			Penitent player = Core.Logic.Penitent;
			player.PlatformCharacterController.PlatformCharacterPhysics.GravityScale = 0f;
			for (int i = 0; i < 5; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			float groundDist = player.PlatformCharacterController.GroundDist;
			Vector3 spawnPosition = new Vector3(player.transform.position.x, player.transform.position.y - groundDist, player.transform.position.z);
			player.transform.position = spawnPosition;
			player.Animator.Play("Idle");
			player.PlatformCharacterController.PlatformCharacterPhysics.GravityScale = 3f;
			this.UpdateNewCameraParams();
			Core.Logic.SetState(LogicStates.Playing);
			UIController.instance.ShowLoad(false, background);
			float time = (!useFade) ? 0f : 0.6f;
			Color fadecolor = (background != null) ? background.Value : Color.black;
			FadeWidget.instance.StartEasyFade(fadecolor, new Color(0f, 0f, 0f, 0f), time, false);
			Core.Input.ResetManager();
			Core.Persistence.OnLevelLoaded(this.lastLevel, this.currentLevel);
			if (LevelManager.OnLevelLoaded != null)
			{
				LevelManager.OnLevelLoaded(this.lastLevel, this.currentLevel);
			}
			if (this.InCinematicsChangeLevel == LevelManager.CinematicsChangeLevel.Outro)
			{
				this.InCinematicsChangeLevel = LevelManager.CinematicsChangeLevel.No;
			}
			if (this.PendingrDoorToExit != null)
			{
				this.PendingrDoorToExit.ExitFromThisDoor();
				this.PendingrDoorToExit = null;
			}
			yield break;
		}

		private void ClearOldLevelParams()
		{
			ProCamera2DNumericBoundaries proCamera2DNumericBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries;
			proCamera2DNumericBoundaries.UseNumericBoundaries = false;
			proCamera2DNumericBoundaries.UseTopBoundary = false;
			proCamera2DNumericBoundaries.UseBottomBoundary = false;
			proCamera2DNumericBoundaries.UseLeftBoundary = false;
			proCamera2DNumericBoundaries.UseRightBoundary = false;
			Core.Logic.CameraManager.ProCamera2D.FollowVertical = true;
			Core.Logic.CameraManager.ProCamera2D.FollowHorizontal = true;
		}

		private void UpdateNewCameraParams()
		{
			Core.Logic.CameraManager.UpdateNewCameraParams();
			Core.Logic.CameraManager.CameraPlayerOffset.UpdateNewParams();
			CameraNumericBoundaries[] array = Object.FindObjectsOfType<CameraNumericBoundaries>();
			if (array.Length > 0)
			{
				bool flag = false;
				foreach (CameraNumericBoundaries cameraNumericBoundaries in array)
				{
					if (!cameraNumericBoundaries.notSetOnLevelLoad)
					{
						if (flag)
						{
							Debug.LogWarning("UpdateNewCameraParams " + array.Length.ToString() + " CameraNumericBoundaries found, only first applied");
							break;
						}
						flag = true;
						cameraNumericBoundaries.SetBoundariesOnLevelLoad();
					}
				}
			}
		}

		private void InitializeLevel()
		{
			SceneManager.SetActiveScene(this.currentLevel.GetLogicScene().Scene);
			if (LevelManager.OnLevelPreLoaded != null)
			{
				LevelManager.OnLevelPreLoaded(this.lastLevel, this.currentLevel);
			}
			if (Core.Logic.DebugExecutionEnabled)
			{
				ExecutionCommand.EnableDebugExecution(true);
			}
			Core.Logic.EnemySpawner.SpawnEnemiesOnLoad();
			Core.SpawnManager.SpawnPlayerOnLevelLoad(true);
		}

		private IEnumerator StreamingLevels()
		{
			List<Level> allLevels = new List<Level>(this.levels.Values);
			foreach (Level level in allLevels)
			{
				if (level.IsBundle && level.CurrentStatus == LevelManager.LevelStatus.Unloaded && this.IsInSameZone(level))
				{
					yield return level.Load(true);
				}
				else if (level.IsBundle && (level.CurrentStatus == LevelManager.LevelStatus.Loaded || level.CurrentStatus == LevelManager.LevelStatus.Deactivated) && !this.IsInSameZone(level) && !this.IsInLastZone(level))
				{
					yield return level.UnLoad();
				}
			}
			yield break;
		}

		private bool IsInSameZone(Level level)
		{
			string value = this.currentLevel.LevelName.Substring(0, 6);
			return level.LevelName.StartsWith(value);
		}

		private bool IsInLastZone(Level level)
		{
			return level.LevelName.StartsWith(this.lastZoneName);
		}

		private void OnBaseSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "GenericElements")
			{
				SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(this.OnBaseSceneLoaded);
				if (LevelManager.OnGenericsElementsLoaded != null)
				{
					LevelManager.OnGenericsElementsLoaded();
				}
			}
		}

		private void InjectGenericElements()
		{
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnBaseSceneLoaded);
			bool flag = true;
			if (flag)
			{
				SceneManager.LoadScene("GenericElements", 1);
			}
		}

		public const string GENERIC_ELEMENTS_SCENE_NAME = "GenericElements";

		public const string PRELOAD_PLATFORM_PREFIX = "Preload_";

		private bool mustWaitToCacheScene;

		private Level preloadLevel;

		private const float SCENE_FADE_DELAY = 0.25f;

		private const int MAX_DISTANCE = 999;

		private const int STREAMING_MAX_DISTANCE = 4;

		private const int STREAMING_DISTANCE_TO_LOAD = 2;

		private string lastZoneName = string.Empty;

		private Dictionary<string, Level> levels = new Dictionary<string, Level>();

		private ScriptableLevelEffects levelEffectsDatabase;

		public enum LevelStatus
		{
			Unloaded,
			Unloading,
			Loaded,
			Loading,
			Activated,
			Activating,
			Deactivated,
			Deactivating
		}

		public delegate void EmptyDelegate();

		public delegate void SceneDelegate(Scene unityScene);

		public delegate void LevelDelegate(Level oldLevel, Level newLevel);

		public enum CinematicsChangeLevel
		{
			No,
			Intro,
			Outro
		}
	}
}
