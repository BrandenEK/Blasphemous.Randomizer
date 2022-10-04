using System;
using System.Collections.Generic;
using System.Linq;
using Framework.FrameworkCore.Attributes;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Framework.EditorScripts.BossesBalance
{
	[RequireComponent(typeof(Enemy))]
	public abstract class BossBalanceImporter : MonoBehaviour
	{
		private void Awake()
		{
			this.bossEnemy = base.GetComponent<Enemy>();
		}

		private void Start()
		{
			this.LoadStats();
			this.ApplyLoadedStats();
			this.OnStart();
		}

		protected virtual void OnStart()
		{
			this.SetLifeStat();
			this.SetStrengthStat();
			this.SetPurgePoints();
		}

		private void LoadStats()
		{
			List<Dictionary<string, object>> bossesBalance = Core.GameModeManager.GetCurrentBossesBalanceChart().BossesBalance;
			using (IEnumerator<Dictionary<string, object>> enumerator = (from bossBalanceItem in bossesBalance
			where this.bossEnemy.Id.Equals(bossBalanceItem["Id"])
			select bossBalanceItem).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Dictionary<string, object> dictionary = enumerator.Current;
					this.bossLoadedStats = dictionary;
				}
			}
		}

		public virtual void SetLifeStat()
		{
			this.bossEnemy.Stats.Life = new Life(float.Parse(this.bossLoadedStats["Life Base"].ToString()), this.bossEnemy.Stats.LifeUpgrade, float.Parse(this.bossLoadedStats["Life Base"].ToString()), 1f);
		}

		private void SetPurgePoints()
		{
			if (this.bossEnemy)
			{
				this.bossEnemy.purgePointsWhenDead = (float)this.GetPurgePoints;
			}
		}

		private void SetStrengthStat()
		{
			this.bossEnemy.Stats.Strength = new Strength(float.Parse(this.bossLoadedStats["Strength"].ToString()), this.bossEnemy.Stats.StrengthUpgrade, 1f);
		}

		protected int GetLifeBase
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Life Base"].ToString());
			}
		}

		protected int GetLightAttackDamage
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Light Attack"].ToString());
			}
		}

		protected int GetMediumAttackDamage
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Medium Attack"].ToString());
			}
		}

		protected int GetHeavyAttackDamage
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Heavy Attack"].ToString());
			}
		}

		protected int GetCriticalAttackDamage
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Critical Attack"].ToString());
			}
		}

		protected int GetPurgePoints
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Purge Points"].ToString());
			}
		}

		protected int GetContactDamage
		{
			get
			{
				return int.Parse(this.bossLoadedStats["Contact Damage"].ToString());
			}
		}

		protected abstract void ApplyLoadedStats();

		protected const string LIFE_BASE_STAT = "Life Base";

		protected const string STRENGTH_BASE_STAT = "Strength";

		protected const string LIGHT_ATTACK = "Light Attack";

		protected const string MEDIUM_ATTACK = "Medium Attack";

		protected const string HEAVY_ATTACK = "Heavy Attack";

		protected const string CRITICAL_ATTACK = "Critical Attack";

		protected const string LIGHT_ATTACK_COOLDOWN = "L.A. Cooldown";

		protected const string MEDIUM_ATTACK_COOLDOWN = "M.A. Cooldown";

		protected const string HEAVY_ATTACK_COOLDOWN = "H.A. Cooldown";

		protected const string VULNERABLE_LAPSE = "Vulnerable Lapse";

		protected const string CONTACT_DAMAGE = "Contact Damage";

		protected const string PURGE_POINTS = "Purge Points";

		protected Enemy bossEnemy;

		protected Dictionary<string, object> bossLoadedStats;
	}
}
