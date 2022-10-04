using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.BossRush
{
	[Serializable]
	public struct ScoreInterval
	{
		public BossRushManager.BossRushCourseScore score;

		[MinMaxSlider(0f, 300f, true)]
		public Vector2 timeRangeInMinutes;
	}
}
