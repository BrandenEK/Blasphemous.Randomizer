using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class EnemyConfig
	{
		public EnemyConfig(int type, bool area)
		{
			this.type = type;
			areaScaling = area;
		}

		public int type;
		public bool areaScaling;
	}
}
