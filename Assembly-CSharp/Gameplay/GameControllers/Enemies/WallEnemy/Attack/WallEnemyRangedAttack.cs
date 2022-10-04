using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WallEnemy.Attack
{
	public class WallEnemyRangedAttack : EnemyAttack
	{
		public WallEnemyWeapon Weapon { get; private set; }

		public RootMotionDriver FiringPosition { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<WallEnemyWeapon>();
			this.FiringPosition = base.EntityOwner.GetComponentInChildren<RootMotionDriver>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Weapon = (WallEnemyWeapon)base.CurrentEnemyWeapon;
			this._wallEnemyHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = DamageArea.DamageType.Normal,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
			if (this.Projectile)
			{
				PoolManager.Instance.CreatePool(this.Projectile, 2);
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._wallEnemyHit);
			this.FireProjectile();
		}

		private void FireProjectile()
		{
			if (!this.Projectile)
			{
				return;
			}
			Vector2 fireProjectilePosition = this.GetFireProjectilePosition();
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.Projectile, fireProjectilePosition, Quaternion.identity, false, 1);
			WallEnemyProjectile componentInChildren = objectInstance.GameObject.GetComponentInChildren<WallEnemyProjectile>();
			if (componentInChildren)
			{
				componentInChildren.SetOwner(base.EntityOwner);
			}
		}

		private Vector2 GetFireProjectilePosition()
		{
			return this.FiringPosition.transform.position;
		}

		public GameObject Projectile;

		private Hit _wallEnemyHit;
	}
}
