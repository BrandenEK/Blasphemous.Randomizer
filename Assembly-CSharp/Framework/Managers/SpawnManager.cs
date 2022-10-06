using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Sirenix.Utilities;
using Tools.Level;
using Tools.Level.Interactables;
using Tools.Level.Layout;
using UnityEngine;

namespace Framework.Managers
{
	public class SpawnManager : GameSystem, PersistentInterface
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event SpawnManager.SpawnEvent OnPlayerSpawn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event SpawnManager.TeleportEvent OnTeleport;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event SpawnManager.TeleportEvent OnTeleportPrieDieu;

		public string InitialScene { get; set; }

		public override void Start()
		{
			this.HidePlayerInNextSpawn = false;
			this.FirstSpanw = false;
			this.AutomaticRespawn = false;
			this.IgnoreNextAutomaticRespawn = false;
			this.customLevel = string.Empty;
			this.customIsMiriam = false;
			this.LoadAllTeleports();
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public bool AutomaticRespawn { get; set; }

		public bool IgnoreNextAutomaticRespawn { get; set; }

		public void Teleport(string teleportId)
		{
			if (this.TeleportDict.ContainsKey(teleportId))
			{
				this.Teleport(this.TeleportDict[teleportId]);
			}
			else
			{
				Debug.LogError("The key " + teleportId + "is not present in the teleports dictionary!");
			}
		}

		public void Teleport(TeleportDestination teleport)
		{
			Color? color = null;
			if (teleport.UseCustomLoadColor)
			{
				color = new Color?(teleport.CustomLoadColor);
			}
			string sceneName = teleport.sceneName;
			SpawnManager.PosibleSpawnPoints spawnType = SpawnManager.PosibleSpawnPoints.Teleport;
			string teleportName = teleport.teleportName;
			bool useFade = teleport.UseFade;
			Color? background = color;
			this.SpawnPlayer(sceneName, spawnType, teleportName, useFade, false, background);
		}

		public void SpawnFromMiriam(string targetScene, string targetTeleport, bool useFade)
		{
			this.SpawnPlayer(targetScene, SpawnManager.PosibleSpawnPoints.Miriam, targetTeleport, useFade, true, null);
		}

		public void SpawnFromDoor(string targetScene, string targetDoor, bool useFade)
		{
			this.SpawnPlayer(targetScene, SpawnManager.PosibleSpawnPoints.Door, targetDoor, useFade, false, null);
		}

		public void SpawnFromMenu()
		{
			this.SpawnPlayer(string.Empty, SpawnManager.PosibleSpawnPoints.Menu, string.Empty, true, false, null);
		}

		public void SpawnFromTeleportOnPrieDieu(string targetScene, bool useFade)
		{
			this.SpawnPlayer(targetScene, SpawnManager.PosibleSpawnPoints.TeleportPrieDieu, string.Empty, useFade, false, null);
		}

		public void SetCurrentToCustomSpawnData(bool IsMiriam)
		{
			this.customIsMiriam = IsMiriam;
			this.customLevel = Core.LevelManager.currentLevel.LevelName;
			this.customPosition = Core.Logic.Penitent.GetPosition();
			this.customOrientation = Core.Logic.Penitent.GetOrientation();
		}

		public bool HidePlayerInNextSpawn { get; set; }

		public void SpawnFromCustom(bool usefade, Color? background = null)
		{
			if (this.customIsMiriam)
			{
				this.SpawnPlayer(Core.Events.MiriamCurrentScenePortalToReturn, SpawnManager.PosibleSpawnPoints.Miriam, string.Empty, usefade, false, background);
			}
			else if (this.customLevel != string.Empty)
			{
				this.SpawnPlayer(this.customLevel, SpawnManager.PosibleSpawnPoints.CustomPosition, string.Empty, usefade, false, background);
			}
			this.customLevel = string.Empty;
			this.customIsMiriam = false;
		}

		public void Respawn()
		{
			if (StringExtensions.IsNullOrWhitespace(this.activePrieDieuScene))
			{
				Debug.LogError("Respawn: Respawn without scene, loading initial");
				this.activePrieDieuScene = this.InitialScene;
			}
			this.SpawnPlayer(this.activePrieDieuScene, SpawnManager.PosibleSpawnPoints.PrieDieu, this.activePrieDieuId, true, true, null);
			UIController.instance.UpdatePurgePoints();
		}

		public void RespawnSafePosition()
		{
			Core.Events.SetFlag("CHERUB_RESPAWN", true, false);
			this.safePosition = Core.LevelManager.LastSafePosition;
			this.SpawnPlayer(Core.LevelManager.LastSafeLevel, SpawnManager.PosibleSpawnPoints.SafePosition, string.Empty, true, true, null);
			UIController.instance.UpdatePurgePoints();
		}

		public void RespawnMiriamSameLevel(bool useFade, Color? background = null)
		{
			Core.Events.SetFlag("CHERUB_RESPAWN", true, false);
			this.safePosition = Core.LevelManager.LastSafePosition;
			this.SpawnPlayer(Core.LevelManager.currentLevel.LevelName, SpawnManager.PosibleSpawnPoints.Miriam, string.Empty, useFade, true, background);
			UIController.instance.UpdatePurgePoints();
		}

		public ReadOnlyCollection<TeleportDestination> GetAllTeleports()
		{
			return this.TeleportList.AsReadOnly();
		}

		public ReadOnlyCollection<TeleportDestination> GetAllUIActiveTeleports()
		{
			return (from element in this.TeleportList
			where element.isActive && element.useInUI
			select element).ToList<TeleportDestination>().AsReadOnly();
		}

		public void SetTeleportActive(string teleportId, bool active)
		{
			if (this.TeleportDict.ContainsKey(teleportId))
			{
				this.TeleportDict[teleportId].isActive = active;
				Core.AchievementsManager.CheckProgressToAC46();
			}
		}

		public bool IsTeleportActive(string teleportId)
		{
			bool result = false;
			if (this.TeleportDict.ContainsKey(teleportId))
			{
				result = this.TeleportDict[teleportId].isActive;
			}
			return result;
		}

		public float GetPercentageCompletition()
		{
			float num = 0f;
			foreach (TeleportDestination teleportDestination in from x in this.TeleportDict.Values
			where x.useInCompletition && x.isActive
			select x)
			{
				num += GameConstants.PercentageValues[teleportDestination.percentageType];
			}
			return num;
		}

		public PrieDieu ActivePrieDieu
		{
			set
			{
				if (value == null)
				{
					return;
				}
				value.Ligthed = true;
				this.activePrieDieuScene = Core.LevelManager.currentLevel.LevelName;
				this.activePrieDieuId = value.GetPersistenID();
			}
		}

		public void SetActivePriedieuManually(string levelName, string activePriedieuPersistentID)
		{
			this.activePrieDieuScene = levelName;
			this.activePrieDieuId = activePriedieuPersistentID;
		}

		public string GetActivePriedieuScene()
		{
			return this.activePrieDieuScene;
		}

		public string GetActivePriedieuId()
		{
			return this.activePrieDieuId;
		}

		public void PrepareForSpawnFromEditor()
		{
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.Editor;
		}

		public void PrepareForSpawnFromMenu()
		{
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.Menu;
		}

		public void SetInitialSpawn(string level)
		{
			this.activePrieDieuScene = level;
		}

		public void PrepareForCommandSpawn(string scene)
		{
			this.activePrieDieuId = string.Empty;
			this.spawnId = string.Empty;
			this.activePrieDieuScene = scene;
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.PrieDieu;
		}

		public void PrepareForNewGamePlus(string initialScene)
		{
			this.ResetPersistence();
			this.activePrieDieuScene = initialScene;
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.PrieDieu;
		}

		public void PrepareForBossRush()
		{
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.PrieDieu;
		}

		public void SpawnPlayerOnLevelLoad(bool createNewInstance = true)
		{
			bool flag = false;
			PrieDieu[] array = Object.FindObjectsOfType<PrieDieu>();
			this.currentDoor = null;
			string text = string.Empty;
			Vector3 position = Vector3.zero;
			EntityOrientation orientation = EntityOrientation.Right;
			switch (this.pendingSpawn)
			{
			case SpawnManager.PosibleSpawnPoints.None:
				Debug.LogWarning("SpawnManager: Pending spawn IS NONE");
				break;
			case SpawnManager.PosibleSpawnPoints.Menu:
				flag = true;
				break;
			case SpawnManager.PosibleSpawnPoints.Editor:
			{
				DebugSpawn debugSpawn = Object.FindObjectOfType<DebugSpawn>();
				if (debugSpawn)
				{
					position = debugSpawn.transform.position;
					orientation = EntityOrientation.Right;
					flag = true;
					text = debugSpawn.initialCommands;
				}
				else if (array.Length > 0)
				{
					position = array[0].transform.position;
					orientation = array[0].spawnOrientation;
					flag = true;
				}
				break;
			}
			case SpawnManager.PosibleSpawnPoints.PrieDieu:
			{
				PrieDieu prieDieu = array.FirstOrDefault((PrieDieu p) => p.GetPersistenID() == this.spawnId);
				if (prieDieu)
				{
					position = prieDieu.transform.position;
					orientation = prieDieu.spawnOrientation;
					flag = true;
				}
				break;
			}
			case SpawnManager.PosibleSpawnPoints.Teleport:
			{
				Teleport[] source = Object.FindObjectsOfType<Teleport>();
				Teleport teleport = source.FirstOrDefault((Teleport p) => p.telportName == this.spawnId);
				if (teleport)
				{
					position = teleport.transform.position;
					orientation = teleport.spawnOrientation;
					flag = true;
				}
				break;
			}
			case SpawnManager.PosibleSpawnPoints.Door:
			{
				Door[] source2 = Object.FindObjectsOfType<Door>();
				this.currentDoor = source2.FirstOrDefault((Door p) => p.identificativeName == this.spawnId);
				if (this.currentDoor)
				{
					position = ((!(this.currentDoor.spawnPoint != null)) ? this.currentDoor.transform.position : this.currentDoor.spawnPoint.position);
					orientation = this.currentDoor.exitOrientation;
					flag = true;
				}
				break;
			}
			case SpawnManager.PosibleSpawnPoints.SafePosition:
				flag = true;
				position = this.safePosition;
				orientation = EntityOrientation.Left;
				break;
			case SpawnManager.PosibleSpawnPoints.CustomPosition:
				flag = true;
				position = this.customPosition;
				orientation = this.customOrientation;
				break;
			case SpawnManager.PosibleSpawnPoints.TeleportPrieDieu:
				if (array.Length > 0)
				{
					PrieDieu prieDieu2 = array[0];
					position = prieDieu2.transform.position;
					orientation = prieDieu2.spawnOrientation;
					this.ActivePrieDieu = prieDieu2;
					flag = true;
				}
				break;
			case SpawnManager.PosibleSpawnPoints.Miriam:
			{
				MiriamStart[] array2 = Object.FindObjectsOfType<MiriamStart>();
				MiriamStart miriamStart = null;
				if (array2.Length > 0)
				{
					miriamStart = array2[0];
				}
				if (!miriamStart)
				{
					MiriamPortal[] array3 = Object.FindObjectsOfType<MiriamPortal>();
					if (array3 != null && array3.Length > 0)
					{
						position = array3[0].transform.position;
						orientation = array3[0].Orientation();
						flag = true;
					}
				}
				else
				{
					position = miriamStart.transform.position;
					orientation = miriamStart.spawnOrientation;
					flag = true;
				}
				break;
			}
			}
			if (!flag)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"SpawnManager: Pending spawn ",
					this.pendingSpawn.ToString(),
					" with id ",
					this.spawnId,
					"not found, trying spanw first PrieDeu or Debug or Zero"
				}));
				if (array.Length > 0)
				{
					position = array[0].transform.position;
					orientation = array[0].spawnOrientation;
				}
				else
				{
					DebugSpawn debugSpawn2 = Object.FindObjectOfType<DebugSpawn>();
					if (debugSpawn2)
					{
						position = debugSpawn2.transform.position;
						orientation = EntityOrientation.Right;
					}
				}
			}
			this.CreatePlayer(position, orientation, createNewInstance);
			if (Core.Events.GetFlag("CHERUB_RESPAWN"))
			{
				Core.Logic.Penitent.CherubRespawn();
			}
			if (this.FirstSpanw)
			{
				this.FirstSpanw = false;
				UIController.instance.UpdatePurgePoints();
				UIController.instance.UpdateGuiltLevel(true);
			}
		}

		private void LoadAllTeleports()
		{
			TeleportDestination[] source = Resources.LoadAll<TeleportDestination>("Teleport/");
			this.TeleportList = new List<TeleportDestination>(from x in source
			orderby x.order
			select x);
			this.TeleportDict = new Dictionary<string, TeleportDestination>();
			foreach (TeleportDestination teleportDestination in this.TeleportList)
			{
				teleportDestination.isActive = teleportDestination.activeAtStart;
				this.TeleportDict[teleportDestination.id] = teleportDestination;
			}
		}

		private void CreatePlayer(Vector3 position, EntityOrientation orientation, bool createNewInstance)
		{
			if (!this.penitentPrefab)
			{
				this.penitentPrefab = Resources.Load<Penitent>("Core/Penitent");
			}
			if (createNewInstance)
			{
				Core.Logic.Penitent = Object.Instantiate<Penitent>(this.penitentPrefab, position, Quaternion.identity);
			}
			else
			{
				Core.Logic.Penitent.transform.position = position;
			}
			if (this.HidePlayerInNextSpawn)
			{
				Core.Logic.Penitent.SpriteRenderer.enabled = false;
				Core.Logic.Penitent.DamageArea.enabled = false;
				Core.Logic.Penitent.Status.CastShadow = false;
				Core.Logic.Penitent.Physics.EnableColliders(false);
				Core.Logic.Penitent.Physics.EnablePhysics(false);
				this.HidePlayerInNextSpawn = false;
			}
			Core.Logic.Penitent.SetOrientation(orientation, true, false);
			if (SpawnManager.OnPlayerSpawn != null)
			{
				SpawnManager.OnPlayerSpawn(Core.Logic.Penitent);
			}
			SpawnManager.PosibleSpawnPoints posibleSpawnPoints = this.pendingSpawn;
			if (posibleSpawnPoints != SpawnManager.PosibleSpawnPoints.Teleport)
			{
				if (posibleSpawnPoints != SpawnManager.PosibleSpawnPoints.TeleportPrieDieu)
				{
					if (posibleSpawnPoints == SpawnManager.PosibleSpawnPoints.Door)
					{
						if (this.currentDoor)
						{
							Core.LevelManager.PendingrDoorToExit = this.currentDoor;
						}
					}
				}
				else if (SpawnManager.OnTeleportPrieDieu != null)
				{
					SpawnManager.OnTeleportPrieDieu(this.spawnId);
				}
			}
			else if (SpawnManager.OnTeleport != null)
			{
				SpawnManager.OnTeleport(this.spawnId);
			}
			this.currentDoor = null;
		}

		private void SpawnPlayer(string level, SpawnManager.PosibleSpawnPoints spawnType, string id, bool usefade = true, bool forceLoad = false, Color? background = null)
		{
			this.pendingSpawn = spawnType;
			this.spawnId = id;
			bool flag = level == Core.LevelManager.currentLevel.LevelName;
			if (spawnType == SpawnManager.PosibleSpawnPoints.Menu || (!forceLoad && flag))
			{
				if (spawnType != SpawnManager.PosibleSpawnPoints.Teleport)
				{
					this.SpawnPlayerOnLevelLoad(!flag);
				}
				else
				{
					Teleport[] source = Object.FindObjectsOfType<Teleport>();
					Teleport teleport = source.FirstOrDefault((Teleport p) => p.telportName == this.spawnId);
					if (teleport)
					{
						Core.Logic.Penitent.transform.position = teleport.transform.position;
						Core.Logic.Penitent.SetOrientation(teleport.spawnOrientation, true, false);
						if (SpawnManager.OnTeleport != null)
						{
							SpawnManager.OnTeleport(this.spawnId);
						}
					}
					else
					{
						Debug.LogWarning("** TELEPORT inside level " + this.spawnId + " Not found");
					}
				}
			}
			else
			{
				Core.LevelManager.ChangeLevel(level, false, usefade, forceLoad, background);
			}
		}

		public string GetPersistenID()
		{
			return "ID_CHECKPOINT_MANAGER";
		}

		public int GetOrder()
		{
			return 0;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			SpawnManager.CheckPointPersistenceData checkPointPersistenceData = new SpawnManager.CheckPointPersistenceData();
			if (StringExtensions.IsNullOrWhitespace(this.activePrieDieuScene))
			{
				Debug.LogWarning("PersistentManager get activePrieDieuScene, no scene yet");
				checkPointPersistenceData.activePrieDieuScene = this.InitialScene;
			}
			else
			{
				checkPointPersistenceData.activePrieDieuScene = this.activePrieDieuScene;
			}
			checkPointPersistenceData.activePrieDieuId = this.activePrieDieuId;
			Debug.Log(string.Format("<color=red> Active priedieu scene: {0}\n activePriedieuId{1}</color>", this.activePrieDieuScene, this.activePrieDieuId));
			checkPointPersistenceData.pendingSpawn = this.pendingSpawn;
			checkPointPersistenceData.spawnId = this.spawnId;
			foreach (TeleportDestination teleportDestination in this.TeleportList)
			{
				if (teleportDestination.isActive)
				{
					checkPointPersistenceData.activeTeleports.Add(teleportDestination.id);
				}
			}
			return checkPointPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			SpawnManager.CheckPointPersistenceData checkPointPersistenceData = (SpawnManager.CheckPointPersistenceData)data;
			this.activePrieDieuScene = checkPointPersistenceData.activePrieDieuScene;
			this.activePrieDieuId = checkPointPersistenceData.activePrieDieuId;
			if (isloading)
			{
				this.pendingSpawn = SpawnManager.PosibleSpawnPoints.PrieDieu;
				this.spawnId = this.activePrieDieuId;
				this.TeleportList.ForEach(delegate(TeleportDestination element)
				{
					element.isActive = element.activeAtStart;
				});
				foreach (string key in checkPointPersistenceData.activeTeleports)
				{
					if (this.TeleportDict.ContainsKey(key))
					{
						this.TeleportDict[key].isActive = true;
					}
				}
			}
			else
			{
				this.pendingSpawn = checkPointPersistenceData.pendingSpawn;
				this.spawnId = checkPointPersistenceData.spawnId;
				if (this.pendingSpawn == SpawnManager.PosibleSpawnPoints.TeleportPrieDieu)
				{
					PrieDieu[] array = Object.FindObjectsOfType<PrieDieu>();
					if (array.Length > 0)
					{
						this.ActivePrieDieu = array[0];
					}
				}
			}
		}

		public void ResetPersistence()
		{
			this.activePrieDieuScene = string.Empty;
			this.activePrieDieuId = string.Empty;
			this.pendingSpawn = SpawnManager.PosibleSpawnPoints.None;
			this.spawnId = string.Empty;
			this.LoadAllTeleports();
		}

		public static List<string> GetAllTeleportsId()
		{
			if (SpawnManager.forceReload || SpawnManager.cachedTeleportId.Count == 0)
			{
				SpawnManager.cachedTeleportId.Clear();
				TeleportDestination[] source = Resources.LoadAll<TeleportDestination>("Teleport/");
				foreach (TeleportDestination teleportDestination in from x in source
				orderby x.order
				select x)
				{
					SpawnManager.cachedTeleportId.Add(teleportDestination.id);
				}
				SpawnManager.cachedTeleportId.Sort();
			}
			SpawnManager.forceReload = false;
			return SpawnManager.cachedTeleportId;
		}

		private const string TELEPORT_RESOURCE_DIR = "Teleport/";

		private const string PENITENT_PREFAB = "Core/Penitent";

		private string activePrieDieuScene = string.Empty;

		private string activePrieDieuId = string.Empty;

		private SpawnManager.PosibleSpawnPoints pendingSpawn;

		private string spawnId = string.Empty;

		private Vector3 safePosition;

		private List<TeleportDestination> TeleportList;

		private Dictionary<string, TeleportDestination> TeleportDict;

		private Penitent penitentPrefab;

		private Door currentDoor;

		public bool FirstSpanw;

		private string customLevel = string.Empty;

		private Vector3 customPosition = Vector3.zero;

		private EntityOrientation customOrientation = EntityOrientation.Left;

		private bool customIsMiriam;

		public const string CHECK_PERSITENT_ID = "ID_CHECKPOINT_MANAGER";

		private static bool forceReload = true;

		private static List<string> cachedTeleportId = new List<string>();

		public delegate void SpawnEvent(Penitent penitent);

		public delegate void TeleportEvent(string spawnId);

		public enum PosibleSpawnPoints
		{
			None,
			Menu,
			Editor,
			PrieDieu,
			Teleport,
			Door,
			SafePosition,
			CustomPosition,
			TeleportPrieDieu,
			Miriam
		}

		[Serializable]
		public class CheckPointPersistenceData : PersistentManager.PersistentData
		{
			public CheckPointPersistenceData() : base("ID_CHECKPOINT_MANAGER")
			{
			}

			public string activePrieDieuScene = string.Empty;

			public string activePrieDieuId = string.Empty;

			public SpawnManager.PosibleSpawnPoints pendingSpawn;

			public string spawnId = string.Empty;

			public List<string> activeTeleports = new List<string>();
		}
	}
}
