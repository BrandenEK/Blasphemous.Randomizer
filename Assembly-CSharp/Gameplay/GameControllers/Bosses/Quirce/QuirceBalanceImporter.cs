using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce
{
	public class QuirceBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetInstantProjectileDamage();
			this.SetMovingInstantProjectileDamage();
			this.SetFireDashDamage();
			this.SetPlungeDamage();
			this.SetSplineThrowDamage();
			this.SetMeleeDashDamage();
			this.SetAreaSummonDamage();
			this.SetMultiTeleportPlungeDamage();
			this.SetLandingAreaSummonDamage();
		}

		private void SetInstantProjectileDamage()
		{
			this.SetDirectAttackDamage(this.InstantProjectile, base.GetHeavyAttackDamage);
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.InstantProjectile, base.GetLightAttackDamage);
		}

		private void SetMovingInstantProjectileDamage()
		{
			this.SetDirectAttackDamage(this.MovingInstantProjectile, base.GetLightAttackDamage);
		}

		private void SetFireDashDamage()
		{
			this.SetDirectAttackDamage(this.FireDash, base.GetCriticalAttackDamage);
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.FireDash, base.GetHeavyAttackDamage);
		}

		private void SetPlungeDamage()
		{
			this.SetDirectAttackDamage(this.Plunge, base.GetMediumAttackDamage);
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.Plunge, base.GetMediumAttackDamage);
		}

		private void SetSplineThrowDamage()
		{
			this.SetProjectileAttackDamage(this.SplineThrow, base.GetMediumAttackDamage);
		}

		private void SetMeleeDashDamage()
		{
			this.SetDirectAttackDamage(this.MeleeDash, base.GetHeavyAttackDamage);
		}

		private void SetAreaSummonDamage()
		{
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.AreaSummon, base.GetMediumAttackDamage);
		}

		private void SetMultiTeleportPlungeDamage()
		{
			this.SetDirectAttackDamage(this.MultiTeleportPlunge, base.GetMediumAttackDamage);
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.MultiTeleportPlunge, base.GetMediumAttackDamage);
		}

		private void SetLandingAreaSummonDamage()
		{
			QuirceBalanceImporter.SetSpawnerAttackDamage(this.LandingAreaSummon, base.GetMediumAttackDamage);
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
				Debug.LogError("QuirceBalanceImporter::SetDirectAttackDamage: IDirectAttack not found in attackGO: " + attackGO + "!");
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
				Debug.LogError("QuirceBalanceImporter::SetProjectileAttackDamage: IProjectileAttack not found in attackGO: " + attackGO + "!");
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
				Debug.LogError("QuirceBalanceImporter::SetSpawnerAttackDamage: ISpawnerAttack not found in attackGO: " + attackGO + "!");
			}
		}

		[SerializeField]
		protected GameObject InstantProjectile;

		[SerializeField]
		protected GameObject MovingInstantProjectile;

		[SerializeField]
		protected GameObject FireDash;

		[SerializeField]
		protected GameObject Plunge;

		[SerializeField]
		protected GameObject SplineThrow;

		[SerializeField]
		protected GameObject MeleeDash;

		[SerializeField]
		protected GameObject AreaSummon;

		[SerializeField]
		protected GameObject MultiTeleportPlunge;

		[SerializeField]
		protected GameObject LandingAreaSummon;
	}
}
