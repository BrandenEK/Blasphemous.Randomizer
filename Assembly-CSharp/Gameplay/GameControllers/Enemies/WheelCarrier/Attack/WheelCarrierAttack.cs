using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.WheelCarrier.Attack
{
	public class WheelCarrierAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<WheelCarrierWeapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = this.DamageType,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
		}

		private Hit _weaponHit;
	}
}
