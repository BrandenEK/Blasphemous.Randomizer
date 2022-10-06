using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Environment.Traps.Turrets;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurnfaceBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetHomingBallsDamage();
			this.SetBeamAttackDamage();
			this.SetRosaryBeadsDamage();
		}

		private void SetBeamAttackDamage()
		{
			foreach (BurntFaceBeamAttack directAttack in this.BeamAttacks)
			{
				BurnfaceBalanceImporter.SetDirectAttackDamage(directAttack, base.GetHeavyAttackDamage);
			}
		}

		private void SetHomingBallsDamage()
		{
			foreach (BossStraightProjectileAttack projectileAttack in this.HomingBallAttacks)
			{
				BurnfaceBalanceImporter.SetProjectileAttackDamage(projectileAttack, base.GetMediumAttackDamage);
			}
		}

		private void SetRosaryBeadsDamage()
		{
			foreach (BasicTurret projectileAttack in Object.FindObjectsOfType<BasicTurret>())
			{
				BurnfaceBalanceImporter.SetProjectileAttackDamage(projectileAttack, base.GetLightAttackDamage);
			}
		}

		private static void SetDirectAttackDamage(IDirectAttack directAttack, int damage)
		{
			if (directAttack == null)
			{
				return;
			}
			directAttack.SetDamage(damage);
		}

		private static void SetProjectileAttackDamage(IProjectileAttack projectileAttack, int damage)
		{
			if (projectileAttack == null)
			{
				return;
			}
			projectileAttack.SetProjectileWeaponDamage(damage);
		}

		[SerializeField]
		protected BossStraightProjectileAttack[] HomingBallAttacks;

		[SerializeField]
		protected BurntFaceBeamAttack[] BeamAttacks;
	}
}
