using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Framework.FrameworkCore;
using Framework.Map;
using Framework.Penitences;
using Framework.Randomizer;
using FullSerializer;
using Gameplay.GameControllers.Entities;
using Sirenix.Utilities;
using Steamworks;
using Tools;
using UnityEngine;

namespace Framework.Managers
{
	public class PersistentManager : GameSystem, PersistentInterface
	{
		private float LastTimeStored { get; set; }

		private float TimeStored { get; set; }

		private float TimeAtSlotAscension { get; set; }

		public override void Initialize()
		{
			this.currentSnapshot = new PersistentManager.SnapShot();
			this.persistenceSystems.Clear();
			this.serializer = new fsSerializer();
			this.LastTimeStored = 0f;
			this.ResetPersistence();
			this.AddPersistentManager(this);
		}

		public void AddPersistentManager(PersistentInterface manager)
		{
			if (!this.persistenceSystems.Contains(manager))
			{
				this.persistenceSystems.Add(manager);
			}
		}

		public void ResetAll()
		{
			Log.Debug("[Persistence]", "Reset all", null);
			this.currentSnapshot.Clear();
			foreach (PersistentInterface persistentInterface in from x in this.persistenceSystems
			orderby x.GetOrder()
			select x)
			{
				persistentInterface.ResetPersistence();
			}
			if (Core.Logic.Penitent)
			{
				Core.Logic.Penitent.Stats.ResetPersistence();
			}
		}

		public void ResetCurrent()
		{
			Log.Debug("Persistence", "ResetCurrent - Reseting data on \"" + Core.LevelManager.currentLevel.LevelName + "\".", null);
			this.LoadSnapShot(this.currentSnapshot, Core.LevelManager.currentLevel.LevelName, false, this.CreateAndGetSaveGameInternalDir(PersistentManager.GetAutomaticSlot()));
		}

		public void RestoreStored()
		{
			Log.Debug("[Persistence]", "RestoreStored", null);
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			if (Core.Alms.GetPrieDieuLevel() > 1)
			{
				Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			}
			else
			{
				Core.Logic.Penitent.Stats.Fervour.Current = 0f;
			}
			Core.InventoryManager.ResetObjectsEffects();
			this.SaveGame(true);
		}

		public void PrepareForNewGamePlus()
		{
			this.currentSnapshot.sceneElements.Clear();
			this.TimeAtSlotAscension = this.GetCurrentTimePlayed();
		}

		public void DEBUG_SaveAllData(int slot, int mapReveals, int elementPerScene)
		{
			Core.GuiltManager.DEBUG_AddAllGuilts();
			Core.InventoryManager.TestAddAllObjects();
			foreach (string scene in Core.LevelManager.DEBUG_GetAllLevelsName())
			{
				string sceneKeyName = this.GetSceneKeyName(scene);
				if (this.currentSnapshot.sceneElements.ContainsKey(sceneKeyName))
				{
					this.currentSnapshot.sceneElements[sceneKeyName].Clear();
				}
				this.currentSnapshot.sceneElements[sceneKeyName] = new Dictionary<string, PersistentManager.PersistentData>();
				for (int i = 0; i < elementPerScene; i++)
				{
					string text = string.Concat(new object[]
					{
						sceneKeyName,
						"_",
						i,
						this.ToString(),
						"_TEST_ID_VERY_LONG_ID_LIKE_EVERY_IDS"
					});
					PersistentManager.DEBUGPersistence value = new PersistentManager.DEBUGPersistence(text);
					this.currentSnapshot.sceneElements[sceneKeyName][text] = value;
				}
			}
			this.SaveGame(slot, true);
		}

		public void SaveGame(int slot, bool fullSave = true)
		{
			Debug.Log("**** Persistent: SAVEGAME");
			PersistentManager.SetAutomaticSlot(slot);
			if (PersistentManager.GetAutomaticSlot() == -1)
			{
				Debug.LogError("Trying to save slot without initializing");
				return;
			}
			string dataPath = this.CreateAndGetSaveGameInternalDir(slot);
			this.SaveSnapShot(this.currentSnapshot, Core.LevelManager.currentLevel.LevelName, fullSave, dataPath);
		}

		public void SaveGame(bool fullSave = true)
		{
			this.SaveGame(PersistentManager.GetAutomaticSlot(), fullSave);
		}

		public bool DeleteSaveGame(int slot)
		{
			bool result = false;
			if (this.ExistSlot(slot))
			{
				this.DeleteSaveGame_Internal(this.GetSaveGameFile(slot), this.GetSaveGameInternalName(slot));
				result = true;
			}
			if (this.ExistBackupSlot(slot))
			{
				this.DeleteSaveGame_Internal(this.GetSaveGameBackupFile(slot), this.GetSaveGameBackupInternalName(slot));
				result = true;
			}
			return result;
		}

		public bool LoadGame(int slot)
		{
			PersistentManager.SetAutomaticSlot(slot);
			bool flag = this.LoadGameWithOutRespawn(slot);
			if (flag)
			{
				Core.Logic.EnemySpawner.Reset();
				Core.SpawnManager.FirstSpanw = true;
				Core.SpawnManager.Respawn();
			}
			return flag;
		}

		public bool LoadGameWithOutRespawn(int slot)
		{
			Debug.Log("**** Persistent: LOADGAME");
			Core.Logic.ResetAllData();
			bool flag = this.LoadGameSnapShot(slot, ref this.currentSnapshot);
			PersistentManager.PublicSlotData slotData = Core.Persistence.GetSlotData(slot);
			if ((!flag || slotData.persistence.Corrupted) && this.ExistBackupSlot(slot))
			{
				Debug.LogWarning("Loading backup for corrupted slot " + slot);
				this.RestoreBackup(slot);
				flag = this.LoadGameSnapShot(slot, ref this.currentSnapshot);
			}
			if (flag)
			{
				this.BackupSlot(slot);
				this.LoadSnapShot(this.currentSnapshot, string.Empty, true, this.CreateAndGetSaveGameInternalDir(slot));
			}
			return flag;
		}

		public bool ExistSlot(int slot)
		{
			return File.Exists(this.GetSaveGameFile(slot));
		}

		public bool ExistBackupSlot(int slot)
		{
			return File.Exists(this.GetSaveGameBackupFile(slot));
		}

		public PersistentManager.PublicSlotData GetSlotData(int slot)
		{
			PersistentManager.PublicSlotData publicSlotData = null;
			PersistentManager.SnapShot snapShot = new PersistentManager.SnapShot();
			bool corrupted = false;
			bool flag = this.LoadGameSnapShot(slot, ref snapShot);
			bool flag2 = this.ExistBackupSlot(slot);
			if (!flag && flag2)
			{
				flag = this.LoadGameBackupSnapShot(slot, ref snapShot);
				corrupted = true;
			}
			if (flag)
			{
				publicSlotData = new PersistentManager.PublicSlotData();
				if (snapShot.commonElements.ContainsKey(this.GetPersistenID()))
				{
					publicSlotData.persistence = (PersistentManager.PersitentPersistenceData)snapShot.commonElements[this.GetPersistenID()];
					ZoneKey sceneKey = new ZoneKey(publicSlotData.persistence.CurrentDomain, publicSlotData.persistence.CurrentZone, string.Empty);
					if (!Core.NewMapManager.ZoneHasName(sceneKey))
					{
						SpawnManager.CheckPointPersistenceData checkPointPersistenceData = (SpawnManager.CheckPointPersistenceData)snapShot.commonElements["ID_CHECKPOINT_MANAGER"];
						if (!checkPointPersistenceData.activePrieDieuScene.IsNullOrWhitespace())
						{
							publicSlotData.persistence.CurrentDomain = checkPointPersistenceData.activePrieDieuScene.Substring(0, 3);
							publicSlotData.persistence.CurrentZone = checkPointPersistenceData.activePrieDieuScene.Substring(3, 3);
						}
						else
						{
							corrupted = true;
						}
					}
					publicSlotData.persistence.CanConvertToNewGamePlus = Core.GameModeManager.CanConvertToNewGamePlus(snapShot);
					publicSlotData.persistence.Corrupted = corrupted;
					publicSlotData.persistence.HasBackup = flag2;
					string persistenID = Core.Randomizer.GetPersistenID();
					if (snapShot.commonElements.ContainsKey(persistenID) && ((RandomizerPersistenceData)snapShot.commonElements[persistenID]).startedInRando)
					{
						PersistentManager.PersitentPersistenceData persistence = publicSlotData.persistence;
						persistence.CurrentZone += "r";
					}
				}
				string persistenID2 = Core.PenitenceManager.GetPersistenID();
				if (snapShot.commonElements.ContainsKey(persistenID2))
				{
					publicSlotData.penitence = (PenitenceManager.PenitencePersistenceData)snapShot.commonElements[persistenID2];
				}
				persistenID2 = Core.Events.GetPersistenID();
				if (snapShot.commonElements.ContainsKey(persistenID2))
				{
					publicSlotData.flags = (EventManager.FlagPersistenceData)snapShot.commonElements[persistenID2];
				}
				persistenID2 = Core.AchievementsManager.GetPersistenID();
				if (snapShot.commonElements.ContainsKey(persistenID2))
				{
					publicSlotData.achievement = (AchievementsManager.AchievementPersistenceData)snapShot.commonElements[persistenID2];
				}
			}
			else if (this.ExistSlot(slot))
			{
				publicSlotData = new PersistentManager.PublicSlotData();
				publicSlotData.persistence = new PersistentManager.PersitentPersistenceData();
				publicSlotData.persistence.Corrupted = true;
			}
			return publicSlotData;
		}

		public static int GetAutomaticSlot()
		{
			return PersistentManager.CurrentSaveSlot;
		}

		public static void SetAutomaticSlot(int slot)
		{
			PersistentManager.CurrentSaveSlot = slot;
		}

		public static void ResetAutomaticSlot()
		{
			PersistentManager.CurrentSaveSlot = -1;
		}

		public void HACK_EnableNewGamePlusInCurrent()
		{
			string text = "d07z01s03";
			if (!this.currentSnapshot.sceneElements.ContainsKey(text))
			{
				this.currentSnapshot.sceneElements[text] = new Dictionary<string, PersistentManager.PersistentData>();
			}
			string text2 = text + "__TEST_ID_VERY_LONG_ID_LIKE_EVERY_IDS";
			PersistentManager.DEBUGPersistence value = new PersistentManager.DEBUGPersistence(text2);
			this.currentSnapshot.sceneElements[text][text2] = value;
		}

		public void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			if (oldLevel == null || oldLevel.LevelName.Equals("MainMenu") || oldLevel.LevelName.Equals("D07Z01S04"))
			{
				return;
			}
			Debug.Log("**** PERSITENT EVENT: OnBeforeLevelLoad " + oldLevel.LevelName);
			if (PersistentManager.GetAutomaticSlot() == -1)
			{
				Debug.LogError("Trying to save slot without initializing");
				return;
			}
			this.SaveSnapShot(this.currentSnapshot, oldLevel.LevelName, false, this.CreateAndGetSaveGameInternalDir(PersistentManager.GetAutomaticSlot()));
			this.InsideLoadLevelProcess = true;
			Log.Trace("Persistence", "Persistence SAVE: " + oldLevel.LevelName, null);
		}

		public void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.CheckAndApplyBeadFix();
			this.InsideLoadLevelProcess = false;
			if (newLevel == null || newLevel.LevelName.Equals("MainMenu"))
			{
				return;
			}
			Debug.Log("**** PERSITENT EVENT: OnLevelLoaded " + newLevel.LevelName);
			this.LoadSnapShot(this.currentSnapshot, newLevel.LevelName, false, this.CreateAndGetSaveGameInternalDir(PersistentManager.GetAutomaticSlot()));
		}

		public float PercentCompleted
		{
			get
			{
				float num = Core.InventoryManager.GetPercentageCompletition();
				num += Core.Events.GetPercentageCompletition();
				EntityStats stats = Core.Logic.Penitent.Stats;
				float num2 = (float)(stats.Life.GetUpgrades() + stats.Fervour.GetUpgrades() + stats.MeaCulpa.GetUpgrades()) * GameConstants.PercentageValues[PersistentManager.PercentageType.Upgraded];
				num += num2;
				num += Core.NewMapManager.GetPercentageCompletition();
				num += Core.SpawnManager.GetPercentageCompletition();
				if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS))
				{
					float percentageCompletition = Core.PenitenceManager.GetPercentageCompletition();
					num += percentageCompletition;
				}
				return num;
			}
		}

		private void DeleteSaveGame_Internal(string file, string internalDir)
		{
			if (File.Exists(file))
			{
				File.Delete(file);
			}
			if (!internalDir.EndsWith("/"))
			{
				internalDir += "/";
			}
			if (Directory.Exists(internalDir))
			{
				Directory.Delete(internalDir, true);
			}
		}

		private string GetSaveGameBackupInternalName(int slot)
		{
			return this.GetSaveGameInternalName(slot) + "_backup";
		}

		private string GetSaveGameBackupFile(int slot)
		{
			return this.GetSaveGameFile(slot) + "_backup";
		}

		private bool BackupSlot(int slot)
		{
			if (!this.ExistSlot(slot))
			{
				return false;
			}
			try
			{
				File.Copy(this.GetSaveGameFile(slot), this.GetSaveGameBackupFile(slot), true);
				string saveGameInternalName = this.GetSaveGameInternalName(slot);
				string saveGameBackupInternalName = this.GetSaveGameBackupInternalName(slot);
				FileTools.DirectoryCopy(saveGameInternalName, saveGameBackupInternalName, true, true);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private bool RestoreBackup(int slot)
		{
			if (!this.ExistBackupSlot(slot))
			{
				return false;
			}
			try
			{
				File.Copy(this.GetSaveGameBackupFile(slot), this.GetSaveGameFile(slot), true);
				string saveGameInternalName = this.GetSaveGameInternalName(slot);
				FileTools.DirectoryCopy(this.GetSaveGameBackupInternalName(slot), saveGameInternalName, true, true);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private bool LoadGameSnapShot(int slot, ref PersistentManager.SnapShot snap)
		{
			if (!this.ExistSlot(slot))
			{
				return false;
			}
			string saveGameFile = this.GetSaveGameFile(slot);
			return this.LoadGameSnapShotInternal(saveGameFile, ref snap);
		}

		private bool LoadGameBackupSnapShot(int slot, ref PersistentManager.SnapShot snap)
		{
			if (!this.ExistBackupSlot(slot))
			{
				return false;
			}
			string saveGameBackupFile = this.GetSaveGameBackupFile(slot);
			return this.LoadGameSnapShotInternal(saveGameBackupFile, ref snap);
		}

		private bool LoadGameSnapShotInternal(string path, ref PersistentManager.SnapShot snap)
		{
			bool flag = true;
			string input = string.Empty;
			try
			{
				byte[] bytes = Convert.FromBase64String(File.ReadAllText(path));
				input = Encoding.UTF8.GetString(bytes);
			}
			catch (Exception)
			{
				flag = false;
			}
			if (flag)
			{
				fsData data;
				fsResult fsResult = fsJsonParser.Parse(input, out data);
				if (fsResult.Failed)
				{
					Debug.LogError("** LoadGame parsing error: " + fsResult.FormattedMessages);
					flag = false;
				}
				else
				{
					try
					{
						fsResult = this.serializer.TryDeserialize<PersistentManager.SnapShot>(data, ref snap);
					}
					catch (Exception ex)
					{
						Debug.LogError("** LoadGame deserialization exception: " + ex.Message);
						flag = false;
					}
					finally
					{
						if (fsResult.Failed)
						{
							Debug.LogError("** LoadGame deserialization error: " + fsResult.FormattedMessages);
							flag = false;
						}
					}
				}
			}
			return flag;
		}

		private bool SaveGame_Internal(int slot)
		{
			string saveGameFile = this.GetSaveGameFile(slot);
			Debug.Log(string.Concat(new object[]
			{
				"* SaveGame, saving slot ",
				slot,
				" to file ",
				saveGameFile
			}));
			fsData data;
			fsResult fsResult = this.serializer.TrySerialize<PersistentManager.SnapShot>(this.currentSnapshot, out data);
			if (fsResult.Failed)
			{
				Debug.LogError("** SaveGame error: " + fsResult.FormattedMessages);
				return false;
			}
			string s = fsJsonPrinter.CompressedJson(data);
			string encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
			FileTools.SaveSecure(saveGameFile, encryptedData);
			return true;
		}

		private string CreateAndGetSaveGameInternalDir(int slot)
		{
			string text = this.GetSaveGameInternalName(slot) + "/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		private string GetSaveGameInternalName(int slot)
		{
			return PersistentManager.GetPathAppSettings("savegame_" + slot);
		}

		private string GetSaveGameFile(int slot)
		{
			return this.GetSaveGameInternalName(slot) + ".save";
		}

		private void SaveSnapShot(PersistentManager.SnapShot snapShot, string scene, bool fullSave, string dataPath)
		{
			if (this.InsideLoadLevelProcess)
			{
				Debug.LogError("*** SAVEERROR 1. SAVE INSIDE LOAD LEVEL PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
				return;
			}
			if (this.InsideSaveProcess)
			{
				Debug.LogError("*** SAVEERROR 2. SAVE INSIDE SAVE PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
				return;
			}
			if (this.InsideLoadProcess)
			{
				Debug.LogError("*** SAVEERROR 3. SAVE INSIDE LOAD PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
				return;
			}
			this.InsideSaveProcess = true;
			string sceneKeyName = this.GetSceneKeyName(scene);
			if (snapShot.sceneElements.ContainsKey(sceneKeyName))
			{
				snapShot.sceneElements[sceneKeyName].Clear();
			}
			else
			{
				snapShot.sceneElements[sceneKeyName] = new Dictionary<string, PersistentManager.PersistentData>();
			}
			foreach (PersistentObject persistentObject in UnityEngine.Object.FindObjectsOfType<PersistentObject>())
			{
				if (!persistentObject.IsIgnoringPersistence())
				{
					try
					{
						PersistentManager.PersistentData currentPersistentState = persistentObject.GetCurrentPersistentState(dataPath, fullSave);
						if (currentPersistentState != null)
						{
							if (currentPersistentState.UniqId == string.Empty)
							{
								Debug.LogError("Persisten Error: Object " + persistentObject.gameObject.name + " has persistence and not UniqueId component");
							}
							else
							{
								snapShot.sceneElements[sceneKeyName][currentPersistentState.UniqId] = currentPersistentState;
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("** SaveSnapShot, error persistent.GetCurrentPersistentState: " + ex.Message);
						Debug.LogException(ex);
					}
				}
			}
			foreach (PersistentInterface persistentInterface in from x in this.persistenceSystems
			orderby x.GetOrder()
			select x)
			{
				try
				{
					PersistentManager.PersistentData currentPersistentState2 = persistentInterface.GetCurrentPersistentState(dataPath, fullSave);
					snapShot.commonElements[currentPersistentState2.UniqId] = currentPersistentState2;
				}
				catch (Exception ex2)
				{
					Debug.LogError("** SaveSnapShot, error item.GetCurrentPersistentState: " + ex2.Message);
					Debug.LogException(ex2);
				}
			}
			EntityStats stats = Core.Logic.Penitent.Stats;
			try
			{
				PersistentManager.PersistentData currentPersistentState3 = stats.GetCurrentPersistentState(dataPath, fullSave);
				snapShot.commonElements[currentPersistentState3.UniqId] = currentPersistentState3;
			}
			catch (Exception ex3)
			{
				Debug.LogError("** SaveSnapShot, error stats.GetCurrentPersistentState: " + ex3.Message);
				Debug.LogException(ex3);
			}
			this.SaveGame_Internal(PersistentManager.GetAutomaticSlot());
			this.InsideSaveProcess = false;
		}

		private void LoadSnapShot(PersistentManager.SnapShot snapShot, string scene, bool isloading, string dataPath)
		{
			if (this.InsideLoadLevelProcess)
			{
				Debug.LogError("*** SAVEERROR 4. LOAD INSIDE LOAD LEVEL PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
			}
			if (this.InsideSaveProcess)
			{
				Debug.LogError("*** SAVEERROR 5. LOAD INSIDE SAVE PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
			}
			if (this.InsideLoadProcess)
			{
				Debug.LogError("*** SAVEERROR 6. LOAD INSIDE LOAD PROCESS, PLEASE LOOK TRACE AND CALL A DEVELOPER");
			}
			this.InsideLoadProcess = true;
			string sceneKeyName = this.GetSceneKeyName(scene);
			if (snapShot.sceneElements.ContainsKey(sceneKeyName))
			{
				Dictionary<string, PersistentManager.PersistentData> dictionary = snapShot.sceneElements[sceneKeyName];
				foreach (PersistentObject persistentObject in UnityEngine.Object.FindObjectsOfType<PersistentObject>())
				{
					if (!persistentObject.IsIgnoringPersistence())
					{
						string persistenID = persistentObject.GetPersistenID();
						if (persistenID == string.Empty)
						{
							Debug.LogWarning("Persisten Error: Object " + persistentObject.gameObject.name + " has persistence and not UniqueId component");
						}
						else if (!dictionary.ContainsKey(persistenID))
						{
							Debug.Log("*** LoadSnapShot -- NO DATA FOR " + persistentObject.gameObject.name);
						}
						else
						{
							try
							{
								persistentObject.SetCurrentPersistentState(dictionary[persistenID], isloading, dataPath);
							}
							catch (Exception ex)
							{
								Debug.LogError("** LoadSnapShot, error persistent.SetCurrentPersistentState: " + ex.Message);
								Debug.LogException(ex);
							}
						}
					}
				}
			}
			foreach (PersistentInterface persistentInterface in from x in this.persistenceSystems
			orderby x.GetOrder()
			select x)
			{
				if (snapShot.commonElements.ContainsKey(persistentInterface.GetPersistenID()))
				{
					PersistentManager.PersistentData data = snapShot.commonElements[persistentInterface.GetPersistenID()];
					try
					{
						persistentInterface.SetCurrentPersistentState(data, isloading, dataPath);
					}
					catch (Exception ex2)
					{
						Debug.LogError("** LoadSnapShot, error item.SetCurrentPersistentState: " + ex2.Message);
						Debug.LogException(ex2);
					}
				}
			}
			EntityStats stats = Core.Logic.Penitent.Stats;
			if (snapShot.commonElements.ContainsKey(stats.GetPersistenID()))
			{
				try
				{
					stats.SetCurrentPersistentState(snapShot.commonElements[stats.GetPersistenID()], isloading, dataPath);
				}
				catch (Exception ex3)
				{
					Debug.LogError("** LoadSnapShot, error stats.SetCurrentPersistentState: " + ex3.Message);
					Debug.LogException(ex3);
				}
			}
			this.InsideLoadProcess = false;
		}

		private string GetSceneKeyName(string scene)
		{
			return scene.ToLower();
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_PERSISTENT_MANAGER";
		}

		public float GetCurrentTimePlayed()
		{
			return this.TimeStored + Time.realtimeSinceStartup - this.LastTimeStored;
		}

		public float GetCurrentTimePlayedForAC44()
		{
			return this.GetCurrentTimePlayed() - this.TimeAtSlotAscension;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			PersistentManager.PersitentPersistenceData persitentPersistenceData = new PersistentManager.PersitentPersistenceData();
			persitentPersistenceData.Percent = this.PercentCompleted;
			persitentPersistenceData.Time = this.TimeStored + Time.realtimeSinceStartup - this.LastTimeStored;
			persitentPersistenceData.TimeAtSlotAscension = this.TimeAtSlotAscension;
			if (Core.Logic.Penitent)
			{
				EntityStats stats = Core.Logic.Penitent.Stats;
				persitentPersistenceData.Purge = Core.Logic.Penitent.Stats.Purge.Current;
			}
			persitentPersistenceData.CurrentZone = Core.NewMapManager.CurrentSafeScene.Zone;
			persitentPersistenceData.CurrentDomain = Core.NewMapManager.CurrentSafeScene.District;
			persitentPersistenceData.IsNewGamePlus = Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
			persitentPersistenceData.NewGamePlusUpgrades = Core.GameModeManager.GetNewGamePlusUpgrades();
			persitentPersistenceData.CanConvertToNewGamePlus = false;
			return persitentPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			PersistentManager.PersitentPersistenceData persitentPersistenceData = (PersistentManager.PersitentPersistenceData)data;
			if (isloading)
			{
				this.TimeStored = persitentPersistenceData.Time;
				this.TimeAtSlotAscension = persitentPersistenceData.TimeAtSlotAscension;
				this.LastTimeStored = Time.realtimeSinceStartup;
			}
		}

		public void ResetPersistence()
		{
			this.LastTimeStored = Time.realtimeSinceStartup;
			this.TimeStored = 0f;
			this.TimeAtSlotAscension = 0f;
			this.InsideLoadLevelProcess = false;
			this.InsideSaveProcess = false;
			this.InsideLoadProcess = false;
		}

		private void CheckAndApplyBeadFix()
		{
			if (!PersistentManager.FixHasBeenApplied)
			{
				PersistentManager.FixHasBeenApplied = true;
				if (!this.CheckIfBeadFixIsApplied())
				{
					for (int i = 0; i < 3; i++)
					{
						this.LoadGameWithOutRespawn(i);
						this.FixBeadsRewards(i);
					}
					this.MarkBeadFixApplied();
				}
			}
		}

		private void FixBeadsRewards(int slotIndex)
		{
			bool flag = false;
			if (Core.GameModeManager.GetNewGamePlusUpgrades() == 1)
			{
				PersistentManager.SnapShot snapshot = new PersistentManager.SnapShot();
				bool flag2 = this.LoadGameSnapShot(slotIndex, ref snapshot);
				bool flag3 = this.ExistBackupSlot(slotIndex);
				if (!flag2 && flag3)
				{
					flag2 = this.LoadGameBackupSnapShot(slotIndex, ref snapshot);
				}
				IPenitence currentPenitence = Core.PenitenceManager.GetCurrentPenitence();
				if (!flag2 || !Core.GameModeManager.CanConvertToNewGamePlus(snapshot) || currentPenitence == null)
				{
					goto IL_1BE;
				}
				using (Dictionary<string, string>.Enumerator enumerator = PersistentManager.SkinsAndObjects.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, string> keyValuePair = enumerator.Current;
						if (Core.ColorPaletteManager.IsColorPaletteUnlocked(keyValuePair.Key) && !Core.InventoryManager.IsRosaryBeadOwned(keyValuePair.Value) && keyValuePair.Key.EndsWith(currentPenitence.Id))
						{
							flag = true;
							Core.InventoryManager.AddRosaryBead(keyValuePair.Value);
							Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();
						}
					}
					goto IL_1BE;
				}
			}
			if (Core.GameModeManager.GetNewGamePlusUpgrades() > 1)
			{
				foreach (KeyValuePair<string, string> keyValuePair2 in PersistentManager.SkinsAndObjects)
				{
					if (Core.ColorPaletteManager.IsColorPaletteUnlocked(keyValuePair2.Key) && !Core.InventoryManager.IsRosaryBeadOwned(keyValuePair2.Value))
					{
						flag = true;
						Core.InventoryManager.AddRosaryBead(keyValuePair2.Value);
						string[] array = keyValuePair2.Key.Split(new char[]
						{
							'_'
						});
						if (array.Length != 0)
						{
							string id = array[1];
							Core.PenitenceManager.MarkPenitenceAsCompleted(id);
						}
						else
						{
							Debug.LogError("RewardBeadsAlreadyFlagged: key: " + keyValuePair2.Key + " isn't a valid key!");
						}
					}
				}
			}
			IL_1BE:
			if (flag)
			{
				Core.Persistence.SaveGame(slotIndex, false);
			}
		}

		private bool CheckIfBeadFixIsApplied()
		{
			bool result = false;
			string pathAppSettings = PersistentManager.GetPathAppSettings("/app_settings");
			if (File.Exists(pathAppSettings))
			{
				new Dictionary<string, fsData>();
				byte[] bytes = Convert.FromBase64String(File.ReadAllText(pathAppSettings));
				fsData fsData;
				fsResult fsResult = fsJsonParser.Parse(Encoding.UTF8.GetString(bytes), out fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("CheckInFileIfBeadsRewarded: parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
					string key = "REWARDS_FIX";
					fsData fsData2;
					if (asDictionary.TryGetValue(key, out fsData2))
					{
						result = fsData2.AsBool;
					}
				}
			}
			return result;
		}

		private void MarkBeadFixApplied()
		{
			string pathAppSettings = PersistentManager.GetPathAppSettings("/app_settings");
			fsData fsData = PersistentManager.ReadAppSettings(pathAppSettings);
			if (fsData == null || !fsData.IsDictionary)
			{
				return;
			}
			string key = "REWARDS_FIX";
			fsData.AsDictionary[key] = new fsData(true);
			string s = fsJsonPrinter.CompressedJson(fsData);
			string encryptedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
			FileTools.SaveSecure(pathAppSettings, encryptedData);
		}

		public static string GetPathAppSettings(string fileName)
		{
			string arg = string.Empty;
			if (SteamManager.Initialized)
			{
				arg = SteamUser.GetSteamID().ToString();
			}
			string text = string.Format("{0}/Savegames/{1}/", Application.persistentDataPath, arg);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!fileName.IsNullOrWhitespace())
			{
				text += fileName;
			}
			return text;
		}

		public static fsData ReadAppSettings(string filePath)
		{
			fsData result = new fsData();
			string s;
			if (PersistentManager.TryToReadFile(filePath, out s))
			{
				byte[] bytes = Convert.FromBase64String(s);
				fsResult fsResult = fsJsonParser.Parse(Encoding.UTF8.GetString(bytes), out result);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("Parsing error: " + fsResult.FormattedMessages);
				}
			}
			return result;
		}

		public static bool TryToReadFile(string filePath, out string fileData)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError("File at path '" + filePath + "' does not exists!");
				fileData = string.Empty;
				return false;
			}
			fileData = File.ReadAllText(filePath);
			if (fileData.Length == 0)
			{
				Debug.Log("File at path '" + filePath + "' is empty.");
				return false;
			}
			return true;
		}

		private const int NOT_INITIALIZED_SLOT = -1;

		private const string BACKUP_SUFIX = "_backup";

		private static int CurrentSaveSlot = -1;

		private fsSerializer serializer = new fsSerializer();

		private bool InsideLoadLevelProcess;

		private bool InsideSaveProcess;

		private bool InsideLoadProcess;

		private readonly List<PersistentInterface> persistenceSystems = new List<PersistentInterface>();

		private PersistentManager.SnapShot currentSnapshot;

		private const string EVENT_PERSITENT_ID = "ID_PERSISTENT_MANAGER";

		public const string APP_SETTINGS_PATH = "/app_settings";

		private const string REWARDS_FIX_KEY = "REWARDS_FIX";

		private const int MAX_SLOTS = 3;

		private static Dictionary<string, string> SkinsAndObjects = new Dictionary<string, string>
		{
			{
				"PENITENT_PE01",
				"RB101"
			},
			{
				"PENITENT_PE02",
				"RB102"
			},
			{
				"PENITENT_PE03",
				"RB103"
			}
		};

		private static bool FixHasBeenApplied = false;

		[Serializable]
		public abstract class PersistentData
		{
			public PersistentData()
			{
				this.UniqId = string.Empty;
			}

			public PersistentData(string uniqId)
			{
				this.UniqId = uniqId;
			}

			[fsIgnore]
			public string UniqId { get; private set; }

			public string debugName = "Manager";
		}

		[Serializable]
		public class SnapShot
		{
			public SnapShot()
			{
				this.commonElements = new Dictionary<string, PersistentManager.PersistentData>();
				this.sceneElements = new Dictionary<string, Dictionary<string, PersistentManager.PersistentData>>();
			}

			public void Clear()
			{
				this.commonElements.Clear();
				this.sceneElements.Clear();
			}

			public void Copy(PersistentManager.SnapShot other)
			{
				this.commonElements = new Dictionary<string, PersistentManager.PersistentData>(other.commonElements);
				this.sceneElements = new Dictionary<string, Dictionary<string, PersistentManager.PersistentData>>(other.sceneElements);
			}

			public Dictionary<string, PersistentManager.PersistentData> commonElements;

			public Dictionary<string, Dictionary<string, PersistentManager.PersistentData>> sceneElements;
		}

		private class DEBUGPersistence : PersistentManager.PersistentData
		{
			public DEBUGPersistence(string id) : base(id)
			{
			}

			public bool boolean1;

			public bool boolean2;

			public float value1 = 1230.4f;

			public float value2 = 9191f;
		}

		public enum PercentageType
		{
			BossDefeated_1,
			BossDefeated_2,
			Upgraded,
			Exploration,
			Teleport_A,
			EndingA,
			ItemAdded,
			Custom,
			Map,
			Map_NgPlus,
			BossDefeated_NgPlus,
			Penitence_NgPlus,
			Teleport_B
		}

		[Serializable]
		public class PersitentPersistenceData : PersistentManager.PersistentData
		{
			public PersitentPersistenceData() : base("ID_PERSISTENT_MANAGER")
			{
			}

			public bool Corrupted;

			public string CurrentDomain = string.Empty;

			public string CurrentZone = string.Empty;

			public bool HasBackup;

			public float Percent;

			public float Purge;

			public float Time;

			public float TimeAtSlotAscension;

			public bool IsNewGamePlus;

			public bool CanConvertToNewGamePlus;

			public int NewGamePlusUpgrades;
		}

		public class PublicSlotData
		{
			public PersistentManager.PersitentPersistenceData persistence;

			public PenitenceManager.PenitencePersistenceData penitence;

			public EventManager.FlagPersistenceData flags;

			public AchievementsManager.AchievementPersistenceData achievement;
		}
	}
}
