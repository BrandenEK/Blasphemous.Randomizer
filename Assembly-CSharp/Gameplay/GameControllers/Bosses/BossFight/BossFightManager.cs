using System;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameControllers.Bosses.BossFight
{
	public class BossFightManager : MonoBehaviour
	{
		public BossFightAudio Audio { get; set; }

		public BossFightMetrics Metrics { get; set; }

		public string CurrentBossPhaseId { get; private set; }

		public bool IsFightStarted { get; private set; }

		private void Awake()
		{
			this.CurrentBossPhaseId = "state1";
			this.Audio = base.GetComponent<BossFightAudio>();
			this.Metrics = base.GetComponent<BossFightMetrics>();
		}

		private void Start()
		{
			this.Init();
		}

		public void Init()
		{
			if (!this.Boss)
			{
				return;
			}
			this.SetCurrentActivePhase();
			this.Boss.OnDamaged += this.OnDamageBoss;
			this.Boss.OnDeath += this.OnDeathBoss;
		}

		private void Update()
		{
			if (this.IsFightStarted)
			{
				this.Audio.SetBossTrackState(this.Boss.CurrentLife);
			}
		}

		private void OnDestroy()
		{
			if (!this.Boss)
			{
				return;
			}
			this.Boss.OnDamaged -= this.OnDamageBoss;
			this.Boss.OnDeath -= this.OnDeathBoss;
		}

		public void EnableLifeBar(bool enableLifeBar = true)
		{
			if (enableLifeBar)
			{
				if (this.ShowLifeBar)
				{
					UIController.instance.ShowBossHealth(this.Boss);
				}
				this.Boss.UseHealthBar = false;
			}
			else
			{
				UIController.instance.HideBossHealth();
			}
		}

		public void StartBossFight()
		{
			this.StartBossFight(true);
		}

		public void StartBossFight(bool playBossTrack = true)
		{
			this.EnableLifeBar(true);
			this.SetCurrentActivePhase();
			if (playBossTrack)
			{
				this.Audio.PlayBossTrack();
				this.Audio.SetBossTrackState(this.BossCurrentLifeNormalized());
			}
			this.Metrics.StartBossFight();
			this.IsFightStarted = true;
			this.numFlasksAtStartBossFight = Core.Logic.Penitent.Stats.Flask.Current;
			this.onStartBossFight.Invoke();
		}

		public void SetCurrentActivePhase()
		{
			BossPhase currentPhase = this.GetCurrentPhase();
			if (currentPhase == null)
			{
				return;
			}
			foreach (BossPhase bossPhase in this.Phases)
			{
				bossPhase.IsActive = bossPhase.Id.Equals(currentPhase.Id);
				if (bossPhase.IsActive)
				{
					this.CurrentBossPhaseId = bossPhase.Id;
				}
			}
		}

		private BossPhase GetCurrentPhase()
		{
			if (this.Phases == null)
			{
				return null;
			}
			float currentBossLife = 100f;
			if (this.Boss.gameObject.activeInHierarchy)
			{
				currentBossLife = this.Boss.CurrentLife / this.Boss.Stats.Life.Base * 100f;
			}
			return this.Phases.FirstOrDefault((BossPhase phase) => currentBossLife >= phase.PhaseInterval.x && currentBossLife <= phase.PhaseInterval.y);
		}

		private float BossCurrentLifeNormalized()
		{
			return this.Boss.CurrentLife / this.Boss.Stats.Life.Base * 100f;
		}

		public Vector3 GetPenitentSpawnPoint()
		{
			return this.SpawnPoint.position;
		}

		public void SetPenitentOnSpawnPoint()
		{
			Penitent penitent = Core.Logic.Penitent;
			if (penitent != null)
			{
				Vector2 vector = this.GetPenitentSpawnPoint();
				penitent.transform.position = vector;
			}
		}

		private void OnDamageBoss()
		{
			this.SetCurrentActivePhase();
		}

		private void OnDeathBoss()
		{
			this.EnableLifeBar(false);
			this.Metrics.EndBossFight();
			this.Audio.SetBossTrackState(this.BossCurrentLifeNormalized());
			this.Audio.SetBossEndingMusic(1f);
			this.IsFightStarted = false;
			this.onFinishBossFight.Invoke();
		}

		public void AddProgressToAC43()
		{
			if (!this.BossCountsTowardsAC43)
			{
				return;
			}
			this.numFlasksAtEndBossFight = Core.Logic.Penitent.Stats.Flask.Current;
			if (Core.Events.GetFlag(Core.LevelManager.currentLevel.LevelName + "_BOSSDEAD_AC43"))
			{
				Debug.Log("There was previously an increase to AC43 due to this boss!");
				return;
			}
			if (this.numFlasksAtStartBossFight == this.numFlasksAtEndBossFight)
			{
				Core.AchievementsManager.Achievements["AC43"].AddProgress(9.090909f);
				Core.Events.SetFlag(Core.LevelManager.currentLevel.LevelName + "_BOSSDEAD_AC43", true, false);
			}
		}

		public UnityEvent onStartBossFight;

		public UnityEvent onFinishBossFight;

		public const string State1 = "state1";

		public const string State2 = "state2";

		public const string State3 = "state3";

		public const string Ending = "Ending";

		public Enemy Boss;

		public BossPhase[] Phases;

		public bool BossCountsTowardsAC43 = true;

		public Transform SpawnPoint;

		public bool ShowLifeBar = true;

		private float numFlasksAtStartBossFight;

		private float numFlasksAtEndBossFight;

		private const int TOTAL_NUMBER_OF_BOSSES_FOR_AC43 = 11;
	}
}
