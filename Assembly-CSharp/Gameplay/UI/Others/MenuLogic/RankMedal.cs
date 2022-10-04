using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	[Serializable]
	public struct RankMedal
	{
		public BossRushManager.BossRushCourseScore score;

		public Sprite sprite;
	}
}
