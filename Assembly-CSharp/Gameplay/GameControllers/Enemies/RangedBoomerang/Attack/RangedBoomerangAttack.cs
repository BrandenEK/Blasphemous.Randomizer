using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Enemies.RangedBoomerang.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang.Attack
{
	public class RangedBoomerangAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.projectilePrefab, 1);
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
			BoomerangProjectile component = PoolManager.Instance.ReuseObject(this.projectilePrefab, this.pool.transform.position, Quaternion.identity, false, 1).GameObject.GetComponent<BoomerangProjectile>();
			component.Init(component.transform.position, this.target.position + Vector3.up * 1.25f, 12f);
			this._weaponHit.AttackingEntity = component.gameObject;
			component.GetComponent<BoomerangBlade>().SetHit(this._weaponHit);
			component.OnBackToOrigin += this.OnBoomerangBack;
		}

		private void OnBoomerangBack(BoomerangProjectile b)
		{
			b.GetComponent<BoomerangBlade>().Recycle();
			this.ownerBehaviour.OnBoomerangRecovered();
			b.OnBackToOrigin -= this.OnBoomerangBack;
		}

		private Hit _weaponHit;

		public ProjectilePool pool;

		public Transform target;

		public GameObject projectilePrefab;

		public RangedBoomerangBehaviour ownerBehaviour;
	}
}
