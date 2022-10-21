using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class EnemyConfig
	{
		public EnemyConfig(int type, bool area)
		{
			this.type = type;
			this.areaScaling = area;
		}

		public int type;

		public bool areaScaling;
	}
}
