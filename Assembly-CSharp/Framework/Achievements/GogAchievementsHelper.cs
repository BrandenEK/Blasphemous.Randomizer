using System;

namespace Framework.Achievements
{
	public class GogAchievementsHelper : IAchievementsHelper
	{
		public GogAchievementsHelper()
		{
			this.gogInit();
		}

		private void gogInit()
		{
		}

		public void SetAchievementProgress(string Id, float value)
		{
		}

		public void GetAchievementProgress(string Id, GetAchievementOperationEvent evt)
		{
		}

		private bool isOnline;

		private bool statsValid;
	}
}
