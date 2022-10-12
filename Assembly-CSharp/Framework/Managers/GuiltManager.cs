using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Map;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Tools.DataContainer;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using UnityEngine;

namespace Framework.Managers
{
	public class GuiltManager : GameSystem, PersistentInterface
	{
		private Penitent Penitent { get; set; }

		public override void Start()
		{
			this.guiltConfig = Resources.Load<GuiltConfigData>("GuiltConfig");
			this.InitializeAll();
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
		}

		private void InitializeAll()
		{
			this.guiltDrops.Clear();
		}

		private void OnPenitentReady(Penitent penitent)
		{
			if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH) || Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				return;
			}
			this.Penitent = penitent;
			Penitent penitent2 = this.Penitent;
			penitent2.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent2.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			this.Penitent.Stats.Fervour.MaxFactor = this.GetFervourMaxFactor();
		}

		private void OnPenitentDead()
		{
			if (Core.Logic.CurrentLevelConfig.GuiltConfiguration != LevelInitializer.GuiltConfigurationEnum.DontGenerateGuilt && Core.Logic.Penitent.GuiltDrop)
			{
				this.AddDrop();
				if (this.DropTearsAlongWithGuilt)
				{
					this.DroppedTears = Core.Logic.Penitent.Stats.Purge.Current;
					Core.Logic.Penitent.Stats.Purge.Current = 0f;
				}
			}
			Core.InventoryManager.OnPenitentDead();
			if (this.Penitent)
			{
				Penitent penitent = this.Penitent;
				penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			}
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.guiltDrops.ForEach(delegate(GuiltManager.GuiltDrop drop)
			{
				this.InstanciateDrop(drop);
			});
		}

		public void OnDropTaken(string ID)
		{
			GuiltManager.GuiltDrop guiltDrop = this.guiltDrops.FirstOrDefault((GuiltManager.GuiltDrop x) => x.id == ID && x.scene == Core.LevelManager.currentLevel.LevelName);
			if (guiltDrop != null)
			{
				using (List<string>.Enumerator enumerator = guiltDrop.linkedIds.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string linkedId = enumerator.Current;
						GuiltManager.GuiltDrop guiltDrop2 = this.guiltDrops.FirstOrDefault((GuiltManager.GuiltDrop x) => x.id == linkedId && x.scene == Core.LevelManager.currentLevel.LevelName);
						if (guiltDrop2 != null)
						{
							this.guiltDrops.Remove(guiltDrop2);
						}
						else
						{
							Debug.LogError(string.Concat(new string[]
							{
								"Linked Drop taken NOT found ID:",
								ID,
								"  IdLinked:",
								linkedId,
								"  Scene",
								Core.LevelManager.currentLevel.LevelName,
								"  Current: ",
								Core.LevelManager.currentLevel.LevelName
							}));
						}
					}
				}
				this.guiltDrops.Remove(guiltDrop);
				this.UpdateGuilt(false);
			}
			else
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Drop taken NOT found ID:",
					ID,
					" Scene",
					Core.LevelManager.currentLevel.LevelName,
					"  Current: ",
					Core.LevelManager.currentLevel.LevelName
				}));
			}
		}

		public float GetFervourGainFactor()
		{
			float result = this.guiltConfig.fervourGainFactor.Evaluate((float)this.guiltDrops.Count);
			if (this.guiltDrops.Count > 0 && this.DropSingleGuilt)
			{
				result = this.guiltConfig.fervourGainFactor.Evaluate((float)this.guiltConfig.maxDeathsDrops);
			}
			return result;
		}

		public float GetFervourMaxFactor()
		{
			if (!this.guiltConfig)
			{
				return 0f;
			}
			float result = this.guiltConfig.fervourMaxFactor.Evaluate((float)this.guiltDrops.Count);
			if (this.guiltDrops.Count > 0 && this.DropSingleGuilt)
			{
				result = this.guiltConfig.fervourMaxFactor.Evaluate((float)this.guiltConfig.maxDeathsDrops);
			}
			return result;
		}

		public float GetPurgeGainFactor()
		{
			float result = this.guiltConfig.purgeGainFactor.Evaluate((float)this.guiltDrops.Count);
			if (this.guiltDrops.Count > 0 && this.DropSingleGuilt)
			{
				result = this.guiltConfig.purgeGainFactor.Evaluate((float)this.guiltConfig.maxDeathsDrops);
			}
			return result;
		}

		public ReadOnlyCollection<GuiltManager.GuiltDrop> GetAllDrops()
		{
			return this.guiltDrops.AsReadOnly();
		}

		public List<GuiltManager.GuiltDrop> GetAllCurrentMapDrops()
		{
			return this.guiltDrops.ToList<GuiltManager.GuiltDrop>();
		}

		public int GetDropsCount()
		{
			int num = this.guiltDrops.Count;
			if (num > 0 && this.DropSingleGuilt)
			{
				num = this.guiltConfig.maxDeathsDrops;
			}
			return num;
		}

		public void ResetGuilt(bool restoreDropTears)
		{
			if (restoreDropTears && this.DropTearsAlongWithGuilt)
			{
				Core.Logic.Penitent.Stats.Purge.Current += Core.GuiltManager.DroppedTears;
				Core.GuiltManager.DroppedTears = 0f;
			}
			this.guiltDrops.Clear();
			InteractableGuiltDrop[] array = UnityEngine.Object.FindObjectsOfType<InteractableGuiltDrop>();
			foreach (InteractableGuiltDrop interactableGuiltDrop in array)
			{
				UnityEngine.Object.Destroy(interactableGuiltDrop.gameObject);
			}
			this.UpdateGuilt(true);
		}

		public void AddGuilt()
		{
			GuiltManager.GuiltDrop drop = this.AddDrop();
			this.InstanciateDrop(drop);
			this.UpdateGuilt(true);
		}

		public void DEBUG_AddAllGuilts()
		{
			for (int i = 0; i < this.guiltConfig.maxDeathsDrops; i++)
			{
				GuiltManager.GuiltDrop guiltDrop = this.AddDrop();
			}
		}

		private void UpdateGuilt(bool whenDead)
		{
			if (Core.Logic.Penitent != null)
			{
				Core.Logic.Penitent.Stats.Fervour.MaxFactor = this.GetFervourMaxFactor();
			}
			UIController.instance.UpdateGuiltLevel(whenDead);
		}

		private GuiltManager.GuiltDrop AddDrop()
		{
			if (this.guiltDrops.Count >= this.guiltConfig.maxDeathsDrops)
			{
				return null;
			}
			if (this.DropSingleGuilt && this.guiltDrops.Count == 1)
			{
				this.ResetGuilt(false);
			}
			GuiltManager.GuiltDrop drop = new GuiltManager.GuiltDrop();
			do
			{
				drop.id = Guid.NewGuid().ToString();
			}
			while (this.guiltDrops.FirstOrDefault((GuiltManager.GuiltDrop p) => p.id == drop.id) != null);
			if (Core.Logic.CurrentLevelConfig.OverrideGuiltPosition)
			{
				drop.position = Core.Logic.CurrentLevelConfig.guiltPositionOverrider.position;
				drop.scene = Core.LevelManager.currentLevel.LevelName;
			}
			else if (Core.LevelManager.LastSafeGuiltLevel == string.Empty)
			{
				Debug.LogWarning("No safe position for Guilt, placing on safe level");
				drop.position = new Vector3(-513f, 11f);
				drop.scene = "D01Z02S01";
			}
			else
			{
				drop.position = Core.LevelManager.LastSafeGuiltPosition;
				drop.scene = Core.LevelManager.LastSafeGuiltLevel;
				drop.cellKey = Core.LevelManager.LastSafeGuiltCellKey;
			}
			if (drop.cellKey == null)
			{
				drop.cellKey = Core.NewMapManager.GetCellKeyFromPosition(drop.scene, drop.position);
			}
			if (drop.cellKey == null)
			{
				drop.cellKey = Core.NewMapManager.GetPlayerCell();
				Debug.LogWarning(string.Concat(new object[]
				{
					"[Debug only message] Unable to find CellKey for new guilt drop. SCENE:",
					drop.scene,
					"  POS:",
					drop.position,
					" LEVEL:",
					Core.LevelManager.currentLevel.LevelName,
					"  OVERRIDE:",
					Core.Logic.CurrentLevelConfig.OverrideGuiltPosition
				}));
			}
			foreach (GuiltManager.GuiltDrop guiltDrop in this.guiltDrops)
			{
				if (!guiltDrop.isLinked && guiltDrop.scene == drop.scene)
				{
					float magnitude = (guiltDrop.position - drop.position).magnitude;
					if (magnitude <= this.guiltConfig.MaxDistanceToLink)
					{
						drop.isLinked = true;
						guiltDrop.linkedIds.Add(drop.id);
						break;
					}
				}
			}
			this.guiltDrops.Add(drop);
			return drop;
		}

		private void InstanciateDrop(GuiltManager.GuiltDrop drop)
		{
			if (drop != null && !drop.isLinked && drop.scene == Core.LevelManager.currentLevel.LevelName)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.guiltConfig.dropPrefab, drop.position, Quaternion.identity);
				gameObject.GetComponent<InteractableGuiltDrop>().SetDropId(drop.id);
			}
		}

		public int GetOrder()
		{
			return 10;
		}

		public string GetPersistenID()
		{
			return "ID_GUILT";
		}

		public void ResetPersistence()
		{
			this.InitializeAll();
			this.DropSingleGuilt = false;
			this.DropTearsAlongWithGuilt = false;
			this.DroppedTears = 0f;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new GuiltManager.GuiltPersistenceData
			{
				guiltDrops = new List<GuiltManager.GuiltDrop>(this.guiltDrops),
				DropSingleGuilt = this.DropSingleGuilt,
				DropTearsAlongWithGuilt = this.DropTearsAlongWithGuilt,
				DroppedTears = this.DroppedTears
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			GuiltManager.GuiltPersistenceData guiltPersistenceData = (GuiltManager.GuiltPersistenceData)data;
			this.guiltDrops = new List<GuiltManager.GuiltDrop>(guiltPersistenceData.guiltDrops);
			foreach (GuiltManager.GuiltDrop guiltDrop in this.guiltDrops)
			{
				if (guiltDrop.cellKey == null)
				{
					guiltDrop.cellKey = Core.NewMapManager.GetCellKeyFromPosition(guiltDrop.position);
				}
			}
			this.DropSingleGuilt = guiltPersistenceData.DropSingleGuilt;
			this.DropTearsAlongWithGuilt = guiltPersistenceData.DropTearsAlongWithGuilt;
			this.DroppedTears = guiltPersistenceData.DroppedTears;
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("GuiltManager -------------------------------------", 10, 1500);
			base.DebugDrawTextLine("-- Config max drops: " + this.guiltConfig.maxDeathsDrops.ToString(), 10, 1500);
			base.DebugDrawTextLine("   Config max distance to link: " + this.guiltConfig.MaxDistanceToLink.ToString(), 10, 1500);
			base.DebugDrawTextLine("-- Current Fervour Gain Factor: " + this.GetFervourGainFactor().ToString(), 10, 1500);
			base.DebugDrawTextLine("   Current Fervour Max Factor: " + this.GetFervourMaxFactor().ToString(), 10, 1500);
			base.DebugDrawTextLine("   Current Purge Gain Factor: " + this.GetPurgeGainFactor().ToString(), 10, 1500);
			base.DebugDrawTextLine("-- DROPS: " + this.guiltDrops.Count.ToString(), 10, 1500);
			foreach (GuiltManager.GuiltDrop guiltDrop in this.guiltDrops)
			{
				string text = string.Concat(new string[]
				{
					"    ID: ",
					guiltDrop.id,
					" Scene:",
					guiltDrop.scene,
					"  POS: ",
					guiltDrop.position.ToString()
				});
				if (guiltDrop.isLinked)
				{
					text += "  LINKED";
				}
				base.DebugDrawTextLine(text, 10, 1500);
				if (guiltDrop.linkedIds.Count > 0)
				{
					foreach (string str in guiltDrop.linkedIds)
					{
						base.DebugDrawTextLine("        LINKED ID: " + str, 10, 1500);
					}
				}
			}
		}

		private const string GUILD_RESOURCE_CONFIG = "GuiltConfig";

		private GuiltConfigData guiltConfig;

		private List<GuiltManager.GuiltDrop> guiltDrops = new List<GuiltManager.GuiltDrop>();

		public const int GLOBAL_SAFE_POSITION_X = -513;

		public const int GLOBAL_SAFE_POSITION_Y = 11;

		public const string GLOBAL_SAFE_LEVEL = "D01Z02S01";

		public bool DropSingleGuilt;

		public bool DropTearsAlongWithGuilt;

		public float DroppedTears;

		private const string PERSITENT_ID = "ID_GUILT";

		[Serializable]
		public class GuiltDrop
		{
			public string id;

			public string scene;

			public Vector3 position;

			public List<string> linkedIds = new List<string>();

			public bool isLinked;

			public CellKey cellKey;
		}

		[Serializable]
		public class GuiltPersistenceData : PersistentManager.PersistentData
		{
			public GuiltPersistenceData() : base("ID_GUILT")
			{
			}

			public List<GuiltManager.GuiltDrop> guiltDrops;

			public bool DropSingleGuilt;

			public bool DropTearsAlongWithGuilt;

			public float DroppedTears;
		}
	}
}
