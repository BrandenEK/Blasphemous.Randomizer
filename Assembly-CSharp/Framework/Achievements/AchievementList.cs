using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Achievements
{
	[Serializable]
	public class AchievementList : SerializedScriptableObject
	{
		public List<Achievement> achievementList = new List<Achievement>();
	}
}
