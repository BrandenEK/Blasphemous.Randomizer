using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.Penitences;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Damage;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;

namespace Framework.Managers
{
	public class PenitenceManager : GameSystem, PersistentInterface
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event PenitenceManager.PenitenceDelegate OnPenitenceChanged;

		private static int FlaskUpgradeLevel
		{
			get
			{
				return (int)(Core.Logic.Penitent.Stats.FlaskHealth.PermanetBonus / Core.Logic.Penitent.Stats.FlaskHealthUpgrade) + 1;
			}
		}

		public override void Initialize()
		{
			this.ResetPenitencesList();
			this.regenFactor = 0f;
			this.flaskRegenerationBalance = Resources.Load<FlaskRegenerationBalance>("PE02/FlaskRegenerationBalance");
			if (!this.flaskRegenerationBalance)
			{
				UnityEngine.Debug.LogErrorFormat("Can't find flask regeneration balance chart at {0}", new object[]
				{
					"PE02/FlaskRegenerationBalance"
				});
			}
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnPenitentHit));
		}

		public override void Dispose()
		{
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnPenitentHit));
		}

		public override void Update()
		{
			if (Mathf.Approximately(this.regenFactor, 0f) || !this.isLevelLoaded || !Core.ready || Core.Logic.Penitent == null)
			{
				return;
			}
			Life life = Core.Logic.Penitent.Stats.Life;
			if (this.isRegenActive)
			{
				this.RestoreHealth();
				if (life.Current >= life.CurrentMax)
				{
					this.isRegenActive = false;
				}
			}
			else
			{
				Healing componentInChildren = Core.Logic.Penitent.GetComponentInChildren<Healing>();
				if (componentInChildren.IsHealing && life.Current < life.CurrentMax)
				{
					this.isRegenActive = true;
					this.lifeAccum = 0f;
				}
			}
		}

		public void ActivatePenitence(string id)
		{
			IPenitence penitence = this.allPenitences.Find((IPenitence p) => p.Id.Equals(id));
			if (penitence != null)
			{
				this.ActivatePenitence(penitence);
			}
			else
			{
				UnityEngine.Debug.LogError("ActivatePenitence: Penitence with id '" + id + "' does not exist!");
			}
		}

		public void ActivatePE01()
		{
			this.ActivatePenitence(this.allPenitences.Find((IPenitence p) => p is PenitencePE01));
		}

		public void ActivatePE02()
		{
			this.ActivatePenitence(this.allPenitences.Find((IPenitence p) => p is PenitencePE02));
		}

		public void ActivatePE03()
		{
			this.ActivatePenitence(this.allPenitences.Find((IPenitence p) => p is PenitencePE03));
		}

		public bool CheckPenitenceExists(string id)
		{
			return this.allPenitences.Exists((IPenitence p) => p.Id.Equals(id));
		}

		public void DeactivateCurrentPenitence()
		{
			if (this.currentPenitence != null)
			{
				this.currentPenitence.Deactivate();
				this.currentPenitence = null;
				this.SendEvent();
			}
		}

		public void MarkCurrentPenitenceAsAbandoned()
		{
			this.currentPenitence.Abandoned = true;
			this.DeactivateCurrentPenitence();
			this.SendEvent();
		}

		public void MarkCurrentPenitenceAsCompleted()
		{
			this.currentPenitence.Completed = true;
			this.SendEvent();
		}

		public IPenitence GetCurrentPenitence()
		{
			return this.currentPenitence;
		}

		public List<IPenitence> GetAllPenitences()
		{
			return this.allPenitences;
		}

		public List<IPenitence> GetAllCompletedPenitences()
		{
			return this.allPenitences.FindAll((IPenitence p) => p.Completed);
		}

		public List<IPenitence> GetAllAbandonedPenitences()
		{
			return this.allPenitences.FindAll((IPenitence p) => p.Abandoned);
		}

		public float GetPercentageCompletition()
		{
			return (float)this.allPenitences.Count((IPenitence p) => p.Completed) * GameConstants.PercentageValues[PersistentManager.PercentageType.Penitence_NgPlus];
		}

		public void MarkPenitenceAsCompleted(string id)
		{
			IPenitence penitence = this.allPenitences.Find((IPenitence x) => x.Id == id);
			if (penitence != null)
			{
				penitence.Completed = true;
			}
			else
			{
				UnityEngine.Debug.LogError("MarkPenitenceAsCompleted: penitence with id: " + id + " could not be found!");
			}
		}

		public void AddFlasksPassiveHealthRegen(float regenFactor)
		{
			this.regenFactor += regenFactor;
			if (this.regenFactor < 0f)
			{
				this.regenFactor = 0f;
			}
		}

		public void ResetRegeneration()
		{
			if (this.isRegenActive)
			{
				PlayerHealthPE02.FillAmount = 0f;
				this.lifeAccum = 0f;
			}
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			this.isLevelLoaded = false;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.isLevelLoaded = true;
		}

		private void OnPenitentHit(Penitent penitent, Hit hit)
		{
			if (this.isRegenActive)
			{
				this.isRegenActive = false;
				PlayerHealthPE02.FillAmount = 0f;
				this.lifeAccum = 0f;
			}
		}

		private void ActivatePenitence(IPenitence penitence)
		{
			if (this.currentPenitence != null)
			{
				this.currentPenitence.Deactivate();
			}
			this.currentPenitence = penitence;
			this.currentPenitence.Activate();
			this.SendEvent();
		}

		private void ResetPenitencesList()
		{
			this.allPenitences = new List<IPenitence>
			{
				new PenitencePE01(),
				new PenitencePE02(),
				new PenitencePE03()
			};
			this.SendEvent();
		}

		private void SendEvent()
		{
			if (PenitenceManager.OnPenitenceChanged != null)
			{
				PenitenceManager.OnPenitenceChanged(this.GetCurrentPenitence(), this.GetAllCompletedPenitences());
			}
		}

		private void RestoreHealth()
		{
			if (Core.PenitenceManager.UseStocksOfHealth)
			{
				if (this.lastFlaskLevel != PenitenceManager.FlaskUpgradeLevel)
				{
					this.regenerationPerSecond = this.flaskRegenerationBalance.GetTimeByFlaskLevel(PenitenceManager.FlaskUpgradeLevel);
					this.lastFlaskLevel = PenitenceManager.FlaskUpgradeLevel;
				}
				this.lifeAccum += Time.deltaTime * (30f / (this.regenerationPerSecond / this.regenFactor));
				float num = 30f;
				float num2 = Core.Logic.Penitent.Stats.Life.Current % num;
				if (num2 > 0f)
				{
					num = num2;
				}
				PlayerHealthPE02.FillAmount = (float)((int)(this.lifeAccum / num / 0.09090909f)) / 11f;
				if (this.lifeAccum >= num)
				{
					Core.Logic.Penitent.Stats.Life.Current += num;
					this.lifeAccum = 0f;
					PlayerHealthPE02.FillAmount = 1f;
				}
			}
			else
			{
				Core.Logic.Penitent.Stats.Life.Current += Time.deltaTime * this.regenFactor * (float)PenitenceManager.FlaskUpgradeLevel;
			}
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("Penitence -------------------------------------", 10, 1500);
			base.DebugDrawTextLine("List:", 10, 1500);
			foreach (IPenitence penitence in this.allPenitences)
			{
				string text = string.Empty;
				if (penitence == this.currentPenitence)
				{
					text = "CURRENT -->";
				}
				base.DebugDrawTextLine(string.Concat(new object[]
				{
					text,
					penitence.Id,
					": Abandoned:",
					penitence.Abandoned,
					" Completed:",
					penitence.Completed
				}), 10, 1500);
			}
			if (this.regenFactor > 0f)
			{
				base.DebugDrawTextLine(string.Empty, 10, 1500);
				base.DebugDrawTextLine("Health Regeneration:", 10, 1500);
				base.DebugDrawTextLine("IsRegenActive:" + this.isRegenActive, 10, 1500);
				base.DebugDrawTextLine("UseStocksOfHealth:" + this.UseStocksOfHealth, 10, 1500);
				base.DebugDrawTextLine("IsLevelLoaded:" + this.isLevelLoaded, 10, 1500);
				base.DebugDrawTextLine("RegenFactor:" + this.regenFactor, 10, 1500);
				base.DebugDrawTextLine("Flask Level:" + PenitenceManager.FlaskUpgradeLevel, 10, 1500);
				base.DebugDrawTextLine(string.Concat(new object[]
				{
					"Time for flask level ",
					PenitenceManager.FlaskUpgradeLevel,
					": ",
					this.regenerationPerSecond
				}), 10, 1500);
				base.DebugDrawTextLine("Final time for flask level and RegenFactor: " + this.regenerationPerSecond / this.regenFactor, 10, 1500);
				base.DebugDrawTextLine("LifeAccum:" + this.lifeAccum, 10, 1500);
			}
		}

		public int GetOrder()
		{
			return 10;
		}

		public string GetPersistenID()
		{
			return "ID_PENITENCE";
		}

		public void ResetPersistence()
		{
			if (this.currentPenitence != null)
			{
				this.DeactivateCurrentPenitence();
			}
			this.ResetPenitencesList();
			this.regenFactor = 0f;
			this.lifeAccum = 0f;
			this.isLevelLoaded = false;
			this.isRegenActive = false;
			this.UseFervourFlasks = false;
			this.UseStocksOfHealth = false;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new PenitenceManager.PenitencePersistenceData
			{
				currentPenitence = this.currentPenitence,
				allPenitences = this.allPenitences
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			PenitenceManager.PenitencePersistenceData penitencePersistenceData = (PenitenceManager.PenitencePersistenceData)data;
			if (penitencePersistenceData.currentPenitence != null)
			{
				this.ActivatePenitence(penitencePersistenceData.currentPenitence);
			}
			this.allPenitences = penitencePersistenceData.allPenitences;
			for (int i = 0; i < 3; i++)
			{
				if (this.allPenitences[i] == null)
				{
					if (i == 0)
					{
						this.allPenitences[i] = new PenitencePE01();
					}
					else if (i == 1)
					{
						this.allPenitences[i] = new PenitencePE02();
					}
					else if (i == 2)
					{
						this.allPenitences[i] = new PenitencePE03();
					}
					else
					{
						UnityEngine.Debug.LogError("Invalid Penitence Index: " + i);
					}
				}
			}
		}

		private IPenitence currentPenitence;

		private List<IPenitence> allPenitences;

		public bool UseStocksOfHealth;

		public const float HealthPerStock = 30f;

		private FlaskRegenerationBalance flaskRegenerationBalance;

		private const string FLASK_REGENERATION_BALANCE_CHART = "PE02/FlaskRegenerationBalance";

		private float regenFactor;

		private float lifeAccum;

		private bool isLevelLoaded;

		private bool isRegenActive;

		private int lastFlaskLevel;

		private float regenerationPerSecond;

		public bool UseFervourFlasks;

		private const string PERSITENT_ID = "ID_PENITENCE";

		public delegate void PenitenceDelegate(IPenitence current, List<IPenitence> completed);

		[Serializable]
		public class PenitencePersistenceData : PersistentManager.PersistentData
		{
			public PenitencePersistenceData() : base("ID_PENITENCE")
			{
			}

			public IPenitence currentPenitence;

			public List<IPenitence> allPenitences = new List<IPenitence>();
		}
	}
}
