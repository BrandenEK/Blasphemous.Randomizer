using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "EnemiesBalance", menuName = "Blasphemous/Enemies Balance")]
	public class EnemiesBalance : ScriptableObject
	{
		[ButtonGroup("Controls", 0)]
		[Button(ButtonSizes.Medium)]
		private void Load()
		{
			this.LoadResources(this.GroundEnemies);
			this.LoadResources(this.FlyingEnemies);
		}

		[ButtonGroup("Controls", 0)]
		[Button(ButtonSizes.Medium)]
		private void Apply()
		{
			this.ApplyChangesOnResources(this.GroundEnemies);
			this.ApplyChangesOnResources(this.FlyingEnemies);
		}

		private void LoadResources(EnemiesBalance.EnemyBalance[] enemyBalances)
		{
			for (int i = 0; i < enemyBalances.Length; i++)
			{
				if (!(enemyBalances[i].Enemy == null))
				{
					Enemy componentInChildren = enemyBalances[i].Enemy.GetComponentInChildren<Enemy>();
					if (componentInChildren)
					{
						EnemyAttack componentInChildren2 = componentInChildren.GetComponentInChildren<EnemyAttack>();
						EntityStats stats = componentInChildren.Stats;
						enemyBalances[i].LifeBase = stats.LifeBase;
						enemyBalances[i].StrengthBase = stats.StrengthBase;
						enemyBalances[i].PurgePoints = componentInChildren.purgePointsWhenDead;
						if (componentInChildren2)
						{
							enemyBalances[i].ContactDamage = componentInChildren2.ContactDamageAmount;
						}
					}
				}
			}
		}

		private void ApplyChangesOnResources(EnemiesBalance.EnemyBalance[] enemyBalances)
		{
		}

		[TabGroup("TabGroup", "Ground Enemies", false, 0)]
		public EnemiesBalance.EnemyBalance[] GroundEnemies;

		[TabGroup("TabGroup", "Flying Enemies", false, 0)]
		public EnemiesBalance.EnemyBalance[] FlyingEnemies;

		[Serializable]
		public struct EnemyBalance
		{
			[SerializeField]
			[InlineEditor(InlineEditorModes.LargePreview)]
			public GameObject Enemy;

			[SerializeField]
			[Range(0f, 1000f)]
			public float LifeBase;

			[SerializeField]
			[Range(0f, 1000f)]
			public float StrengthBase;

			[SerializeField]
			[Range(0f, 1000f)]
			public float PurgePoints;

			[SerializeField]
			[Range(0f, 1000f)]
			public float ContactDamage;
		}
	}
}
