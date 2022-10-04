using System;
using Framework.EditorScripts.BossesBalance;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Crisanta
{
	public class CrisantaBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetShockWaveAttackDamage();
			this.SetInstaLightningAttackDamage();
			this.SetMeleeLightAttackDamage();
			this.SetMeleeHeavyAttackDamage();
			this.SetSlashDashAttackDamage();
			this.SetProjectileDamage();
			this.SetDiagonalSlashDashAttackDamage();
			this.SetContactDamage();
		}

		public override void SetLifeStat()
		{
			float num = (!this.Crisanta.IsCrisantaRedux) ? 1f : 1.5f;
			this.bossEnemy.Stats.Life = new Life(float.Parse(this.bossLoadedStats["Life Base"].ToString()) * num, this.bossEnemy.Stats.LifeUpgrade, float.Parse(this.bossLoadedStats["Life Base"].ToString()) * num, 1f);
		}

		private void SetShockWaveAttackDamage()
		{
			this.SetSummonAttackAreaDamage(this.ShockWaveAttack, base.GetHeavyAttackDamage);
		}

		private void SetInstaLightningAttackDamage()
		{
			this.SetSummonAttackAreaDamage(this.InstaLightningAttack, base.GetMediumAttackDamage);
		}

		private void SetMeleeLightAttackDamage()
		{
			CrisantaBalanceImporter.SetDirectAttackDamage(this.MeleeLightAttack, base.GetMediumAttackDamage);
		}

		private void SetMeleeHeavyAttackDamage()
		{
			CrisantaBalanceImporter.SetDirectAttackDamage(this.MeleeHeavyAttack, base.GetHeavyAttackDamage);
		}

		private void SetSlashDashAttackDamage()
		{
			CrisantaBalanceImporter.SetDirectAttackDamage(this.SlashDash, base.GetHeavyAttackDamage);
		}

		private void SetDiagonalSlashDashAttackDamage()
		{
			CrisantaBalanceImporter.SetDirectAttackDamage(this.DiagonalSlashDash, base.GetCriticalAttackDamage);
		}

		private void SetProjectileDamage()
		{
			CrisantaBalanceImporter.SetProjectileAttackDamage(this.ProjectileAttack, base.GetLightAttackDamage);
		}

		private static void SetDirectAttackDamage(IDirectAttack directAttack, int damage)
		{
			if (directAttack == null)
			{
				return;
			}
			directAttack.SetDamage(damage);
		}

		private void SetSummonAttackAreaDamage(BossAreaSummonAttack attackArea, int damage)
		{
			if (attackArea == null)
			{
				return;
			}
			attackArea.SpawnedAreaAttackDamage = damage;
		}

		private static void SetProjectileAttackDamage(IProjectileAttack projectileAttack, int damage)
		{
			if (projectileAttack == null)
			{
				return;
			}
			projectileAttack.SetProjectileWeaponDamage(damage);
		}

		protected void SetContactDamage()
		{
			foreach (EnemyAttack enemyAttack in this.bossEnemy.GetComponentsInChildren<EnemyAttack>())
			{
				enemyAttack.ContactDamageAmount = (float)base.GetContactDamage;
			}
		}

		[SerializeField]
		protected Crisanta Crisanta;

		[SerializeField]
		protected BossAreaSummonAttack ShockWaveAttack;

		[SerializeField]
		protected BossAreaSummonAttack InstaLightningAttack;

		[SerializeField]
		protected CrisantaMeleeAttack MeleeLightAttack;

		[SerializeField]
		protected CrisantaMeleeAttack MeleeHeavyAttack;

		[SerializeField]
		protected BossDashAttack SlashDash;

		[SerializeField]
		protected BossDashAttack DiagonalSlashDash;

		[SerializeField]
		protected BossStraightProjectileAttack ProjectileAttack;
	}
}
