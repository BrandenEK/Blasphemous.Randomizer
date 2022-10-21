using System;
using System.Collections.Generic;
using Framework.FrameworkCore.Attributes;
using Framework.Managers;
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
			this.scalingFactor = 0.07f;
			this.difficultyRatings = new Dictionary<string, int>
			{
				{
					"D01Z01",
					1
				},
				{
					"D01Z03",
					2
				},
				{
					"D01Z04",
					2
				},
				{
					"D01Z05",
					3
				},
				{
					"D03Z01",
					3
				},
				{
					"D02Z01",
					3
				},
				{
					"D02Z02",
					4
				},
				{
					"D03Z02",
					4
				},
				{
					"D20Z01",
					5
				},
				{
					"D03Z03",
					6
				},
				{
					"D02Z03",
					6
				},
				{
					"D04Z01",
					7
				},
				{
					"D04Z02",
					7
				},
				{
					"D05Z01",
					8
				},
				{
					"D05Z02",
					8
				},
				{
					"D06Z01",
					9
				},
				{
					"D09Z01",
					10
				},
				{
					"D20Z02",
					11
				},
				{
					"EN01",
					2
				},
				{
					"EN02",
					2
				},
				{
					"EN03",
					4
				},
				{
					"EN04",
					4
				},
				{
					"EN05",
					2
				},
				{
					"EN06",
					3
				},
				{
					"EN07",
					6
				},
				{
					"EN08",
					3
				},
				{
					"EN09",
					8
				},
				{
					"EN10",
					8
				},
				{
					"EN11",
					6
				},
				{
					"EN12",
					6
				},
				{
					"EN13",
					2
				},
				{
					"EN14",
					4
				},
				{
					"EN15",
					2
				},
				{
					"EN16",
					7
				},
				{
					"EN17",
					4
				},
				{
					"EN18",
					3
				},
				{
					"EN20",
					4
				},
				{
					"EN21",
					6
				},
				{
					"EN22",
					5
				},
				{
					"EN23",
					8
				},
				{
					"EN24",
					3
				},
				{
					"EN26",
					7
				},
				{
					"EN27",
					6
				},
				{
					"EN28",
					3
				},
				{
					"EN29",
					7
				},
				{
					"EN31",
					9
				},
				{
					"EN32",
					3
				},
				{
					"EN33",
					10
				},
				{
					"EV01",
					8
				},
				{
					"EV02",
					7
				},
				{
					"EV03",
					3
				},
				{
					"EV05",
					8
				},
				{
					"EV08",
					1
				},
				{
					"EV10",
					1
				},
				{
					"EV11",
					1
				},
				{
					"EV12",
					3
				},
				{
					"EV13",
					3
				},
				{
					"EV14",
					6
				},
				{
					"EV15",
					4
				},
				{
					"EV17",
					6
				},
				{
					"EV18",
					8
				},
				{
					"EV19",
					10
				},
				{
					"EV20",
					8
				},
				{
					"EV21",
					4
				},
				{
					"EV22",
					8
				},
				{
					"EV23",
					9
				},
				{
					"EV24",
					7
				},
				{
					"EV26",
					8
				},
				{
					"EV27",
					9
				},
				{
					"EV29",
					9
				},
				{
					"EN201",
					11
				},
				{
					"EN202",
					11
				},
				{
					"EN203",
					11
				}
			};
		}

		public void SetEnemyStats(Enemy enemy)
		{
			if (!enemy || enemy.Id.IsNullOrWhitespace())
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
			string text = Core.LevelManager.currentLevel.LevelName;
			if (text.Substring(0, 6) != "D19Z01")
			{
				text = text.Substring(0, 6);
			}
			int rating = this.getRating(text);
			int rating2 = this.getRating(enemy.Id);
			float num = 0f;
			if (rating != 0 && rating2 != 0 && Core.Randomizer.gameConfig.enemies.type > 0 && Core.Randomizer.gameConfig.enemies.areaScaling)
			{
				num = (float)(rating - rating2) * this.scalingFactor;
			}
			enemy.Stats.Strength = new Strength(balanceItemByName.Strength + balanceItemByName.Strength * num, enemy.Stats.StrengthUpgrade, 1f);
			enemy.Stats.Life = new Life(balanceItemByName.LifeBase + balanceItemByName.LifeBase * num, enemy.Stats.LifeUpgrade, enemy.Stats.LifeMaximun, 1f);
			enemy.purgePointsWhenDead = (float)balanceItemByName.PurgePoints + (float)balanceItemByName.PurgePoints * num;
			EnemyAttack componentInChildren = enemy.GetComponentInChildren<EnemyAttack>();
			if (componentInChildren)
			{
				componentInChildren.ContactDamageAmount = balanceItemByName.ContactDamage + balanceItemByName.ContactDamage * num;
			}
		}

		private EnemyBalanceItem GetBalanceItemByName(string enemyId)
		{
			EnemyBalanceItem result = null;
			string value = enemyId.Trim().ToLower();
			foreach (EnemyBalanceItem enemyBalanceItem in this.enemiesBalanceItems)
			{
				if (enemyBalanceItem.Id.Trim().ToLower().Equals(value))
				{
					result = enemyBalanceItem;
					break;
				}
			}
			return result;
		}

		private int getRating(string id)
		{
			if (this.difficultyRatings.ContainsKey(id))
			{
				return this.difficultyRatings[id];
			}
			if (id == "D19Z01S01")
			{
				return 2;
			}
			if (id == "D19Z01S02")
			{
				return 3;
			}
			if (id == "D19Z01S03")
			{
				return 4;
			}
			if (id == "D19Z01S04")
			{
				return 6;
			}
			if (id == "D19Z01S05")
			{
				return 7;
			}
			if (id == "D19Z01S06")
			{
				return 8;
			}
			if (id == "D19Z01S07")
			{
				return 10;
			}
			Core.Randomizer.Log("Enemy/Area rating " + id + " does not exist!", 0);
			return 0;
		}

		private List<EnemyBalanceItem> enemiesBalanceItems;

		private EnemyBalanceItem balanceItem;

		private Dictionary<string, int> difficultyRatings;

		private float scalingFactor;
	}
}
