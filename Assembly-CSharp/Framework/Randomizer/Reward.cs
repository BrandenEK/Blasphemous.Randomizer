using System;

namespace Framework.Randomizer
{
	public class Reward
	{
		public Reward(int type, int id)
		{
			this.type = type;
			this.id = id;
		}

		public Reward(int type, int id, bool progression)
		{
			this.type = type;
			this.id = id;
			this.progression = progression;
		}

		public int id;

		public int type;

		public bool progression;
	}
}
