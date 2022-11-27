using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class EnemyConfig
	{
		public EnemyConfig(int type, bool maintain, bool area)
		{
			this.type = type;
			maintainClass = maintain;
			areaScaling = area;
		}

		public int type;
		public bool maintainClass;
		public bool areaScaling;
	}
}
