using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.MeltedLady.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Attack
{
	public class MeltedLadyAttack : EnemyAttack
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
		}

		private Hit _weaponHit;

		public MeltedLadyBehaviour ownerBehaviour;

		public ProjectilePool pool;

		public GameObject projectilePrefab;

		public Transform target;
	}
}
