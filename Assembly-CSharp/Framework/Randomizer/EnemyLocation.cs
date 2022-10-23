using System;

namespace Framework.Randomizer
{
	public class EnemyLocation
	{
		public EnemyLocation(int location, string enemy, int type, bool arena)
		{
			this.locationId = location;
			this.enemyId = enemy;
			this.enemyType = type;
			this.arena = arena;
		}

		public int locationId;

		public string enemyId;

		public int enemyType;

		public bool arena;
	}
}
