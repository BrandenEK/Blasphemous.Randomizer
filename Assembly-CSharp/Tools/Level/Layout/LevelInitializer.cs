using System;
using System.Collections;
using System.Collections.Generic;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Entity.BlobShadow;
using Gameplay.GameControllers.Environment;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Sirenix.OdinInspector;
using Tools.Level.Effects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.Level.Layout
{
	[DefaultExecutionOrder(-1)]
	public class LevelInitializer : MonoBehaviour
	{
		private bool ShowPositionOverrider()
		{
			return this.GuiltConfiguration == LevelInitializer.GuiltConfigurationEnum.OverridePosition;
		}

		public bool OverrideGuiltPosition
		{
			get
			{
				return this.GuiltConfiguration == LevelInitializer.GuiltConfigurationEnum.OverridePosition && this.guiltPositionOverrider;
			}
		}

		public bool UseDefaultGuiltSystem
		{
			get
			{
				return this.GuiltConfiguration == LevelInitializer.GuiltConfigurationEnum.Default || (this.GuiltConfiguration == LevelInitializer.GuiltConfigurationEnum.OverridePosition && !this.guiltPositionOverrider);
			}
		}

		public bool LevelDebug { get; private set; }

		public bool IsSleeping { get; set; }

		public LevelEffectsStore LevelEffectsStore { get; private set; }

		public BlobShadowManager BlobShadowManager
		{
			get
			{
				return this.blobShadowManager;
			}
		}

		public EnemyStatsImporter EnemyStatsImporter
		{
			get
			{
				return this.enemyStatsImporter;
			}
		}

		public float TimeScale
		{
			get
			{
				return this.TimeScaleReal;
			}
			set
			{
				if (Math.Abs(value - this.TimeScaleReal) > Mathf.Epsilon)
				{
					this.TimeScaleReal = value;
				}
			}
		}

		private void Awake()
		{
			if (!LevelInitializer.IsInitilizated)
			{
				if (!Core.ready)
				{
					Debug.Log("==============  LevelInitializer");
					Singleton<Core>.Instance.Initialize();
				}
				else
				{
					LevelInitializer.IsInitilizated = true;
				}
			}
			this.LevelEffectsStore = base.GetComponentInChildren<LevelEffectsStore>();
			this.LevelDebug = !Core.Events.GetFlag("EXECUTED_FROM_MAINMENU");
			this.IsSleeping = false;
			Core.Logic.SetCurrentLevelConfig(this);
			if (this.useLevelEffectsDatabase)
			{
				LevelColorEffectData levelEffects = Core.LevelManager.GetLevelEffects(this.levelEffectsTemplate);
				this.SetLevelEffectsData(levelEffects);
			}
			this.SetEnemyStats();
		}

		private void SetLevelEffectsData(LevelColorEffectData levelEffectsData)
		{
			this.colorizeAmount = levelEffectsData.colorizeAmount;
			this.colorizeColor = levelEffectsData.colorizeColor;
			this.colorizeMultColor = levelEffectsData.colorizeMultColor;
		}

		private void SetLevelScreenEffect(ScreenMaterialEffectsManager.SCREEN_EFFECTS e)
		{
			Core.Logic.CameraManager.ScreenEffectsManager.SetEffect(e);
		}

		private void Start()
		{
			this.ApplyBackgroundColor();
			this.SetLevelScreenEffect(this.screenEffect);
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			if (!LevelInitializer.IsInitilizated)
			{
				string text = SceneManager.GetActiveScene().name;
				text = text.Split(new char[]
				{
					'_'
				})[0];
				Core.SpawnManager.PrepareForSpawnFromMenu();
				Core.LevelManager.ChangeLevel(text, true, true, false, null);
				LevelInitializer.IsInitilizated = true;
			}
		}

		private void SetEnemyStats()
		{
			List<EnemyBalanceItem> enemiesBalanceItems = Core.GameModeManager.GetCurrentEnemiesBalanceChart().EnemiesBalanceItems;
			if (enemiesBalanceItems.Count > 0)
			{
				this.enemyStatsImporter = new EnemyStatsImporter(enemiesBalanceItems);
			}
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			this.ColorizePlayer();
			this.ApplyReflectionConfig();
		}

		private void ColorizePlayer()
		{
			MasterShaderEffects componentInChildren = Core.Logic.Penitent.GetComponentInChildren<MasterShaderEffects>();
			if (componentInChildren == null)
			{
				return;
			}
			if (Math.Abs(this.colorizeAmount) < Mathf.Epsilon)
			{
				componentInChildren.DeactivateColorize();
			}
			else
			{
				componentInChildren.SetColorizeData(this.colorizeColor, this.colorizeMultColor, this.colorizeAmount);
			}
		}

		public void ApplyLevelColorEffects(MasterShaderEffects effects)
		{
			effects.SetColorizeData(this.colorizeColor, this.colorizeMultColor, this.colorizeAmount);
		}

		private void Update()
		{
			Time.timeScale = this.TimeScaleReal;
		}

		private void ApplyBackgroundColor()
		{
			if (Camera.main == null)
			{
				Debug.LogWarning("Imposible to change background color. Camera not found.");
			}
			else
			{
				Camera.main.backgroundColor = this.EditorBackgroundColor;
			}
		}

		private void ApplyReflectionConfig()
		{
			PIDI_2DReflection reflections = Core.Logic.Penitent.reflections;
			if (reflections == null)
			{
				return;
			}
			if (this.hasReflections)
			{
				reflections.enabled = true;
				reflections.SetLocalReflectionConfig(this.localReflectionConfig);
			}
			else
			{
				reflections.enabled = false;
			}
		}

		private IEnumerator SleepTimeCoroutine(float sleepTimeLapse)
		{
			this.TimeScale = 0.1f;
			yield return new WaitForSecondsRealtime(sleepTimeLapse);
			this.IsSleeping = false;
			this.sleepTime = 0f;
			if (!UIController.instance.Paused)
			{
				this.TimeScale = 1f;
			}
			yield break;
		}

		public void SleepTime()
		{
			if (!this.IsSleeping && !UIController.instance.Paused)
			{
				this.IsSleeping = true;
				base.StartCoroutine(this.SleepTimeCoroutine(this.sleepTime));
			}
		}

		public bool ShowZoneTitle(Level oldLevel)
		{
			bool flag = false;
			if (oldLevel != null)
			{
				flag = this.ignoreTitleIfComingFromScenes.Contains(oldLevel.LevelName);
			}
			return this.DisplayZoneTitle && !flag;
		}

		public const float SLEEP_TIMESCALE = 0.1f;

		[BoxGroup("Designer Settings", true, false, 0)]
		[SerializeField]
		private Color EditorBackgroundColor;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		private BlobShadowManager blobShadowManager;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public Color levelShadowColor;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public bool hideShadows;

		[BoxGroup("Color Settings", true, false, 0)]
		[SerializeField]
		public bool useLevelEffectsDatabase;

		[ShowIf("useLevelEffectsDatabase", true)]
		[BoxGroup("Color Settings", true, false, 0)]
		[SerializeField]
		public LEVEL_COLOR_CONFIGS levelEffectsTemplate;

		[BoxGroup("Color Settings", true, false, 0)]
		[SerializeField]
		public Color colorizeColor;

		[BoxGroup("Color Settings", true, false, 0)]
		[SerializeField]
		[Range(0f, 1f)]
		public float colorizeAmount;

		[BoxGroup("Color Settings", true, false, 0)]
		[SerializeField]
		public Color colorizeMultColor = Color.white;

		[BoxGroup("Advanced Settings", true, false, 0)]
		public LocalReflectionConfig localReflectionConfig;

		[BoxGroup("Advanced Settings", true, false, 0)]
		public bool hasReflections;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public ScreenMaterialEffectsManager.SCREEN_EFFECTS screenEffect;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public float sleepTime;

		[BoxGroup("Designer Settings", true, false, 0)]
		[SerializeField]
		private bool DisplayZoneTitle = true;

		[BoxGroup("Designer Settings", true, false, 0)]
		[SerializeField]
		[ShowIf("DisplayZoneTitle", true)]
		public List<string> ignoreTitleIfComingFromScenes = new List<string>();

		[BoxGroup("Designer Settings", true, false, 0)]
		[SerializeField]
		public LevelInitializer.GuiltConfigurationEnum GuiltConfiguration;

		[BoxGroup("Designer Settings", true, false, 0)]
		[SerializeField]
		[ShowIf("ShowPositionOverrider", true)]
		public Transform guiltPositionOverrider;

		private EnemyStatsImporter enemyStatsImporter;

		[NonSerialized]
		public float TimeScaleReal = 1f;

		private static bool IsInitilizated;

		public enum GuiltConfigurationEnum
		{
			Default,
			OverridePosition,
			NotInThisScene,
			DontGenerateGuilt
		}
	}
}
