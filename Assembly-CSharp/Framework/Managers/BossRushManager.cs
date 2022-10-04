using System;
using System.Collections;
using System.Collections.Generic;
using Framework.BossRush;
using Framework.FrameworkCore;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Damage;
using Gameplay.UI;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Framework.Managers
{
	public class BossRushManager : GameSystem
	{
		public override void Initialize()
		{
			this.configFile = new LocalDataFile(this.courseData);
			this.LoadCourseScenes();
		}

		private void LoadCourseScenes()
		{
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_A_1] = Resources.Load<BossRushCourse>("BossRush/COURSE_A_1");
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_A_2] = Resources.Load<BossRushCourse>("BossRush/COURSE_A_2");
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_A_3] = Resources.Load<BossRushCourse>("BossRush/COURSE_A_3");
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_B_1] = Resources.Load<BossRushCourse>("BossRush/COURSE_B_1");
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_C_1] = Resources.Load<BossRushCourse>("BossRush/COURSE_C_1");
			this.courseScenesByCourseId[BossRushManager.BossRushCourseId.COURSE_D_1] = Resources.Load<BossRushRandomCourse>("BossRush/COURSE_D_1");
		}

		public override void Update()
		{
			base.Update();
			if (this.currentScore != null)
			{
				this.currentScore.UpdateTimer();
			}
		}

		public override void Dispose()
		{
		}

		public void StartCourse(BossRushManager.BossRushCourseId courseId, BossRushManager.BossRushCourseMode courseMode, int sourceSlot = -1)
		{
			this.SourceSlot = sourceSlot;
			this.currentCourseSceneIndex = -1;
			this.currentCourseId = courseId;
			this.currentCourseMode = courseMode;
			Debug.Log("--- Start Course, initial slot " + sourceSlot);
			if (this.SourceSlot != -1)
			{
				Core.Logic.ResetAllData();
				Core.Persistence.LoadGameWithOutRespawn(this.SourceSlot);
			}
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.BOSS_RUSH);
			PersistentManager.SetAutomaticSlot(7);
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Purge.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Fervour.Current = 0f;
			Core.PenitenceManager.DeactivateCurrentPenitence();
			Core.InventoryManager.RemoveSword("HE201");
			Core.Persistence.SaveGame(false);
			this.currentScore = new BossRushHighScore(this.currentCourseId, this.currentCourseMode);
			this.SuscribeToHighScoreRelatedEvents();
			if (this.courseScenesByCourseId[this.currentCourseId].GetType() == typeof(BossRushRandomCourse))
			{
				BossRushRandomCourse bossRushRandomCourse = (BossRushRandomCourse)this.courseScenesByCourseId[this.currentCourseId];
				bossRushRandomCourse.RandomizeNextScenesList = true;
			}
			this.LoadHub(false);
			UIController.instance.HidePurgePoints();
		}

		public BossRushManager.BossRushCourseMode GetCurrentBossRushMode()
		{
			return this.currentCourseMode;
		}

		public string GetCurrentRunDurationString()
		{
			string result = string.Empty;
			if (this.currentScore != null)
			{
				result = this.currentScore.RunDurationInString();
			}
			return result;
		}

		public bool IsLastScene()
		{
			List<string> scenes = this.courseScenesByCourseId[this.currentCourseId].GetScenes();
			string value = scenes[scenes.Count - 1];
			return Core.LevelManager.currentLevel.LevelName.Equals(value);
		}

		public void LoadHub(bool whiteFade = true)
		{
			Core.SpawnManager.InitialScene = "D22Z01S00";
			Core.SpawnManager.SetInitialSpawn("D22Z01S00");
			Core.LevelManager.ActivatePrecachedScene();
			Core.SpawnManager.FirstSpanw = true;
			if (!Core.LevelManager.currentLevel.LevelName.Equals("D22Z01S00"))
			{
				if (whiteFade)
				{
					Singleton<Core>.Instance.StartCoroutine(this.LoadHubFadeWhiteCourrutine());
				}
				else
				{
					Core.LevelManager.ChangeLevel("D22Z01S00", false, true, false, null);
				}
			}
			UIController.instance.CanEquipSwordHearts = true;
			List<string> scenes = this.courseScenesByCourseId[this.currentCourseId].GetScenes();
			List<string> fontRechargingScenes = this.courseScenesByCourseId[this.currentCourseId].GetFontRechargingScenes();
			if (this.currentCourseSceneIndex >= 0 && fontRechargingScenes.Contains(scenes[this.currentCourseSceneIndex]))
			{
				if (Core.Events.GetFlag("BOSSRUSH_HEALTH_FONT"))
				{
					Core.Events.SetFlag("BOSSRUSH_HEALTH_FONT", false, false);
					Singleton<Core>.Instance.StartCoroutine(this.LaunchHealthFontRechargingVfx());
				}
				if (Core.Events.GetFlag("BOSSRUSH_FERVOUR_FONT"))
				{
					Core.Events.SetFlag("BOSSRUSH_FERVOUR_FONT", false, false);
					Singleton<Core>.Instance.StartCoroutine(this.LaunchFervourFontRechargingVfx());
				}
			}
		}

		private IEnumerator LaunchHealthFontRechargingVfx()
		{
			yield return new WaitUntil(() => Core.LevelManager.currentLevel.LevelName.Equals("D22Z01S00"));
			yield return new WaitForSeconds(1f);
			GameObject vfx = this.courseScenesByCourseId[this.currentCourseId].healthFontRechargingVfx;
			PoolManager.Instance.ReuseObject(vfx, new Vector2(-2f, -2.5f), Quaternion.identity, true, 1);
			yield break;
		}

		private IEnumerator LaunchFervourFontRechargingVfx()
		{
			yield return new WaitUntil(() => Core.LevelManager.currentLevel.LevelName.Equals("D22Z01S00"));
			yield return new WaitForSeconds(1f);
			GameObject vfx = this.courseScenesByCourseId[this.currentCourseId].fervourFontRechargingVfx;
			PoolManager.Instance.ReuseObject(vfx, new Vector2(2f, -2.5f), Quaternion.identity, true, 1);
			yield break;
		}

		private IEnumerator LoadHubFadeWhiteCourrutine()
		{
			Core.Audio.PlayOneShot("event:/SFX/UI/FadeToWhite", default(Vector3));
			yield return new WaitForSecondsRealtime(1f);
			yield return FadeWidget.instance.FadeCoroutine(new Color(0f, 0f, 0f, 0f), Color.white, 2f, true, null);
			Core.LevelManager.ChangeLevel("D22Z01S00", false, false, false, new Color?(Color.white));
			yield break;
		}

		public void LoadLastScene()
		{
			List<string> scenes = this.courseScenesByCourseId[this.currentCourseId].GetScenes();
			this.currentCourseSceneIndex = scenes.Count - 2;
			this.LoadCourseNextScene();
		}

		public void LoadCourseNextScene()
		{
			if (this.currentCourseSceneIndex == -1)
			{
				this.StartTimerAndShow();
			}
			List<string> scenes = this.courseScenesByCourseId[this.currentCourseId].GetScenes();
			if (this.currentCourseSceneIndex == scenes.Count - 1)
			{
				Debug.LogError("Course has already reached its final scene! Use EndCourse instead.");
			}
			else
			{
				this.currentCourseSceneIndex++;
				this.currentScore.NumScenesCompleted++;
			}
			string teleportId = "TELEPORT_" + scenes[this.currentCourseSceneIndex];
			Core.SpawnManager.Teleport(teleportId);
			UIController.instance.CanEquipSwordHearts = false;
		}

		public void EndCourse(bool completed)
		{
			bool unlockHard = false;
			if (completed)
			{
				this.currentScore.NumScenesCompleted++;
				if (this.currentCourseMode == BossRushManager.BossRushCourseMode.NORMAL)
				{
					unlockHard = this.courseData.UnlockCourse(this.currentCourseId, BossRushManager.BossRushCourseMode.HARD);
				}
			}
			this.UnsuscribeToHighScoreRelatedEvents();
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.MENU);
			this.StopTimerAndHide();
			this.currentScore.CalculateScoreObtained(completed);
			BossRushManager.CourseData courseData = null;
			if (!this.courseData.Data.TryGetValue(this.currentCourseId, out courseData))
			{
				this.courseData.UnlockCourse(this.currentCourseId, this.currentCourseMode);
				courseData = this.courseData.Data[this.currentCourseId];
			}
			courseData.SetHighScoreIfBetter(this.currentCourseMode, this.currentScore);
			this.configFile.SaveData();
			this.LogHighScoreObtained();
			Singleton<Core>.Instance.StartCoroutine(this.ShowScoreAndGoNext(true, completed, unlockHard));
			if (this.currentCourseId == BossRushManager.BossRushCourseId.COURSE_C_1 && this.courseData.Data[this.currentCourseId].GetHighScore(BossRushManager.BossRushCourseMode.HARD) != null && this.courseData.Data[this.currentCourseId].GetHighScore(BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted)
			{
				Core.ColorPaletteManager.UnlockBossRushColorPalette();
			}
			this.CheckToGrantBossRushRankSPalette();
		}

		private void CheckToGrantBossRushRankSPalette()
		{
			if (this.courseData == null || this.courseData.Data == null || !this.courseData.Data.ContainsKey(this.currentCourseId) || this.courseData.Data[this.currentCourseId].GetHighScore(this.currentCourseMode) == null || !this.courseData.Data[this.currentCourseId].GetHighScore(this.currentCourseMode).WasTheCourseCompleted)
			{
				return;
			}
			bool flag = true;
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_1));
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_2));
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_3));
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_B_1));
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_C_1));
			flag = (flag && this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_D_1));
			if (!flag)
			{
				return;
			}
			foreach (KeyValuePair<BossRushManager.BossRushCourseId, BossRushManager.CourseData> keyValuePair in this.courseData.Data)
			{
				bool flag2 = keyValuePair.Value.GetHighScore(BossRushManager.BossRushCourseMode.NORMAL) != null && keyValuePair.Value.GetHighScore(BossRushManager.BossRushCourseMode.NORMAL).Score >= BossRushManager.BossRushCourseScore.S_MINUS && keyValuePair.Value.GetHighScore(BossRushManager.BossRushCourseMode.HARD) != null && keyValuePair.Value.GetHighScore(BossRushManager.BossRushCourseMode.HARD).Score >= BossRushManager.BossRushCourseScore.S_MINUS;
				flag = (flag && flag2);
				if (!flag)
				{
					break;
				}
			}
			if (flag)
			{
				Core.ColorPaletteManager.UnlockBossRushRankSColorPalette();
			}
		}

		public void LogHighScoreObtained()
		{
			Debug.Log("<b> - Number of hits received: " + this.currentScore.NumHitsReceived + "</b>");
			Debug.Log("<b> - Number of blood penances used: " + this.currentScore.NumBloodPenancesUsed + "</b>");
			Debug.Log("<b> - Number of dodges achieved: " + this.currentScore.NumDodgesAchieved + "</b>");
			Debug.Log("<b> - Number of flasks used: " + this.currentScore.NumFlasksUsed + "</b>");
			Debug.Log("<b> - Number of prayers used: " + this.currentScore.NumPrayersUsed + "</b>");
			Debug.Log("<b> - Run duration: " + this.currentScore.RunDuration + "</b>");
			Debug.Log("<b> - FINAL SCORE: " + this.currentScore.Score + "</b>");
		}

		public bool IsAnyCourseUnlocked()
		{
			bool flag = false;
			foreach (KeyValuePair<BossRushManager.BossRushCourseId, BossRushManager.CourseData> keyValuePair in this.courseData.Data)
			{
				flag = (flag || keyValuePair.Value.HasAnyModeUnlocked());
			}
			return flag;
		}

		public bool IsModeUnlocked(BossRushManager.BossRushCourseId courseId, BossRushManager.BossRushCourseMode mode)
		{
			bool result = false;
			if (this.courseData.Data.ContainsKey(courseId))
			{
				result = this.courseData.Data[courseId].IsModeUnlocked(mode);
			}
			return result;
		}

		public BossRushHighScore GetHighScore(BossRushManager.BossRushCourseId courseId, BossRushManager.BossRushCourseMode mode)
		{
			BossRushHighScore result = null;
			if (this.courseData.Data.ContainsKey(courseId))
			{
				result = this.courseData.Data[courseId].GetHighScore(mode);
			}
			return result;
		}

		public BossRushHighScore GetPrevHighScore(BossRushManager.BossRushCourseId courseId, BossRushManager.BossRushCourseMode mode)
		{
			BossRushHighScore result = null;
			if (this.courseData.Data.ContainsKey(courseId))
			{
				result = this.courseData.Data[courseId].GetPrevHighScore(mode);
			}
			return result;
		}

		public void DEBUGUnlockCourse(string courseId)
		{
			BossRushManager.BossRushCourseId course = (BossRushManager.BossRushCourseId)Enum.Parse(typeof(BossRushManager.BossRushCourseId), courseId);
			this.courseData.UnlockCourse(course, BossRushManager.BossRushCourseMode.NORMAL);
		}

		public List<BossRushManager.BossRushCourseId> GetUnlockedCourses()
		{
			List<BossRushManager.BossRushCourseId> list = new List<BossRushManager.BossRushCourseId>();
			foreach (KeyValuePair<BossRushManager.BossRushCourseId, BossRushManager.CourseData> keyValuePair in this.courseData.Data)
			{
				if (keyValuePair.Value.HasAnyModeUnlocked())
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list;
		}

		public static List<string> GetAllCoursesNames()
		{
			List<string> list = new List<string>();
			BossRushManager.BossRushCourseId[] array = (BossRushManager.BossRushCourseId[])Enum.GetValues(typeof(BossRushManager.BossRushCourseId));
			foreach (BossRushManager.BossRushCourseId bossRushCourseId in array)
			{
				list.Add(bossRushCourseId.ToString());
			}
			return list;
		}

		public void CheckCoursesUnlockBySlots()
		{
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				PersistentManager.PublicSlotData slotData = Core.Persistence.GetSlotData(i);
				if (slotData != null && !slotData.persistence.Corrupted && slotData.persistence != null)
				{
					if (this.CheckToUnlockCourseA1(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_A_1, BossRushManager.BossRushCourseMode.NORMAL);
					}
					if (this.CheckToUnlockCourseA2(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_A_2, BossRushManager.BossRushCourseMode.NORMAL);
					}
					if (this.CheckToUnlockCourseA3(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_A_3, BossRushManager.BossRushCourseMode.NORMAL);
					}
					if (this.CheckToUnlockCourseB1(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_B_1, BossRushManager.BossRushCourseMode.NORMAL);
					}
					if (this.CheckToUnlockCourseC1(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_C_1, BossRushManager.BossRushCourseMode.NORMAL);
					}
					if (this.CheckToUnlockCourseD1(slotData))
					{
						flag = true;
						this.courseData.UnlockCourse(BossRushManager.BossRushCourseId.COURSE_D_1, BossRushManager.BossRushCourseMode.NORMAL);
					}
				}
			}
			if (flag)
			{
				this.configFile.SaveData();
			}
		}

		public int CountScenesInCourse(BossRushManager.BossRushCourseId id)
		{
			return this.courseScenesByCourseId[id].GetScenes().Count;
		}

		public BossRushManager.BossRushCourseScore GetRankByTimePassed(BossRushManager.BossRushCourseId id, BossRushManager.BossRushCourseMode mode, int minutes)
		{
			List<ScoreInterval> list = (mode != BossRushManager.BossRushCourseMode.NORMAL) ? this.courseScenesByCourseId[id].ScoresByMinutesInHard : this.courseScenesByCourseId[id].ScoresByMinutesInNormal;
			foreach (ScoreInterval scoreInterval in list)
			{
				if (scoreInterval.score <= this.courseScenesByCourseId[id].MaxScoreForFailedRuns)
				{
					break;
				}
				if (scoreInterval.timeRangeInMinutes.x <= (float)minutes && scoreInterval.timeRangeInMinutes.y > (float)minutes)
				{
					return scoreInterval.score;
				}
			}
			return this.courseScenesByCourseId[id].MaxScoreForFailedRuns;
		}

		public BossRushManager.BossRushCourseScore GetRankByNumberOfCompletedBossfights(BossRushManager.BossRushCourseId id, int numBossfightsCompleted)
		{
			int num = this.CountScenesInCourse(id);
			float num2 = (float)numBossfightsCompleted / (float)num;
			float num3 = (float)this.courseScenesByCourseId[id].MaxScoreForFailedRuns * num2;
			return (BossRushManager.BossRushCourseScore)num3;
		}

		private IEnumerator ShowScoreAndGoNext(bool pauseGame, bool complete, bool unlockHard)
		{
			yield return Singleton<Core>.Instance.StartCoroutine(UIController.instance.ShowBossRushRanksAndWait(this.currentScore, pauseGame, complete, unlockHard));
			if (UIController.instance.BossRushRetryPressed)
			{
				this.StartCourse(this.currentCourseId, this.currentCourseMode, this.SourceSlot);
			}
			else
			{
				UIController.instance.ShowLoad(true, new Color?(Color.black));
				Core.Logic.LoadMenuScene(false);
				UIController.instance.ShowPurgePoints();
			}
			yield break;
		}

		private void SuscribeToHighScoreRelatedEvents()
		{
			this.UnsuscribeToHighScoreRelatedEvents();
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Combine(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnHitGlobal));
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			FervourPenance.OnPenanceStart = (Core.SimpleEvent)Delegate.Combine(FervourPenance.OnPenanceStart, new Core.SimpleEvent(this.OnPenanceStart));
			Healing.OnHealingStart = (Core.SimpleEvent)Delegate.Combine(Healing.OnHealingStart, new Core.SimpleEvent(this.OnHealingStart));
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad += this.OnLevelPreload;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			PrayerUse prayerCast = Core.Logic.Penitent.PrayerCast;
			prayerCast.OnPrayerStart = (Core.SimpleEvent)Delegate.Combine(prayerCast.OnPrayerStart, new Core.SimpleEvent(this.OnPrayerStart));
		}

		private void OnLevelPreload(Level oldlevel, Level newlevel)
		{
			PrayerUse prayerCast = Core.Logic.Penitent.PrayerCast;
			prayerCast.OnPrayerStart = (Core.SimpleEvent)Delegate.Remove(prayerCast.OnPrayerStart, new Core.SimpleEvent(this.OnPrayerStart));
		}

		private void UnsuscribeToHighScoreRelatedEvents()
		{
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Remove(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnHitGlobal));
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			FervourPenance.OnPenanceStart = (Core.SimpleEvent)Delegate.Remove(FervourPenance.OnPenanceStart, new Core.SimpleEvent(this.OnPenanceStart));
			Healing.OnHealingStart = (Core.SimpleEvent)Delegate.Remove(Healing.OnHealingStart, new Core.SimpleEvent(this.OnHealingStart));
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad -= this.OnLevelPreload;
			PrayerUse prayerCast = Core.Logic.Penitent.PrayerCast;
			prayerCast.OnPrayerStart = (Core.SimpleEvent)Delegate.Remove(prayerCast.OnPrayerStart, new Core.SimpleEvent(this.OnPrayerStart));
		}

		private void OnHitGlobal(Penitent penitent, Hit hit)
		{
			if (hit.DamageAmount > 0f && !hit.Unnavoidable && penitent.IsDashing)
			{
				this.currentScore.NumDodgesAchieved++;
			}
		}

		private void OnDamagedGlobal(Penitent damaged, Hit hit)
		{
			if (hit.DamageAmount <= 0f)
			{
				return;
			}
			this.currentScore.NumHitsReceived++;
		}

		private void OnPrayerStart()
		{
			this.currentScore.NumPrayersUsed++;
		}

		private void OnPenanceStart()
		{
			this.currentScore.NumBloodPenancesUsed++;
		}

		private void OnHealingStart()
		{
			this.currentScore.NumFlasksUsed++;
		}

		private void StartTimerAndShow()
		{
			this.currentScore.IsTimerActive = true;
			UIController.instance.ShowBossRushTimer();
		}

		private void StopTimerAndHide()
		{
			this.currentScore.IsTimerActive = false;
			UIController.instance.HideBossRushTimer();
		}

		private bool CheckToUnlockCourseA1(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_1) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_A_1].HasAnyModeUnlocked();
			return !flag && (data.persistence.IsNewGamePlus || data.persistence.CanConvertToNewGamePlus);
		}

		private bool CheckToUnlockCourseA2(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_2) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_A_2].HasAnyModeUnlocked();
			return !flag && this.IsModeUnlocked(BossRushManager.BossRushCourseId.COURSE_A_1, BossRushManager.BossRushCourseMode.HARD);
		}

		private bool CheckToUnlockCourseA3(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_A_3) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_A_3].HasAnyModeUnlocked();
			return !flag && this.IsModeUnlocked(BossRushManager.BossRushCourseId.COURSE_A_2, BossRushManager.BossRushCourseMode.HARD);
		}

		private bool CheckToUnlockCourseB1(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_B_1) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_B_1].HasAnyModeUnlocked();
			if (flag)
			{
				return false;
			}
			string key = BossRushManager.BossRushCourseId.COURSE_B_1.ToString() + "_UNLOCKED";
			bool result = false;
			if (data.flags != null)
			{
				if (data.flags.flags.ContainsKey(key))
				{
					result = data.flags.flags[key];
				}
				else if (data.flags.flags.ContainsKey("SANTOS_LAUDES_DEFEATED"))
				{
					result = data.flags.flags["SANTOS_LAUDES_DEFEATED"];
				}
			}
			return result;
		}

		private bool CheckToUnlockCourseC1(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_C_1) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_C_1].HasAnyModeUnlocked();
			return !flag && (this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_1, BossRushManager.BossRushCourseMode.HARD) != null && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_1, BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_2, BossRushManager.BossRushCourseMode.HARD) != null && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_2, BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_3, BossRushManager.BossRushCourseMode.HARD) != null && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_A_3, BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_B_1, BossRushManager.BossRushCourseMode.HARD) != null) && this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_B_1, BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted;
		}

		private bool CheckToUnlockCourseD1(PersistentManager.PublicSlotData data)
		{
			bool flag = this.courseData.Data.ContainsKey(BossRushManager.BossRushCourseId.COURSE_D_1) && this.courseData.Data[BossRushManager.BossRushCourseId.COURSE_D_1].HasAnyModeUnlocked();
			if (flag)
			{
				return false;
			}
			string key = BossRushManager.BossRushCourseId.COURSE_D_1.ToString() + "_UNLOCKED";
			if (this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_C_1, BossRushManager.BossRushCourseMode.NORMAL) == null || !this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_C_1, BossRushManager.BossRushCourseMode.NORMAL).WasTheCourseCompleted || this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_C_1, BossRushManager.BossRushCourseMode.HARD) == null || !this.GetHighScore(BossRushManager.BossRushCourseId.COURSE_C_1, BossRushManager.BossRushCourseMode.HARD).WasTheCourseCompleted)
			{
				return false;
			}
			bool result = false;
			if (data.flags != null)
			{
				if (data.flags.flags.ContainsKey(key))
				{
					result = data.flags.flags[key];
				}
				else if (data.flags.flags.ContainsKey("PONTIFF_HUSK_DEFEATED"))
				{
					result = data.flags.flags["PONTIFF_HUSK_DEFEATED"];
				}
			}
			return result;
		}

		private const string HUB_SCENE = "D22Z01S00";

		private const string LAUDES_FLAG = "SANTOS_LAUDES_DEFEATED";

		private const string HUSK_FLAG = "PONTIFF_HUSK_DEFEATED";

		public const string COURSE_FLAG_SUFFIX = "_UNLOCKED";

		public const string HEALTH_FONT_FLAG = "BOSSRUSH_HEALTH_FONT";

		public const string FERVOUR_FONT_FLAG = "BOSSRUSH_FERVOUR_FONT";

		private const int BOSSRUSH_SLOT = 7;

		private Dictionary<BossRushManager.BossRushCourseId, BossRushCourse> courseScenesByCourseId = new Dictionary<BossRushManager.BossRushCourseId, BossRushCourse>();

		private BossRushManager.BossRushCourseId currentCourseId;

		private BossRushManager.BossRushCourseMode currentCourseMode;

		private int currentCourseSceneIndex;

		private BossRushHighScore currentScore;

		private int SourceSlot = -1;

		private const string FADE_TO_WHITE_SOUND = "event:/SFX/UI/FadeToWhite";

		private BossRushManager.BossRushData courseData = new BossRushManager.BossRushData();

		private LocalDataFile configFile;

		public enum BossRushCourseId
		{
			COURSE_A_1,
			COURSE_A_2,
			COURSE_A_3,
			COURSE_B_1 = 10,
			COURSE_C_1 = 20,
			COURSE_D_1 = 30
		}

		public enum BossRushCourseMode
		{
			NORMAL,
			HARD
		}

		public enum BossRushCourseScore
		{
			F_MINUS,
			F,
			F_PLUS,
			E_MINUS,
			E,
			E_PLUS,
			D_MINUS,
			D,
			D_PLUS,
			C_MINUS,
			C,
			C_PLUS,
			B_MINUS,
			B,
			B_PLUS,
			A_MINUS,
			A,
			A_PLUS,
			S_MINUS,
			S,
			S_PLUS
		}

		[Serializable]
		private class CourseData
		{
			public BossRushHighScore GetHighScore(BossRushManager.BossRushCourseMode mode)
			{
				BossRushHighScore result = null;
				if (this.highScores.ContainsKey(mode))
				{
					result = this.highScores[mode];
				}
				return result;
			}

			public BossRushHighScore GetPrevHighScore(BossRushManager.BossRushCourseMode mode)
			{
				BossRushHighScore result = null;
				if (this.prevHighScores.ContainsKey(mode))
				{
					result = this.prevHighScores[mode];
				}
				return result;
			}

			public void SetHighScoreIfBetter(BossRushManager.BossRushCourseMode mode, BossRushHighScore score)
			{
				if (!score.WasTheCourseCompleted)
				{
					return;
				}
				BossRushHighScore highScore = this.GetHighScore(mode);
				bool flag = score.IsBetterThan(highScore);
				if (flag)
				{
					this.prevHighScores[mode] = highScore;
					this.highScores[mode] = score;
				}
				score.IsNewHighScore = flag;
			}

			public bool HasAnyModeUnlocked()
			{
				bool flag = false;
				foreach (KeyValuePair<BossRushManager.BossRushCourseMode, bool> keyValuePair in this.unlocked)
				{
					flag = (flag || keyValuePair.Value);
				}
				return flag;
			}

			public bool IsModeUnlocked(BossRushManager.BossRushCourseMode mode)
			{
				bool result = false;
				if (this.unlocked.ContainsKey(mode))
				{
					result = this.unlocked[mode];
				}
				return result;
			}

			public Dictionary<BossRushManager.BossRushCourseMode, BossRushHighScore> highScores = new Dictionary<BossRushManager.BossRushCourseMode, BossRushHighScore>();

			public Dictionary<BossRushManager.BossRushCourseMode, BossRushHighScore> prevHighScores = new Dictionary<BossRushManager.BossRushCourseMode, BossRushHighScore>();

			public Dictionary<BossRushManager.BossRushCourseMode, bool> unlocked = new Dictionary<BossRushManager.BossRushCourseMode, bool>();
		}

		[Serializable]
		private class BossRushData : ILocalData
		{
			public string GetFileName()
			{
				return "BossRushData.config";
			}

			public void Clean()
			{
				this.Data.Clear();
			}

			public bool UnlockCourse(BossRushManager.BossRushCourseId course, BossRushManager.BossRushCourseMode mode = BossRushManager.BossRushCourseMode.NORMAL)
			{
				if (!this.Data.ContainsKey(course))
				{
					this.Data[course] = new BossRushManager.CourseData();
				}
				if (!this.Data[course].unlocked.ContainsKey(mode))
				{
					this.Data[course].unlocked[mode] = false;
				}
				bool result = !this.Data[course].unlocked[mode] && mode == BossRushManager.BossRushCourseMode.HARD;
				this.Data[course].unlocked[mode] = true;
				return result;
			}

			public Dictionary<BossRushManager.BossRushCourseId, BossRushManager.CourseData> Data = new Dictionary<BossRushManager.BossRushCourseId, BossRushManager.CourseData>();
		}
	}
}
