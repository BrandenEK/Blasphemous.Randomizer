using System;
using System.Collections.Generic;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.BossRush
{
	[Serializable]
	public class BossRushCourse : SerializedScriptableObject
	{
		public virtual List<string> GetScenes()
		{
			return this.Scenes;
		}

		public virtual List<string> GetFontRechargingScenes()
		{
			return this.FontRechargingScenes;
		}

		[Button(ButtonSizes.Small)]
		public void ResetDefaultMaxScoreForFailedRuns()
		{
			this.MaxScoreForFailedRuns = (BossRushManager.BossRushCourseScore)(this.Scenes.Count - 1);
		}

		private void ResetDefaultScores(List<ScoreInterval> scoresInterval)
		{
			scoresInterval.Clear();
			int num = 0;
			BossRushManager.BossRushCourseScore[] collection = (BossRushManager.BossRushCourseScore[])Enum.GetValues(typeof(BossRushManager.BossRushCourseScore));
			List<BossRushManager.BossRushCourseScore> list = new List<BossRushManager.BossRushCourseScore>(collection);
			list.Reverse();
			foreach (BossRushManager.BossRushCourseScore bossRushCourseScore in list)
			{
				if (bossRushCourseScore == this.MaxScoreForFailedRuns)
				{
					break;
				}
				ScoreInterval item = new ScoreInterval
				{
					score = bossRushCourseScore,
					timeRangeInMinutes = new Vector2((float)num, (float)(num + 1))
				};
				scoresInterval.Add(item);
				num++;
			}
		}

		[Button(ButtonSizes.Small)]
		public void ResetDefaultScoresInNormal()
		{
			this.ResetDefaultScores(this.ScoresByMinutesInNormal);
		}

		[Button(ButtonSizes.Small)]
		public void ResetDefaultScoresInHard()
		{
			this.ResetDefaultScores(this.ScoresByMinutesInHard);
		}

		private void PropagateIntervalModification(List<ScoreInterval> scoresInterval)
		{
			float num = 0f;
			for (int i = 0; i < scoresInterval.Count; i++)
			{
				float num2 = scoresInterval[i].timeRangeInMinutes[0];
				float num3 = scoresInterval[i].timeRangeInMinutes[1];
				if (num2 < num)
				{
					num3 += num - num2;
					num2 = num;
				}
				else if (num2 > num)
				{
					num3 -= num2 - num;
					num2 = num;
				}
				num = num3;
				ScoreInterval value = new ScoreInterval
				{
					score = scoresInterval[i].score,
					timeRangeInMinutes = new Vector2(num2, num3)
				};
				scoresInterval[i] = value;
			}
		}

		[Button(ButtonSizes.Small)]
		public void PropagateIntervalModificationInNormal()
		{
			this.PropagateIntervalModification(this.ScoresByMinutesInNormal);
		}

		[Button(ButtonSizes.Small)]
		public void PropagateIntervalModificationInHard()
		{
			this.PropagateIntervalModification(this.ScoresByMinutesInHard);
		}

		public bool IsRandomCourse;

		[SerializeField]
		[HideIf("IsRandomCourse", true)]
		private List<string> Scenes = new List<string>();

		[SerializeField]
		[HideIf("IsRandomCourse", true)]
		private List<string> FontRechargingScenes = new List<string>();

		public BossRushManager.BossRushCourseScore MaxScoreForFailedRuns;

		[FormerlySerializedAs("ScoresByMinutes")]
		public List<ScoreInterval> ScoresByMinutesInNormal;

		public List<ScoreInterval> ScoresByMinutesInHard;

		public GameObject healthFontRechargingVfx;

		public GameObject fervourFontRechargingVfx;
	}
}
