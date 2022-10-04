using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras
{
	public class LegionaryBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetLightningSummonAttack();
			this.SetMeleeSingleSpinAttackDamage();
			this.SetMeleeHeavyAttackDamage();
		}

		private void SetLightningSummonAttack()
		{
			if (!this.LightningSummonAttack)
			{
				return;
			}
			this.LightningSummonAttack.SpawnedAreaAttackDamage = base.GetHeavyAttackDamage;
		}

		private void SetMeleeHeavyAttackDamage()
		{
			LegionaryBalanceImporter.SetLegionaryMeleeAttackDamage(this.MeleeHeavyAttack, base.GetMediumAttackDamage);
		}

		private void SetMeleeSingleSpinAttackDamage()
		{
			LegionaryBalanceImporter.SetLegionaryMeleeAttackDamage(this.MeleeSingleSpinAttack, base.GetLightAttackDamage);
		}

		private static void SetLegionaryMeleeAttackDamage(IDirectAttack meleeAttack, int meleeAttackDamage)
		{
			if (meleeAttack != null)
			{
				meleeAttack.SetDamage(meleeAttackDamage);
			}
		}

		[SerializeField]
		protected BossAreaSummonAttack LightningSummonAttack;

		[SerializeField]
		protected EsdrasMeleeAttack MeleeSingleSpinAttack;

		[SerializeField]
		protected EsdrasMeleeAttack MeleeHeavyAttack;
	}
}
