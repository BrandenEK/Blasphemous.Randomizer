using System;
using System.Diagnostics;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.CommonAttacks
{
	public class BossBoomerangProjectileAttack : EnemyAttack, IProjectileAttack
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnBoomerangReturnEvent;

		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.projectilePrefab, this.poolSize);
		}

		public void CreateHit()
		{
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = this.damage,
				DamageType = this.DamageType,
				DamageElement = this.DamageElement,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = this.unavoidable
			};
		}

		public void SetDamage(float damage)
		{
			this.damage = damage;
			this.CreateHit();
		}

		public void Shoot(Transform target)
		{
			base.CurrentWeaponAttack();
			this.Shoot(target.position);
		}

		public void Shoot(Vector2 target)
		{
			base.CurrentWeaponAttack();
			BoomerangProjectile component = PoolManager.Instance.ReuseObject(this.projectilePrefab, this.projectileSource.position, Quaternion.identity, false, 1).GameObject.GetComponent<BoomerangProjectile>();
			component.Init(component.transform.position, target + Vector3.up * 1.25f, 12f);
			this._weaponHit.AttackingEntity = component.gameObject;
			this.SetProjectileWeaponDamage(component, (int)this.damage);
			component.OnBackToOrigin += this.OnBoomerangBack;
		}

		private void OnBoomerangBack(BoomerangProjectile b)
		{
			if (this.OnBoomerangReturnEvent != null)
			{
				this.OnBoomerangReturnEvent();
			}
			b.OnBackToOrigin -= this.OnBoomerangBack;
		}

		public void SetProjectileWeaponDamage(int damage)
		{
			this.SetDamage((float)damage);
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
			this.SetDamage((float)damage);
			BoomerangBlade component = projectile.GetComponent<BoomerangBlade>();
			if (component != null)
			{
				component.SetHit(this._weaponHit);
			}
		}

		[FoldoutGroup("Boomerang Attack settings", 0)]
		public float damage;

		[FoldoutGroup("Boomerang Attack settings", 0)]
		public bool unavoidable;

		private Hit _weaponHit;

		public GameObject projectilePrefab;

		public Transform projectileSource;

		public int poolSize = 3;
	}
}
