using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.Attack
{
	public class PatrollingFlyingEnemyAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; set; }

		public bool EntityAttacked { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		private void OnDeath()
		{
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			float final = base.EntityOwner.Stats.Strength.Final;
			Hit weapondHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageType = damageType,
				DamageAmount = final,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}
	}
}
