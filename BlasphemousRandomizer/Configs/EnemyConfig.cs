using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class EnemyConfig
	{
		public EnemyConfig(int type, bool group, bool area)
		{
			this.type = type;
			groupByType = group;
			areaScaling = area;
		}

		public int type;
		public bool groupByType;
		public bool areaScaling;
	}
}
