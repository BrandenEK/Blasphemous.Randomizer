using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.Attack
{
	public class BellGhostVariantAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.projectilePrefab, 2);
			this.rootMotion = this.bulletSpawnPoint.GetComponent<RootMotionDriver>();
		}

		public void ShootProjectileAtPlayer()
		{
			Debug.Log("SHOOTING AT PLAYER");
			bool flipX = base.EntityOwner.SpriteRenderer.flipX;
			Vector3 vector = (!flipX) ? this.rootMotion.transform.position : this.rootMotion.ReversePosition;
			StraightProjectile component = PoolManager.Instance.ReuseObject(this.projectilePrefab.gameObject, vector, Quaternion.identity, false, 1).GameObject.GetComponent<StraightProjectile>();
			(component as TargetedProjectile).Init(vector, this.target.position + Vector3.up * 1.5f, this.projectileSpeed);
			ProjectileWeapon componentInChildren = component.GetComponentInChildren<ProjectileWeapon>();
			componentInChildren.SetOwner(this.owner.gameObject);
			componentInChildren.SetDamage((int)this.owner.Stats.Strength.Final);
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			this.ShootProjectileAtPlayer();
		}

		public ProjectilePool pPool;

		public SpawnPoint bulletSpawnPoint;

		public Entity owner;

		public Transform target;

		public GameObject projectilePrefab;

		public float projectileSpeed;

		public RootMotionDriver rootMotion;

		private const int MAX_PROJECTILES_POOLED = 2;
	}
}
