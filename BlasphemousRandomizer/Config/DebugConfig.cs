using System;

namespace BlasphemousRandomizer.Config
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
