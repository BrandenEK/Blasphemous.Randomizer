using System;
using System.Collections.Generic;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.EditorScripts.EnemiesBalance
{
	public sealed class EnemyStatsImporter
	{
		public EnemyStatsImporter(List<EnemyBalanceItem> enemiesBalanceItems)
		{
			this.enemiesBalanceItems = enemiesBalanceItems;
		}

		public void SetEnemyStats(Enemy enemy)
		{
			if (!enemy || StringExtensions.IsNullOrWhitespace(enemy.Id))
			{
				Debug.LogWarning("Enemy Id is empty: " + enemy.gameObject.name);
				return;
			}
			EnemyBalanceItem balanceItemByName = this.GetBalanceItemByName(enemy.Id);
			if (balanceItemByName == null)
			{
				Debug.LogWarning("Enemy Id does not exist in the balance chart: " + enemy.Id);
				return;
			}
			enemy.Stats.Strength = new Strength(balanceItemByName.Strength, enemy.Stats.StrengthUpgrade, 1f);
			enemy.Stats.Life = new Life(balanceItemByName.LifeBase, enemy.Stats.LifeUpgrade, enemy.Stats.LifeMaximun, 1f);
			enemy.purgePointsWhenDead = (float)balanceItemByName.PurgePoints;
			EnemyAttack componentInChildren = enemy.GetComponentInChildren<EnemyAttack>();
			if (componentInChildren)
			{
				componentInChildren.ContactDamageAmount = balanceItemByName.ContactDamage;
			}
		}

		private EnemyBalanceItem GetBalanceItemByName(string enemyId)
		{
			EnemyBalanceItem result = null;
			string value = enemyId.Trim().ToLower();
			foreach (EnemyBalanceItem enemyBalanceItem in this.enemiesBalanceItems)
			{
				string text = enemyBalanceItem.Id.Trim().ToLower();
				if (text.Equals(value))
				{
					result = enemyBalanceItem;
					break;
				}
			}
			return result;
		}

		private List<EnemyBalanceItem> enemiesBalanceItems;

		private EnemyBalanceItem balanceItem;
	}
}
