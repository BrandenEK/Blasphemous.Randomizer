using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.InitPool();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			foreach (HomingProjectile homingProjectile in this.homingProjectiles)
			{
				if (this.ChargesIsidora)
				{
					if (!this.isidoraBehaviour)
					{
						break;
					}
					bool flag = homingProjectile.ChangeTargetToAlternative(this.isidoraBehaviour.transform, 2f, 1f, 3f);
					if (flag)
					{
						homingProjectile.OnLifeEndedEvent -= this.HomingProjectile_OnLifeEndedEvent;
						homingProjectile.OnLifeEndedEvent += this.HomingProjectile_OnLifeEndedEvent;
					}
				}
				else
				{
					homingProjectile.ChangeTargetToPenitent(true);
				}
			}
		}

		public bool IsAnyProjectileActive()
		{
			foreach (HomingProjectile homingProjectile in this.homingProjectiles)
			{
				if (homingProjectile.gameObject.activeInHierarchy)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearAll()
		{
			foreach (HomingProjectile homingProjectile in this.homingProjectiles)
			{
				homingProjectile.SetTTL(0f);
			}
		}

		private void InitPool()
		{
			if (!this.ProjectileWeapon)
			{
				return;
			}
			PoolManager.Instance.CreatePool(this.ProjectileWeapon.gameObject, this.PoolSize);
		}

		private void SetupProjectileWeapon(ProjectileWeapon projectileWeapon)
		{
			if (!projectileWeapon)
			{
				return;
			}
			projectileWeapon.WeaponOwner = base.EntityOwner;
			projectileWeapon.SetOwner(base.EntityOwner.gameObject);
			projectileWeapon.SetDamage((int)base.EntityOwner.Stats.Strength.Final);
		}

		private void SetupHomingProjectile(HomingProjectile homingProjectile, Vector2 currentDirection, float targetOffsetFactor, Vector2 targetOffset)
		{
			homingProjectile.currentDirection = currentDirection;
			homingProjectile.TargetOffsetFactor = targetOffsetFactor;
			homingProjectile.TargetOffset = targetOffset;
		}

		public void FireProjectile()
		{
			if (!this.ProjectileWeapon)
			{
				return;
			}
			for (int i = 1; i <= this.NumProjectiles; i++)
			{
				Vector2 vector = (!this.UseCastingPosition) ? base.EntityOwner.transform.position : this.CastingPosition;
				vector += this.OffsetPosition;
				int num = 0;
				int num2 = this.NumProjectiles;
				if (this.NumProjectiles % 2 == 0 || i < this.NumProjectiles)
				{
					num = ((i % 2 != 0) ? (-i) : (i - 1));
					num2 = ((i % 2 != 0) ? (this.NumProjectiles - i) : (this.NumProjectiles - i + 1));
				}
				vector.x += (float)num * this.HorizontalSpacingFactor;
				vector.y += (float)num2 * this.VerticalSpacingFactor;
				PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.ProjectileWeapon.gameObject, vector, Quaternion.identity, false, 1);
				HomingProjectile component = objectInstance.GameObject.GetComponent<HomingProjectile>();
				float num3 = 5f;
				Vector2 targetOffset = component.TargetOffset;
				if (this.ChargesIsidora && this.prevOffestWasPositive)
				{
					num3 *= -1f;
					num2 *= -1;
				}
				Vector2 vector2 = new Vector2((float)num, (float)num2);
				Vector2 normalized = vector2.normalized;
				this.SetupHomingProjectile(component, normalized, num3, targetOffset);
				if (!this.homingProjectiles.Contains(component))
				{
					this.homingProjectiles.Add(component);
				}
				if (this.ChargesIsidora)
				{
					component.OnLifeEndedEvent += this.HomingProjectile_OnLifeEndedEvent;
				}
				ProjectileWeapon component2 = objectInstance.GameObject.GetComponent<ProjectileWeapon>();
				this.SetupProjectileWeapon(component2);
			}
			this.prevOffestWasPositive = !this.prevOffestWasPositive;
		}

		private void HomingProjectile_OnLifeEndedEvent(Projectile obj)
		{
			HomingProjectile homingProjectile = obj as HomingProjectile;
			homingProjectile.OnLifeEndedEvent -= this.HomingProjectile_OnLifeEndedEvent;
			Vector3 position = homingProjectile.gameObject.transform.position;
			Vector2 b = homingProjectile.CalculateTargetPosition();
			if (Vector2.Distance(position, b) < 1.3f)
			{
				this.isidoraBehaviour.ProjectileAbsortion(homingProjectile.transform.position, homingProjectile.currentDirection);
			}
		}

		[FoldoutGroup("Projectile Settings", 0)]
		public ProjectileWeapon ProjectileWeapon;

		[FoldoutGroup("Projectile Settings", 0)]
		public int NumProjectiles = 1;

		[FoldoutGroup("Projectile Settings", 0)]
		public Vector2 OffsetPosition;

		[FoldoutGroup("Projectile Settings", 0)]
		public bool UseCastingPosition;

		[FoldoutGroup("Projectile Settings", 0)]
		public Vector2 CastingPosition;

		[FoldoutGroup("Projectile Settings", 0)]
		public float HorizontalSpacingFactor = 1f;

		[FoldoutGroup("Projectile Settings", 0)]
		public float VerticalSpacingFactor = 1f;

		[FoldoutGroup("Other", 0)]
		public int PoolSize = 20;

		[FoldoutGroup("Other", 0)]
		public bool ChargesIsidora;

		[FoldoutGroup("Other", 0)]
		public IsidoraBehaviour isidoraBehaviour;

		private bool prevOffestWasPositive = true;

		private List<HomingProjectile> homingProjectiles = new List<HomingProjectile>();
	}
}
