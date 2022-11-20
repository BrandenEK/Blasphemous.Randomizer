using System;

namespace BlasphemousRandomizer.Config
{
	[Serializable]
	public class DoorConfig
	{
		public DoorConfig(int type)
		{
			this.type = type;
		}

		public int type;
	}
}
