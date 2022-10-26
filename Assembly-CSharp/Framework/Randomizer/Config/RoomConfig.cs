using System;

namespace Framework.Randomizer.Config
{
	[Serializable]
	public class RoomConfig
	{
		public RoomConfig(int type)
		{
			this.type = type;
		}

		public int type;
	}
}
