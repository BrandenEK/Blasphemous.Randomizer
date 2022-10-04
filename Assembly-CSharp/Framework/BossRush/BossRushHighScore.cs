using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.BossRush
{
	[Serializable]
	public class BossRushHighScore
	{
		public BossRushHighScore(BossRushManager.BossRushCourseId currentCourseId, BossRushManager.BossRushCourseMode currentCourseMode)
		{
			this.CourseId = currentCourseId;
			this.CourseMode = currentCourseMode;
		}

		public void CalculateScoreObtained(bool completed)
		{
			this.WasTheCourseCompleted = completed;
			if (completed)
			{
				int minutes = (int)Math.Ceiling((double)this.RunDuration) / 60;
				this.Score = Core.BossRushManager.GetRankByTimePassed(this.CourseId, this.CourseMode, minutes);
			}
			else
			{
				this.Score = Core.BossRushManager.GetRankByNumberOfCompletedBossfights(this.CourseId, this.NumScenesCompleted);
			}
		}

		public string RunDurationInString()
		{
			int num = Mathf.FloorToInt(this.RunDuration) / 60;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.RunDuration);
			int seconds = timeSpan.Seconds;
			int num2 = timeSpan.Milliseconds / 10;
			if (num < 100)
			{
				return string.Format("{0:D2} : {1:D2} : {2:D2}", num, seconds, num2);
			}
			return string.Format("{0:D3} : {1:D2} : {2:D2}", num, seconds, num2);
		}

		public void UpdateTimer()
		{
			if (!this.IsTimerActive)
			{
				return;
			}
			this.RunDuration += Time.deltaTime;
		}

		public bool IsBetterThan(BossRushHighScore other)
		{
			bool flag = true;
			if (other != null)
			{
				Debug.Log("**** IsBetterThan Score");
				Debug.Log(string.Concat(new object[]
				{
					"* Base: ",
					this.Score,
					"  Duration:",
					this.RunDuration
				}));
				Debug.Log(string.Concat(new object[]
				{
					"* Other: ",
					other.Score,
					"  Duration:",
					other.RunDuration
				}));
				if (this.Score == other.Score)
				{
					flag = (this.RunDuration <= other.RunDuration);
				}
				else
				{
					flag = (this.Score > other.Score);
				}
				Debug.Log("*** IS BETTER: " + flag);
			}
			return flag;
		}

		public BossRushManager.BossRushCourseId CourseId;

		public BossRushManager.BossRushCourseMode CourseMode;

		public int NumFlasksUsed;

		public int NumDodgesAchieved;

		public int NumPrayersUsed;

		public int NumBloodPenancesUsed;

		public int NumHitsReceived;

		public float RunDuration;

		public int NumScenesCompleted;

		public bool IsTimerActive;

		public BossRushManager.BossRushCourseScore Score;

		public bool IsNewHighScore;

		public bool WasTheCourseCompleted;
	}
}
