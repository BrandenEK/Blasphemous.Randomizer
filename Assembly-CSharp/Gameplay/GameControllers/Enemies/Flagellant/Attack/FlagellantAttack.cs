using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.Flagellant.Attack
{
	public class FlagellantAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._flagellant = (Flagellant)base.EntityOwner;
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			float final = this._flagellant.Stats.Strength.Final;
			Hit weapondHit = default(Hit);
			weapondHit.AttackingEntity = this._flagellant.gameObject;
			weapondHit.DamageType = this.DamageType;
			weapondHit.DamageAmount = final;
			weapondHit.HitSoundId = this.HitSound;
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private Flagellant _flagellant;
	}
}
