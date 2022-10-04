using System;

namespace Framework.EditorScripts.EnemiesBalance
{
	[Serializable]
	public class EnemyBalanceItem
	{
		public string Id;

		public string Name;

		public float LifeBase;

		public float Strength;

		public float ContactDamage;

		public int PurgePoints;
	}
}
