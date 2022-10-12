using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Framework.Dialog;
using Framework.FrameworkCore;
using Gameplay.UI;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Managers
{
	public class EventManager : GameSystem, PersistentInterface
	{
		public event EventManager.StandardEvent OnEventLaunched;

		public event EventManager.StandardFlag OnFlagChanged;

		public GameObject LastCreatedObject { get; private set; }

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			LevelManager.OnLevelPreLoaded += this.OnLevelPreLoaded;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		public override void Start()
		{
			this.ResetPersistence();
		}

		public override void Dispose()
		{
			base.Dispose();
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
			LevelManager.OnLevelPreLoaded -= this.OnLevelPreLoaded;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		public void LaunchEvent(string id, string parameter = "")
		{
			if (!id.IsNullOrWhitespace() && this.OnEventLaunched != null)
			{
				string text = id.Replace(' ', '_').ToUpper();
				string text2 = parameter.Replace(' ', '_').ToUpper();
				if (parameter.IsNullOrWhitespace())
				{
					text2 = "NO_PARAMS";
				}
				Log.Trace("Events", string.Concat(new string[]
				{
					"Event ",
					text,
					" with parameter ",
					text2,
					" has been raised."
				}), null);
				this.OnEventLaunched(text, text2);
			}
		}

		public void SetFlag(string id, bool b, bool forcePreserve = false)
		{
			if (string.IsNullOrEmpty(id))
			{
				return;
			}
			if (id != "REVEAL_FAITH_PLATFORMS")
			{
				Core.Randomizer.Log(id + ": " + b.ToString(), 1);
			}
			string formattedId = this.GetFormattedId(id);
			if (!this.flags.ContainsKey(formattedId))
			{
				FlagObject flagObject = new FlagObject();
				flagObject.id = formattedId;
				flagObject.preserveInNewGamePlus = forcePreserve;
				this.flags[formattedId] = flagObject;
				Debug.Log("-- Create flag " + id);
			}
			this.flags[formattedId].value = b;
			if (forcePreserve && !this.flags[formattedId].preserveInNewGamePlus)
			{
				Debug.LogWarning("-- Created flag " + id + " set force to preserve");
				this.flags[formattedId].preserveInNewGamePlus = forcePreserve;
			}
			Log.Trace("Events", string.Concat(new string[]
			{
				"Flag ",
				formattedId,
				" has been set to ",
				b.ToString().ToUpper(),
				"."
			}), null);
			if (this.OnFlagChanged != null)
			{
				this.OnFlagChanged(formattedId, b);
			}
			if (b && this.flags[formattedId].addToPercentage)
			{
				Core.AchievementsManager.CheckProgressToAC46();
			}
		}

		public bool GetFlag(string id)
		{
			string formattedId = this.GetFormattedId(id);
			bool result = false;
			if (this.flags.ContainsKey(formattedId))
			{
				result = this.flags[formattedId].value;
			}
			return result;
		}

		public string GetFormattedId(string id)
		{
			return id.Replace(' ', '_').ToUpper();
		}

		public void PrepareForNewGamePlus()
		{
			foreach (KeyValuePair<string, FlagObject> keyValuePair in this.flags)
			{
				if (keyValuePair.Value.value && !keyValuePair.Value.preserveInNewGamePlus)
				{
					Debug.Log("Reset flag for ng+ " + keyValuePair.Key);
					keyValuePair.Value.value = false;
				}
				if (keyValuePair.Value.value && keyValuePair.Value.preserveInNewGamePlus)
				{
					Debug.Log("Preserve flag for ng+ " + keyValuePair.Key);
				}
			}
			this.IsMiriamQuestStarted = false;
			this.IsMiriamQuestFinished = false;
			this.MiriamClosedPortals = new List<string>();
			this.MiriamCurrentScenePortal = string.Empty;
			this.MiriamCurrentScenePortalToReturn = string.Empty;
			this.MiriamCurrentSceneDestination = string.Empty;
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			if (Core.LevelManager.InCinematicsChangeLevel == LevelManager.CinematicsChangeLevel.No)
			{
				PlayMakerFSM.BroadcastEvent("ON LEVEL UNLOAD");
			}
		}

		private void OnLevelPreLoaded(Level oldLevel, Level newLevel)
		{
			if (Core.LevelManager.InCinematicsChangeLevel == LevelManager.CinematicsChangeLevel.No)
			{
				PlayMakerFSM.BroadcastEvent("ON LEVEL LOADED");
			}
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.CinematicsChangeLevel inCinematicsChangeLevel = Core.LevelManager.InCinematicsChangeLevel;
			if (inCinematicsChangeLevel != LevelManager.CinematicsChangeLevel.No)
			{
				if (inCinematicsChangeLevel == LevelManager.CinematicsChangeLevel.Outro)
				{
					PlayMakerFSM.BroadcastEvent("POST CUTSCENE");
					return;
				}
			}
			else
			{
				PlayMakerFSM.BroadcastEvent("ON LEVEL READY");
			}
		}

		public float GetPercentageCompletition()
		{
			float num = 0f;
			foreach (FlagObject flagObject in from x in this.flags.Values
			where x.addToPercentage && x.value
			select x)
			{
				num += GameConstants.PercentageValues[flagObject.percentageType];
			}
			return num;
		}

		public bool IsMiriamQuestStarted { get; private set; }

		public bool IsMiriamQuestFinished { get; private set; }

		public string MiriamCurrentScenePortal { get; private set; }

		public string MiriamCurrentScenePortalToReturn { get; private set; }

		public string MiriamCurrentSceneDestination { get; private set; }

		public bool StartMiriamQuest()
		{
			bool result = false;
			if (this.IsMiriamQuestStarted)
			{
				Debug.LogError("*** StartMiriamQuest. quest was started");
			}
			else if (this.IsMiriamQuestFinished)
			{
				Debug.LogError("*** StartMiriamQuest. quest was finished");
			}
			else
			{
				this.IsMiriamQuestStarted = true;
				result = true;
			}
			return result;
		}

		public bool FinishMiriamQuest()
		{
			bool result = false;
			if (this.CheckStatusMiriamQuest())
			{
				this.IsMiriamQuestFinished = true;
				result = true;
			}
			return result;
		}

		public ReadOnlyCollection<string> GetMiriamClosedPortals()
		{
			return this.MiriamClosedPortals.AsReadOnly();
		}

		public bool ActivateMiriamPortalAndTeleport(bool UseFade = true)
		{
			bool result = false;
			if (this.CheckStatusMiriamQuest())
			{
				if (!this.MiriamCurrentScenePortal.IsNullOrWhitespace())
				{
					Debug.LogWarning("ActivateMiriamPortalAndTeleport and have current portal " + this.MiriamCurrentScenePortal);
				}
				int count = this.MiriamClosedPortals.Count;
				if (count >= this.MiriamQuest.Scenes.Count)
				{
					Debug.LogError("ActivateMiriamPortalAndTeleport no more portals in quest");
				}
				else if (this.MiriamClosedPortals.Contains(Core.LevelManager.currentLevel.LevelName))
				{
					Debug.LogError("ActivateMiriamPortalAndTeleport " + Core.LevelManager.currentLevel.LevelName + " is closed.");
				}
				else
				{
					this.MiriamCurrentScenePortal = Core.LevelManager.currentLevel.LevelName;
					this.MiriamCurrentScenePortalToReturn = this.MiriamCurrentScenePortal;
					GameModeManager gameModeManager = Core.GameModeManager;
					gameModeManager.OnEnterMenuMode = (Core.SimpleEvent)Delegate.Combine(gameModeManager.OnEnterMenuMode, new Core.SimpleEvent(this.ClearMiriamChallengeSpawn));
					result = true;
					this.MiriamCurrentSceneDestination = this.MiriamQuest.Scenes[count];
					Core.SpawnManager.SpawnFromMiriam(this.MiriamCurrentSceneDestination, string.Empty, UseFade);
				}
			}
			return result;
		}

		private void ClearMiriamChallengeSpawn()
		{
			GameModeManager gameModeManager = Core.GameModeManager;
			gameModeManager.OnEnterMenuMode = (Core.SimpleEvent)Delegate.Remove(gameModeManager.OnEnterMenuMode, new Core.SimpleEvent(this.ClearMiriamChallengeSpawn));
			this.MiriamCurrentScenePortal = string.Empty;
			this.MiriamCurrentScenePortalToReturn = string.Empty;
			this.MiriamCurrentSceneDestination = string.Empty;
		}

		public bool EndMiriamPortalAndReturn(bool useFade = true)
		{
			bool result = false;
			if (this.CheckStatusMiriamQuest())
			{
				if (!this.AreInMiriamLevel())
				{
					Debug.LogError("EndThisActivatePortalAndGetScene and don't have current portal");
				}
				else
				{
					string miriamCurrentScenePortal = this.MiriamCurrentScenePortal;
					if (this.MiriamClosedPortals.Contains(this.MiriamCurrentScenePortal))
					{
						Debug.LogError("EndThisActivatePortalAndGetScene: Portal " + this.MiriamCurrentScenePortal + " was closed and want to close again");
					}
					else
					{
						this.MiriamClosedPortals.Add(this.MiriamCurrentScenePortal);
					}
					if (this.MiriamClosedPortals.Count == this.MiriamQuest.Scenes.Count)
					{
						this.FinishMiriamQuest();
					}
					result = true;
					this.MiriamCurrentScenePortal = string.Empty;
					this.MiriamCurrentSceneDestination = string.Empty;
					this.SetFlag(this.MiriamQuest.CutsceneFlag, true, false);
					string text = this.MiriamQuest.PortalFlagPrefix + this.MiriamClosedPortals.Count;
					this.SetFlag(text, true, false);
					this.SetFlag(text + this.MiriamQuest.PortalFlagSufix, true, false);
					UIController.instance.HideMiriamTimer();
					Core.LevelManager.ChangeLevelAndPlayEvent(this.MiriamQuest.PortalSceneName, this.MiriamQuest.PortalPlaymakerEventName, true, useFade, false, null, true);
				}
			}
			return result;
		}

		public bool TeleportPenitentToGoal()
		{
			bool result = true;
			if (this.CheckStatusMiriamQuest())
			{
				if (!this.AreInMiriamLevel())
				{
					Debug.LogError("TeleportPenitentToGoal and we are not in Miriam Level!");
					result = false;
				}
				else
				{
					Vector2 position = GameObject.Find("GoalPoint").transform.position;
					Core.Logic.Penitent.Teleport(position);
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool CancelMiriamPortalAndReturn(bool useFade = true)
		{
			int num = 0;
			if (this.CheckStatusMiriamQuest())
			{
				if (!this.AreInMiriamLevel())
				{
					Debug.LogError("CancelMiriamPortalAndReturn and don't have current portal");
					return num != 0;
				}
				string miriamCurrentScenePortalToReturn = this.MiriamCurrentScenePortalToReturn;
				this.MiriamCurrentScenePortal = string.Empty;
				this.MiriamCurrentSceneDestination = string.Empty;
				UIController.instance.HideMiriamTimer();
				Core.SpawnManager.SpawnFromMiriam(miriamCurrentScenePortalToReturn, string.Empty, useFade);
			}
			return num != 0;
		}

		public bool IsMiriamPortalEnabled(string levelName)
		{
			bool result = false;
			if (this.IsMiriamQuestStarted && !this.IsMiriamQuestFinished)
			{
				result = !this.MiriamClosedPortals.Contains(levelName);
			}
			return result;
		}

		public bool AreInMiriamLevel()
		{
			return this.IsMiriamQuestStarted && !this.MiriamCurrentScenePortal.IsNullOrWhitespace();
		}

		private bool CheckStatusMiriamQuest()
		{
			bool result = false;
			if (!this.IsMiriamQuestStarted)
			{
				Debug.LogError("*** MiriamQuest. quest was not started");
			}
			else if (this.IsMiriamQuestFinished)
			{
				Debug.LogError("*** EndMiriamQuest. quest was finished");
			}
			else
			{
				result = true;
			}
			return result;
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_EVENT_MANAGER";
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			EventManager.FlagPersistenceData flagPersistenceData = new EventManager.FlagPersistenceData();
			foreach (KeyValuePair<string, FlagObject> keyValuePair in this.flags)
			{
				flagPersistenceData.flags.Add(keyValuePair.Key, keyValuePair.Value.value);
				flagPersistenceData.flagsPreserve.Add(keyValuePair.Key, keyValuePair.Value.preserveInNewGamePlus);
			}
			flagPersistenceData.IsMiriamQuestFinished = this.IsMiriamQuestFinished;
			flagPersistenceData.IsMiriamQuestStarted = this.IsMiriamQuestStarted;
			flagPersistenceData.MiriamClosedPortals = new List<string>(this.MiriamClosedPortals);
			return flagPersistenceData;
		}

		private bool FlagNeedToBePersistent(string id)
		{
			bool flag = false;
			foreach (string value in GameConstants.FLAGS_ENDINGS_NEED_TO_BE_PERSISTENT)
			{
				if (id.EndsWith(value))
				{
					flag = true;
					break;
				}
			}
			flag = (flag && !GameConstants.IGNORE_FLAG_TO_BE_PERSISTENT.Contains(id));
			return flag;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			EventManager.FlagPersistenceData flagPersistenceData = (EventManager.FlagPersistenceData)data;
			this.flags.ForEach(delegate(KeyValuePair<string, FlagObject> p)
			{
				p.Value.value = false;
			});
			foreach (KeyValuePair<string, bool> keyValuePair in flagPersistenceData.flags)
			{
				if (this.flags.ContainsKey(keyValuePair.Key))
				{
					this.flags[keyValuePair.Key].value = keyValuePair.Value;
				}
				else
				{
					FlagObject flagObject = new FlagObject();
					flagObject.id = keyValuePair.Key;
					flagObject.value = keyValuePair.Value;
					if (flagPersistenceData.flagsPreserve.ContainsKey(keyValuePair.Key))
					{
						flagObject.preserveInNewGamePlus = flagPersistenceData.flagsPreserve[keyValuePair.Key];
					}
					if (!flagObject.preserveInNewGamePlus && this.FlagNeedToBePersistent(keyValuePair.Key))
					{
						flagObject.preserveInNewGamePlus = true;
					}
					this.flags[keyValuePair.Key] = flagObject;
				}
			}
			if (isloading)
			{
				this.IsMiriamQuestFinished = flagPersistenceData.IsMiriamQuestFinished;
				this.IsMiriamQuestStarted = flagPersistenceData.IsMiriamQuestStarted;
				this.MiriamClosedPortals = new List<string>(flagPersistenceData.MiriamClosedPortals);
			}
		}

		public void ResetPersistence()
		{
			this.flags.Clear();
			Log.Trace("Events", "The event manager has been reseted sucessfully.", null);
			foreach (FlagObjectList flagObjectList in Resources.LoadAll<FlagObjectList>("Dialog/"))
			{
				foreach (FlagObject flagObject in flagObjectList.flagList)
				{
					if (this.flags.ContainsKey(flagObject.id))
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Duplicate Flag ",
							flagObject.id,
							" List:",
							this.flags[flagObject.id].sourceList,
							", ",
							flagObjectList.name
						}));
					}
					else
					{
						FlagObject flagObject2 = new FlagObject(flagObject);
						this.flags[flagObject2.id] = flagObject2;
					}
				}
			}
			this.MiriamQuest = Resources.Load<MiriamsConfig>("Dialog/ST205_MIRIAM/MiriamQuest");
			Debug.Log(" Miriam Quest scenes " + this.MiriamQuest.Scenes.Count);
			this.IsMiriamQuestFinished = false;
			this.IsMiriamQuestStarted = false;
			this.MiriamClosedPortals = new List<string>();
			Log.Debug("EventManager", this.flags.Count.ToString() + " initial flags loaded succesfully.", null);
		}

		private Dictionary<string, FlagObject> flags = new Dictionary<string, FlagObject>();

		private const string MIRIAM_QUEST_CONFIG = "Dialog/ST205_MIRIAM/MiriamQuest";

		private MiriamsConfig MiriamQuest;

		private List<string> MiriamClosedPortals = new List<string>();

		public const string EVENT_PERSITENT_ID = "ID_EVENT_MANAGER";

		public delegate void StandardEvent(string id, string parameter);

		public delegate void StandardFlag(string flag, bool flagActive);

		[Serializable]
		public class FlagPersistenceData : PersistentManager.PersistentData
		{
			public FlagPersistenceData() : base("ID_EVENT_MANAGER")
			{
			}

			public Dictionary<string, bool> flags = new Dictionary<string, bool>();

			public Dictionary<string, bool> flagsPreserve = new Dictionary<string, bool>();

			public bool IsMiriamQuestStarted;

			public bool IsMiriamQuestFinished;

			public List<string> MiriamClosedPortals = new List<string>();
		}
	}
}
