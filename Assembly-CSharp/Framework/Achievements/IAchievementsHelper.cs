using System;

namespace Framework.Achievements
{
	public interface IAchievementsHelper
	{
		void SetAchievementProgress(string id, float value);

		void GetAchievementProgress(string id, GetAchievementOperationEvent evt);
	}
}
