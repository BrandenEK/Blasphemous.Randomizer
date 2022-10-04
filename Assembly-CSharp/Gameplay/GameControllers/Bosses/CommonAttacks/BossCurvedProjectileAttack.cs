using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.CommonAttacks
{
	public class BossCurvedProjectileAttack : EnemyAttack, IProjectileAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.projectilePrefab, this.poolSize);
			if (this.muzzleFlashPrefab != null)
			{
				PoolManager.Instance.CreatePool(this.muzzleFlashPrefab, this.poolSize);
			}
		}

		public void Clear()
		{
		}

		public CurvedProjectile Shoot(Vector2 target)
		{
			base.CurrentWeaponAttack();
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.projectilePrefab, this.projectileSource.position, Quaternion.identity, false, 1).GameObject;
			CurvedProjectile curvedProjectile = null;
			if (gameObject)
			{
				curvedProjectile = gameObject.GetComponent<CurvedProjectile>();
				if (!curvedProjectile)
				{
					return curvedProjectile;
				}
				curvedProjectile.OriginalDamage = this.ProjectileDamageAmount;
				this.SetProjectileWeaponDamage(curvedProjectile, this.ProjectileDamageAmount);
				curvedProjectile.Init(curvedProjectile.transform.position, target, this.projectileSpeed);
				if (!this.muzzleFlashPrefab)
				{
					return curvedProjectile;
				}
				Vector2 vector = target - curvedProjectile.transform.position;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				PoolManager.Instance.ReuseObject(this.muzzleFlashPrefab, curvedProjectile.transform.position, Quaternion.Euler(0f, 0f, z), false, 1);
			}
			return curvedProjectile;
		}

		public void SetProjectileWeaponDamage(int damage)
		{
			if (damage > 0)
			{
				this.ProjectileDamageAmount = damage;
			}
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
			this.SetProjectileWeaponDamage(damage);
			if (damage <= 0 || !projectile)
			{
				return;
			}
			ProjectileWeapon componentInChildren = projectile.GetComponentInChildren<ProjectileWeapon>();
			if (componentInChildren)
			{
				componentInChildren.SetDamage(damage);
			}
		}

		public GameObject muzzleFlashPrefab;

		public GameObject projectilePrefab;

		public Transform projectileSource;

		public int ProjectileDamageAmount;

		public int poolSize = 3;

		public float projectileSpeed;

		public Vector2 targetOffset;
	}
}
