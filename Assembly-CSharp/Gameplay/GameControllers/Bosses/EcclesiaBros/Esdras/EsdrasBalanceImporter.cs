using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras
{
	public class EsdrasBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetMeleeLightAttackDamage();
			this.SetLightningAttackDamage();
			this.SetMeleeHeavyAttackDamage();
			this.SetSpinDashAttack();
			this.SetMeleeSingleSpinAttackDamage();
			this.SetInstantLightningAttackDamage();
			this.SetProjectileDamage();
		}

		private void SetMeleeLightAttackDamage()
		{
			EsdrasBalanceImporter.SetEsdrasMeleeAttackDamage(this.MeleeLightAttack, base.GetLightAttackDamage);
		}

		private void SetMeleeHeavyAttackDamage()
		{
			EsdrasBalanceImporter.SetEsdrasMeleeAttackDamage(this.MeleeHeavyAttack, base.GetCriticalAttackDamage);
		}

		private void SetMeleeSingleSpinAttackDamage()
		{
			EsdrasBalanceImporter.SetEsdrasMeleeAttackDamage(this.MeleeSingleSpinAttack, base.GetHeavyAttackDamage);
		}

		private void SetSpinDashAttack()
		{
			EsdrasBalanceImporter.SetEsdrasMeleeAttackDamage(this.SpinDashAttack, base.GetMediumAttackDamage);
		}

		private void SetLightningAttackDamage()
		{
			if (this.LightningAttack == null)
			{
				return;
			}
			this.LightningAttack.SpawnedAreaAttackDamage = base.GetHeavyAttackDamage;
		}

		private void SetInstantLightningAttackDamage()
		{
			if (this.LightningInstaAttack == null)
			{
				return;
			}
			this.LightningInstaAttack.SpawnedAreaAttackDamage = base.GetHeavyAttackDamage;
		}

		private void SetProjectileDamage()
		{
			BossStraightProjectileAttack componentInChildren = this.bossEnemy.GetComponentInChildren<BossStraightProjectileAttack>();
			if (componentInChildren)
			{
				componentInChildren.SetProjectileWeaponDamage(base.GetLightAttackDamage);
			}
		}

		private static void SetEsdrasMeleeAttackDamage(IDirectAttack meleeAttack, int meleeAttackDamage)
		{
			if (meleeAttack != null)
			{
				meleeAttack.SetDamage(meleeAttackDamage);
			}
		}

		[SerializeField]
		protected EsdrasMeleeAttack MeleeLightAttack;

		[SerializeField]
		protected EsdrasMeleeAttack MeleeHeavyAttack;

		[SerializeField]
		protected EsdrasMeleeAttack MeleeSingleSpinAttack;

		[SerializeField]
		protected BossDashAttack SpinDashAttack;

		[SerializeField]
		protected BossAreaSummonAttack LightningAttack;

		[SerializeField]
		protected BossAreaSummonAttack LightningInstaAttack;
	}
}
