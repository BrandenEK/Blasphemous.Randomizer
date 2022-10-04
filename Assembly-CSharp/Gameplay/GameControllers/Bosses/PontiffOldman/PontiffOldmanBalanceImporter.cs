using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman
{
	public class PontiffOldmanBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetMachinegunShooterDamage();
			this.SetLightningAttackDamage();
			this.SetLightningAttackInstantDamage();
			this.SetMagicProjectileDamage();
			this.SetToxicProjectileDamage();
		}

		private void SetMachinegunShooterDamage()
		{
			this.SetProjectileAttackDamage(this.MachinegunShooter, base.GetHeavyAttackDamage);
		}

		private void SetLightningAttackDamage()
		{
			PontiffOldmanBalanceImporter.SetSpawnerAttackDamage(this.LightningAttack, base.GetLightAttackDamage);
		}

		private void SetLightningAttackInstantDamage()
		{
			PontiffOldmanBalanceImporter.SetSpawnerAttackDamage(this.LightningAttackInstant, base.GetLightAttackDamage);
		}

		private void SetMagicProjectileDamage()
		{
			this.SetProjectileAttackDamage(this.MagicProjectile, base.GetCriticalAttackDamage);
		}

		private void SetToxicProjectileDamage()
		{
			this.SetProjectileAttackDamage(this.ToxicProjectile, base.GetMediumAttackDamage);
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
				Debug.LogError("PontiffOldmanBalanceImporter::SetProjectileAttackDamage: IProjectileAttack not found in attackGO: " + attackGO + "!");
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
				Debug.LogError("PontiffOldmanBalanceImporter::SetSpawnerAttackDamage: ISpawnerAttack not found in attackGO: " + attackGO + "!");
			}
		}

		[SerializeField]
		protected GameObject MachinegunShooter;

		[SerializeField]
		protected GameObject LightningAttack;

		[SerializeField]
		protected GameObject LightningAttackInstant;

		[SerializeField]
		protected GameObject MagicProjectile;

		[SerializeField]
		protected GameObject ToxicProjectile;
	}
}
