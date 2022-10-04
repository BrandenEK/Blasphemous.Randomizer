using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	public class PerpetuaBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetBossDashAttackDamage();
			this.SetSummonAttackDamage();
			this.SetProjectileAttackDamage();
		}

		private void SetBossDashAttackDamage()
		{
			if (!this.DashAttack)
			{
				return;
			}
			this.DashAttack.SetDamage(base.GetLightAttackDamage);
		}

		private void SetSummonAttackDamage()
		{
			if (!this.SummonAttack)
			{
				return;
			}
			this.SummonAttack.SetSpawnsDamage(base.GetHeavyAttackDamage);
		}

		private void SetProjectileAttackDamage()
		{
			if (!this.ProjectileAttack)
			{
				return;
			}
			this.ProjectileAttack.SetDamage(base.GetHeavyAttackDamage);
		}

		[SerializeField]
		protected BossDashAttack DashAttack;

		[SerializeField]
		protected BossAreaSummonAttack SummonAttack;

		[SerializeField]
		protected BossInstantProjectileAttack ProjectileAttack;
	}
}
