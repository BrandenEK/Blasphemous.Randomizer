using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.Attack
{
	public class HomingTurretAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.InitPool();
		}

		private void InitPool()
		{
			if (!this.ProjectileWeapon)
			{
				return;
			}
			PoolManager.Instance.CreatePool(this.ProjectileWeapon.gameObject, 5);
		}

		private void SetupProjectile(ProjectileWeapon projectileWeapon)
		{
			if (!projectileWeapon)
			{
				return;
			}
			projectileWeapon.WeaponOwner = base.EntityOwner;
			projectileWeapon.SetOwner(base.EntityOwner.gameObject);
			projectileWeapon.SetDamage((int)base.EntityOwner.Stats.Strength.Final);
		}

		public HomingProjectile FireProjectileToPenitent()
		{
			return this.FireProjectileToTarget(Core.Logic.Penitent.GetPosition());
		}

		public HomingProjectile FireProjectileToTarget(Vector3 target)
		{
			if (!this.ProjectileWeapon)
			{
				return null;
			}
			Transform transform = (!this.UseEntityPosition) ? this.ShootingPoint : base.EntityOwner.transform;
			Vector3 vector = transform.position + this.OffsetPosition;
			if (this.UseEntityOrientation)
			{
				vector.x += (float)((base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1 : 0);
			}
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.ProjectileWeapon.gameObject, vector, Quaternion.identity, false, 1);
			ProjectileWeapon component = objectInstance.GameObject.GetComponent<ProjectileWeapon>();
			this.SetupProjectile(component);
			HomingProjectile component2 = objectInstance.GameObject.GetComponent<HomingProjectile>();
			component2.currentDirection = ((target - vector).normalized + this.InitialDirectionAddendum).normalized;
			return component2;
		}

		[FoldoutGroup("Projectile Settings", 0)]
		public ProjectileWeapon ProjectileWeapon;

		[FoldoutGroup("Projectile Settings", 0)]
		public Vector3 OffsetPosition;

		[Tooltip("This vector is added to the direction to TPO in order to calculate the initital direction of the projectile.")]
		[FoldoutGroup("Projectile Settings", 0)]
		public Vector2 InitialDirectionAddendum;

		[FoldoutGroup("Projectile Settings", 0)]
		public bool UseEntityPosition = true;

		[FoldoutGroup("Projectile Settings", 0)]
		public bool UseEntityOrientation = true;

		[HideIf("UseEntityPosition", true)]
		[FoldoutGroup("Projectile Settings", 0)]
		public Transform ShootingPoint;
	}
}
