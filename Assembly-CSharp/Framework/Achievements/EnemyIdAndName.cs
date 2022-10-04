using System;
using Sirenix.OdinInspector;

namespace Framework.Achievements
{
	[Serializable]
	public struct EnemyIdAndName
	{
		public string id;

		public string name;

		public bool hasAnotherName;

		[ShowIf("hasAnotherName", true)]
		public string otherName;
	}
}
