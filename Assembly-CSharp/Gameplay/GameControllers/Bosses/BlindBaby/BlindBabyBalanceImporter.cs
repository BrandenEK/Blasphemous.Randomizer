using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BlindBaby
{
	public class BlindBabyBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetFallingProjectileDamage();
			this.SetBoomerangProjectileDamage();
			this.SetBouncingProjectileDamage();
			this.SetContactDamage();
		}

		private void SetFallingProjectileDamage()
		{
			BlindBabyBalanceImporter.SetProjectileDamage(this.FallingProjectile, base.GetHeavyAttackDamage);
		}

		private void SetBoomerangProjectileDamage()
		{
			BlindBabyBalanceImporter.SetProjectileDamage(this.BoomerangProjectile, base.GetLightAttackDamage);
		}

		private void SetBouncingProjectileDamage()
		{
			BlindBabyBalanceImporter.SetProjectileDamage(this.BouncingProjectile, base.GetMediumAttackDamage);
		}

		private void SetContactDamage()
		{
			Transform parent = this.bossEnemy.transform.parent;
			SimpleDamageArea[] componentsInChildren = parent.GetComponentsInChildren<SimpleDamageArea>();
			foreach (SimpleDamageArea simpleDamageArea in componentsInChildren)
			{
				simpleDamageArea.Damage = (float)base.GetContactDamage;
			}
		}

		private static void SetProjectileDamage(GameObject projectile, int projectileDamage)
		{
			if (projectile == null)
			{
				return;
			}
			IProjectileAttack component = projectile.GetComponent<IProjectileAttack>();
			if (component != null)
			{
				component.SetProjectileWeaponDamage(projectileDamage);
			}
		}

		[SerializeField]
		protected GameObject FallingProjectile;

		[SerializeField]
		protected GameObject BoomerangProjectile;

		[SerializeField]
		protected GameObject BouncingProjectile;
	}
}
