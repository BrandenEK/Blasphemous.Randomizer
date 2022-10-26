using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class DebugConfig
	{
		public DebugConfig(int type)
		{
			this.type = type;
		}

		public int type;
	}
}
