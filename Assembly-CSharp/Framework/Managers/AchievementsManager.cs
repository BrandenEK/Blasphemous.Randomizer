using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Achievements;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Framework.Managers
{
	public class AchievementsManager : GameSystem, PersistentInterface
	{
		public bool ShowPopUp { get; set; }

		public override void AllInitialized()
		{
			this.ShowPopUp = false;
			Core.Persistence.AddPersistentManager(this);
		}

		private IAchievementsHelper createHelper()
		{
			return new SteamAchievementsHelper();
		}

		private void CreateAchievements()
		{
			this.Achievements = new Dictionary<string, Achievement>();
			this.ResetPersistence();
		}

		public override void Initialize()
		{
			this.CreateAchievements();
			this.helper = this.createHelper();
			Entity.Death += this.AddProgressToAC39;
			Entity.Death += this.AddProgressToAC41;
			this.ac39EnemiesData = Resources.Load<AC39Enemies>("Achievements/AC39_ENEMIES_DATA");
		}

		public override void Dispose()
		{
			Entity.Death -= this.AddProgressToAC39;
			Entity.Death -= this.AddProgressToAC41;
		}

		public void AddAchievementProgress(string achievementId, float progress)
		{
			if (!Core.GameModeManager.ShouldProgressAchievements())
			{
				return;
			}
			if (this.Achievements.ContainsKey(achievementId.ToUpper()))
			{
				this.Achievements[achievementId.ToUpper()].AddProgress(progress);
				Debug.Log(string.Concat(new object[]
				{
					"AddAchievementProgress: achievement with id: ",
					achievementId.ToUpper(),
					" has been added a progress of: ",
					progress,
					" and now has a total of: ",
					this.Achievements[achievementId.ToUpper()].Progress
				}));
			}
			else
			{
				Debug.Log("AddAchievementProgress: Achievements does not contains achievement with id: " + achievementId.ToUpper());
			}
		}

		public void GrantAchievement(string achievementId)
		{
			if (!Core.GameModeManager.ShouldProgressAchievements())
			{
				return;
			}
			if (this.Achievements.ContainsKey(achievementId.ToUpper()))
			{
				if (this.Achievements[achievementId.ToUpper()].IsGranted())
				{
					Debug.Log("GrantAchievement: achievement with id: " + achievementId.ToUpper() + " try to grant but it was granted.");
				}
				else
				{
					this.Achievements[achievementId.ToUpper()].Grant();
					Debug.Log("GrantAchievement: achievement with id: " + achievementId.ToUpper() + " has been granted.");
				}
			}
			else
			{
				Debug.Log("GrantAchievement: Achievements does not contains achievement with id: " + achievementId.ToUpper());
			}
		}

		public bool CheckAchievementGranted(string achievementId)
		{
			if (this.Achievements.ContainsKey(achievementId.ToUpper()))
			{
				if (this.Achievements[achievementId.ToUpper()].IsGranted())
				{
					Debug.Log("CheckAchievementGranted: achievement with id: " + achievementId.ToUpper() + " is granted.");
				}
				else
				{
					Debug.Log("CheckAchievementGranted: achievement with id: " + achievementId.ToUpper() + " is not granted.");
				}
			}
			else
			{
				Debug.Log("CheckAchievementGranted: Achievements does not contains achievement with id: " + achievementId.ToUpper());
			}
			return this.Achievements[achievementId.ToUpper()].IsGranted();
		}

		public float CheckAchievementProgress(string achievementId)
		{
			if (this.Achievements.ContainsKey(achievementId.ToUpper()))
			{
				Debug.Log(string.Concat(new object[]
				{
					"CheckAchievementProgress: achievement with id: ",
					achievementId.ToUpper(),
					" has a progress of: ",
					this.Achievements[achievementId.ToUpper()].Progress
				}));
				return this.Achievements[achievementId.ToUpper()].Progress;
			}
			Debug.Log("CheckAchievementProgress: Achievements does not contains achievement with id: " + achievementId.ToUpper());
			return 0f;
		}

		public void DebugResetAchievement(string achievementId)
		{
			if (this.Achievements.ContainsKey(achievementId.ToUpper()))
			{
				this.Achievements[achievementId.ToUpper()].Reset();
				Debug.Log("DebugResetAchievement: achievement with id: " + achievementId.ToUpper() + " has been reset.");
			}
			else
			{
				Debug.Log("DebugResetAchievement: Achievements does not contains achievement with id: " + achievementId.ToUpper());
			}
		}

		public void PrepareForNewGamePlus()
		{
			foreach (Achievement achievement in from a in this.Achievements.Values
			where !a.IsGranted() && !a.PreserveProgressInNewGamePlus
			select a)
			{
				Debug.Log("Reset achievement " + achievement.Id + " for a new game plus");
				achievement.Progress = 0f;
			}
		}

		public void CheckProgressToAC46()
		{
			Achievement achievement = this.Achievements["AC46"];
			if (!achievement.IsGranted())
			{
				float num = Core.Persistence.PercentCompleted - achievement.Progress;
				if (num > 0f)
				{
					achievement.AddProgress(num);
				}
			}
		}

		public void AddProgressToAC39(Entity entity)
		{
			if (Core.AchievementsManager.Achievements["AC39"].IsGranted())
			{
				return;
			}
			Enemy enemy = entity as Enemy;
			if (!enemy || string.IsNullOrEmpty(enemy.Id) || !this.ac39EnemiesData.EnemiesList.Exists((EnemyIdAndName x) => x.id.Equals(enemy.Id)))
			{
				return;
			}
			this.CheckAndApplyAC39Fix();
			string newSystemFlagName = this.GetNewSystemFlagName(enemy.Id);
			if (!Core.Events.GetFlag(newSystemFlagName))
			{
				Core.Events.SetFlag(newSystemFlagName, true, true);
				Core.AchievementsManager.Achievements["AC39"].AddProgress(100f / (float)this.ac39EnemiesData.EnemiesList.Count);
			}
		}

		private void RecalculateAC39Progress()
		{
			Core.AchievementsManager.Achievements["AC39"].Progress = 0f;
			foreach (EnemyIdAndName enemyIdAndName in this.ac39EnemiesData.EnemiesList)
			{
				string oldSystemFlagName = this.GetOldSystemFlagName(enemyIdAndName.name);
				if (!Core.Events.GetFlag(oldSystemFlagName) && enemyIdAndName.hasAnotherName)
				{
					oldSystemFlagName = this.GetOldSystemFlagName(enemyIdAndName.otherName);
				}
				if (Core.Events.GetFlag(oldSystemFlagName))
				{
					string newSystemFlagName = this.GetNewSystemFlagName(enemyIdAndName.id);
					Core.Events.SetFlag(newSystemFlagName, true, true);
					Core.AchievementsManager.Achievements["AC39"].AddProgress(100f / (float)this.ac39EnemiesData.EnemiesList.Count);
				}
			}
		}

		private string GetOldSystemFlagName(string enemyName)
		{
			string str = enemyName.ToUpper().Trim();
			return str + "_KILLED";
		}

		private string GetNewSystemFlagName(string enemyId)
		{
			return enemyId.ToUpper() + "_KILLED";
		}

		private void CheckAndApplyAC39Fix()
		{
			if (!Core.Events.GetFlag("AC39_FIX"))
			{
				this.RecalculateAC39Progress();
				Core.Events.SetFlag("AC39_FIX", true, true);
			}
		}

		public void AddProgressToAC41(Entity entity)
		{
			if (Core.AchievementsManager.Achievements["AC41"].IsGranted())
			{
				return;
			}
			Enemy enemy = entity as Enemy;
			if (!enemy || enemy.gameObject.CompareTag("CherubCaptor"))
			{
				return;
			}
			Core.AchievementsManager.Achievements["AC41"].AddProgress(0.15015015f);
		}

		public void CheckFlagsToGrantAC19()
		{
			bool flag = true;
			for (int i = 1; i <= 44; i++)
			{
				string id = string.Format("CO{0:00}_OWNED", i);
				if (!Core.Events.GetFlag(id))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.GrantAchievement("AC19");
			}
		}

		public static void CheckFlagsToGrantAC14()
		{
			bool flag = true;
			List<string> list = new List<string>();
			for (int i = 1; i <= 26; i++)
			{
				list.Add(string.Format("CORPSES_{0:00}_FINISHED", i));
			}
			foreach (string id in list)
			{
				if (!Core.Events.GetFlag(id))
				{
					flag = false;
				}
			}
			if (flag)
			{
				Core.AchievementsManager.GrantAchievement("AC14");
			}
		}

		public void AddProgressToAC21(string domain, string zone)
		{
			string domainAndZone = domain.ToUpper() + zone.ToUpper();
			if (this.AC21DomainAndZones.Exists((string x) => x.Equals(domainAndZone)))
			{
				string id = "ZONE_NAME_OF_" + domainAndZone + "_DISPLAYED";
				if (!Core.Events.GetFlag(id))
				{
					Core.Events.SetFlag(id, true, false);
					Core.AchievementsManager.Achievements["AC21"].AddProgress(100f / (float)this.AC21DomainAndZones.Count);
				}
			}
		}

		public void SetAchievementProgress(string achievementId, float progress)
		{
			if (this.helper == null)
			{
				return;
			}
			Debug.Log(string.Concat(new object[]
			{
				"SetAchievementProgress: ID: ",
				achievementId,
				"  Progress:",
				progress
			}));
			this.helper.SetAchievementProgress(achievementId, progress);
			if (progress >= 100f)
			{
				LocalAchievementsHelper.SetAchievementUnlocked(achievementId);
			}
		}

		public void GetAchievementProgress(string achievementId, GetAchievementOperationEvent evt)
		{
			if (this.helper == null)
			{
				return;
			}
			Debug.Log("GetAchievementProgress: string: " + achievementId);
			this.helper.GetAchievementProgress(achievementId, evt);
		}

		public void DebugReset()
		{
			foreach (string key in this.Achievements.Keys)
			{
				this.Achievements[key].Reset();
			}
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("Achievement Manager -------------------------------------", 10, 1500);
			string text = string.Empty;
			foreach (Achievement achievement in from p in this.Achievements.Values
			orderby p.Id
			select p)
			{
				string text2 = achievement.Id + ": " + achievement.Progress.ToString("0.##");
				if (text == string.Empty)
				{
					text = text2;
				}
				else
				{
					base.DebugDrawTextLine(string.Format("{0,-15}", text) + text2, 10, 1500);
					text = string.Empty;
				}
			}
			if (text != string.Empty)
			{
				base.DebugDrawTextLine(text, 10, 1500);
			}
		}

		public List<Achievement> GetAllAchievements()
		{
			this.CreateAchievements();
			List<string> localAchievementIds = LocalAchievementsHelper.GetLocalAchievementIds();
			foreach (string text in this.Achievements.Keys)
			{
				bool flag = localAchievementIds.Contains(text);
				if (flag)
				{
					this.Achievements[text].CurrentStatus = Achievement.Status.UNLOCKED;
				}
				else
				{
					this.Achievements[text].CurrentStatus = ((!this.Achievements[text].CanBeHidden) ? Achievement.Status.LOCKED : Achievement.Status.HIDDEN);
				}
			}
			List<Achievement> list = this.Achievements.Values.ToList<Achievement>();
			list.Sort((Achievement ach1, Achievement ach2) => ach1.Id.CompareTo(ach2.Id));
			return list;
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_ACHIEVEMENTS_MANAGER";
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			AchievementsManager.AchievementPersistenceData achievementPersistenceData = new AchievementsManager.AchievementPersistenceData();
			Achievement[] array = new Achievement[this.Achievements.Values.Count];
			this.Achievements.Values.CopyTo(array, 0);
			achievementPersistenceData.achievements = array.ToList<Achievement>();
			return achievementPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			AchievementsManager.AchievementPersistenceData achievementPersistenceData = (AchievementsManager.AchievementPersistenceData)data;
			foreach (Achievement achievement in achievementPersistenceData.achievements)
			{
				if (this.Achievements.ContainsKey(achievement.Id))
				{
					Sprite image = this.Achievements[achievement.Id].Image;
					achievement.PreserveProgressInNewGamePlus = (achievement.PreserveProgressInNewGamePlus || this.Achievements[achievement.Id].PreserveProgressInNewGamePlus);
					this.Achievements[achievement.Id] = achievement;
					this.Achievements[achievement.Id].Image = image;
				}
				else
				{
					Debug.LogWarning("***** AchievementsManager: SetCurrentPersistentState Achievement " + achievement.Id + " NOT found, creating a new one");
					Achievement achievement2 = new Achievement(achievement.Id);
					achievement2.Progress = achievement.Progress;
					this.Achievements[achievement.Id] = achievement2;
				}
			}
		}

		public void ResetPersistence()
		{
			this.Achievements.Clear();
			AchievementList[] array = Resources.LoadAll<AchievementList>("Achievements/");
			foreach (AchievementList achievementList in array)
			{
				foreach (Achievement other in achievementList.achievementList)
				{
					Achievement achievement = new Achievement(other);
					achievement.CurrentStatus = Achievement.Status.LOCKED;
					this.Achievements[achievement.Id] = achievement;
				}
			}
		}

		private IAchievementsHelper helper;

		public Dictionary<string, Achievement> Achievements;

		private const int TOTAL_NUMBER_OF_ENEMY_DEATHS_FOR_AC41 = 666;

		private const int TOTAL_NUMBER_OF_BODIES_FOR_AC14 = 26;

		private AC39Enemies ac39EnemiesData;

		private const string AC39_FIX_FLAG = "AC39_FIX";

		public readonly List<string> AC21DomainAndZones = new List<string>
		{
			"D01Z01",
			"D01Z02",
			"D01Z03",
			"D01Z04",
			"D01Z05",
			"D02Z01",
			"D02Z02",
			"D02Z03",
			"D03Z01",
			"D03Z02",
			"D03Z03",
			"D04Z01",
			"D04Z02",
			"D04Z03",
			"D05Z01",
			"D05Z02",
			"D06Z01",
			"D07Z01",
			"D08Z01",
			"D08Z02",
			"D09Z01",
			"D17Z01"
		};

		private const string ACHIEVEMENTS_PERSITENT_ID = "ID_ACHIEVEMENTS_MANAGER";

		[Serializable]
		public class AchievementPersistenceData : PersistentManager.PersistentData
		{
			public AchievementPersistenceData() : base("ID_ACHIEVEMENTS_MANAGER")
			{
			}

			public List<Achievement> achievements = new List<Achievement>();
		}
	}
}
