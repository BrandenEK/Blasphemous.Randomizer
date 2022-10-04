using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.WallEnemy.Attack
{
	public class WallEnemyAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; set; }

		public WallEnemyWeapon Weapon { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<WallEnemyWeapon>();
			this.Weapon = (WallEnemyWeapon)base.CurrentEnemyWeapon;
			this._wallEnemyHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				Force = this.Force,
				DamageType = this.DamageType,
				HitSoundId = this.HitSound,
				ThrowbackDirByOwnerPosition = true,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._wallEnemyHit);
		}

		private Hit _wallEnemyHit;
	}
}
