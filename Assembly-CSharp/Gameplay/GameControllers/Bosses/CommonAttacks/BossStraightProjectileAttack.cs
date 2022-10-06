using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.CommonAttacks
{
	public class BossStraightProjectileAttack : EnemyAttack, IProjectileAttack
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

		public StraightProjectile Shoot(Vector2 dir, Vector2 position, Vector2 offset, Vector3 rotation, float hitStrength = 1f)
		{
			StraightProjectile straightProjectile = this.Shoot(dir);
			straightProjectile.transform.position = position;
			straightProjectile.transform.position += offset;
			Quaternion rotation2 = default(Quaternion);
			rotation2.eulerAngles = rotation;
			straightProjectile.transform.rotation = rotation2;
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamageStrength(hitStrength);
			return straightProjectile;
		}

		public StraightProjectile Shoot(Vector2 dir, Vector2 position, Vector2 offset, float hitStrength = 1f)
		{
			StraightProjectile straightProjectile = this.Shoot(dir);
			straightProjectile.transform.position = position;
			straightProjectile.transform.position += offset;
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamageStrength(hitStrength);
			return straightProjectile;
		}

		public StraightProjectile Shoot(Vector2 dir, Vector2 offset, Vector3 rotation, float hitStrength = 1f)
		{
			StraightProjectile straightProjectile = this.Shoot(dir);
			straightProjectile.transform.position += offset;
			Quaternion rotation2 = default(Quaternion);
			rotation2.eulerAngles = rotation;
			straightProjectile.transform.rotation = rotation2;
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamageStrength(hitStrength);
			return straightProjectile;
		}

		public StraightProjectile Shoot(Vector2 dir, Vector2 offset, float hitStrength = 1f)
		{
			StraightProjectile straightProjectile = this.Shoot(dir);
			straightProjectile.transform.position += offset;
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamageStrength(hitStrength);
			return straightProjectile;
		}

		public StraightProjectile Shoot(Vector2 dir)
		{
			base.CurrentWeaponAttack();
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.projectilePrefab, this.projectileSource.position, Quaternion.identity, false, 1).GameObject;
			StraightProjectile straightProjectile = null;
			if (gameObject)
			{
				straightProjectile = gameObject.GetComponent<StraightProjectile>();
				if (!straightProjectile)
				{
					return straightProjectile;
				}
				straightProjectile.OriginalDamage = this.ProjectileDamageAmount;
				this.SetProjectileWeaponDamage(straightProjectile, this.ProjectileDamageAmount);
				straightProjectile.Init(straightProjectile.transform.position, straightProjectile.transform.position + dir, this.projectileSpeed);
				if (!this.muzzleFlashPrefab)
				{
					return straightProjectile;
				}
				float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
				PoolManager.Instance.ReuseObject(this.muzzleFlashPrefab, straightProjectile.transform.position, Quaternion.Euler(0f, 0f, num), false, 1);
			}
			return straightProjectile;
		}

		public void SetProjectileWeaponDamage(int damage)
		{
			this.ProjectileDamageAmount = damage;
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
			this.SetProjectileWeaponDamage(damage);
			if (!projectile)
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
