using System;
using Framework.EditorScripts.BossesBalance;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Bosses.PontiffSword;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant
{
	public class PontiffGiantBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetBeamDamage();
			this.SetLightningAttackInstantDamage();
			this.SetToxicProjectileDamage();
			this.SetMagicBulletsDamage();
			this.SetMagicShockwaveDamage();
			this.SetMachinegunShooterDamage();
			this.SetPontiffSwordLife();
			this.SetSwordMeleeAttackDamage();
		}

		private void SetBeamDamage()
		{
			PontiffGiantBalanceImporter.SetSpawnerAttackDamage(this.Beam, base.GetCriticalAttackDamage);
		}

		private void SetLightningAttackInstantDamage()
		{
			PontiffGiantBalanceImporter.SetSpawnerAttackDamage(this.LightningAttackInstant, base.GetLightAttackDamage);
		}

		private void SetToxicProjectileDamage()
		{
			this.SetProjectileAttackDamage(this.ToxicProjectile, base.GetLightAttackDamage);
		}

		private void SetMagicBulletsDamage()
		{
			this.SetProjectileAttackDamage(this.MagicBullets, base.GetMediumAttackDamage);
		}

		private void SetMagicShockwaveDamage()
		{
			PontiffGiantBalanceImporter.SetSpawnerAttackDamage(this.MagicShockwave, base.GetMediumAttackDamage);
		}

		private void SetMachinegunShooterDamage()
		{
			this.SetProjectileAttackDamage(this.MachinegunShooter, base.GetHeavyAttackDamage);
		}

		private void SetPontiffSwordLife()
		{
			this.SetEnemyLife(this.PontiffSword, (int)((float)base.GetLifeBase * this.SwordHealthPercentage));
		}

		private void SetSwordMeleeAttackDamage()
		{
			this.SetDirectAttackDamage(this.SwordMeleeAttack, (int)((float)base.GetLightAttackDamage * this.SwordDamagePercentage));
		}

		private void SetDirectAttackDamage(GameObject attackGO, int damage)
		{
			if (attackGO == null)
			{
				return;
			}
			IDirectAttack component = attackGO.GetComponent<IDirectAttack>();
			if (component != null)
			{
				component.SetDamage(damage);
			}
			else
			{
				Debug.LogError("PontiffGiantBalanceImporter::SetDirectAttackDamage: IDirectAttack not found in attackGO: " + attackGO + "!");
			}
		}

		private void SetProjectileAttackDamage(GameObject attackGO, int damage)
		{
			if (attackGO == null)
			{
				return;
			}
			IProjectileAttack component = attackGO.GetComponent<IProjectileAttack>();
			if (component != null)
			{
				component.SetProjectileWeaponDamage(damage);
			}
			else
			{
				Debug.LogError("PontiffGiantBalanceImporter::SetProjectileAttackDamage: IProjectileAttack not found in attackGO: " + attackGO + "!");
			}
		}

		private static void SetSpawnerAttackDamage(GameObject attackGO, int damage)
		{
			if (attackGO == null)
			{
				return;
			}
			ISpawnerAttack component = attackGO.GetComponent<ISpawnerAttack>();
			if (component != null)
			{
				component.SetSpawnsDamage(damage);
			}
			else
			{
				Debug.LogError("PontiffGiantBalanceImporter::SetSpawnerAttackDamage: ISpawnerAttack not found in attackGO: " + attackGO + "!");
			}
		}

		private void SetEnemyLife(Enemy enemy, int life)
		{
			if (enemy == null)
			{
				return;
			}
			enemy.Stats.Life = new Life((float)life, enemy.Stats.LifeUpgrade, (float)life, 1f);
		}

		[SerializeField]
		protected GameObject Beam;

		[SerializeField]
		protected GameObject LightningAttackInstant;

		[SerializeField]
		protected GameObject ToxicProjectile;

		[SerializeField]
		protected GameObject MagicBullets;

		[SerializeField]
		protected GameObject MagicShockwave;

		[SerializeField]
		protected GameObject MachinegunShooter;

		[SerializeField]
		protected PontiffSword PontiffSword;

		[SerializeField]
		protected GameObject SwordMeleeAttack;

		[DetailedInfoBox("Info", "This value is used to set the PontiffSword's max health value", 1, null)]
		public float SwordHealthPercentage = 0.19f;

		[DetailedInfoBox("Info", "This value is used to set the PontiffSword's attack value by using the PontiffGiant LightAttackDamage and multiplying that value by this coefficient.", 1, null)]
		public float SwordDamagePercentage = 0.5f;
	}
}
