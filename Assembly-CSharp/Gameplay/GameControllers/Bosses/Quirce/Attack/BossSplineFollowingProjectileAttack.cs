using System;
using System.Diagnostics;
using BezierSplines;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossSplineFollowingProjectileAttack : EnemyAttack, IProjectileAttack
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BossSplineFollowingProjectileAttack, float, float> OnPathAdvanced;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BossSplineFollowingProjectileAttack> OnPathFinished;

		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.projectilePrefab, this.poolSize);
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

		public void SetProjectileWeaponDamage(int damage)
		{
			if (damage > 0)
			{
				this.PathFollowingProjectileDamage = (float)damage;
			}
		}

		public void SetProjectileWeaponDamage(Projectile projectile, int damage)
		{
			this.SetProjectileWeaponDamage(damage);
			if (projectile == null)
			{
				return;
			}
			PathFollowingProjectile component = projectile.GetComponent<PathFollowingProjectile>();
			if (component)
			{
				component.SetDamage((float)damage);
			}
		}

		public void Shoot(BezierSpline spline, AnimationCurve curve, float totalSeconds = 4f)
		{
			base.CurrentWeaponAttack();
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.projectilePrefab, this.projectileSource.position, Quaternion.identity, false, 1).GameObject;
			SplineFollowingProjectile component = gameObject.GetComponent<SplineFollowingProjectile>();
			component.Init(component.transform.position, spline, totalSeconds, curve);
			this._weaponHit.AttackingEntity = component.gameObject;
			PathFollowingProjectile component2 = component.GetComponent<PathFollowingProjectile>();
			if (component2 != null)
			{
				PathFollowingProjectile component3 = component.GetComponent<PathFollowingProjectile>();
				if (component3)
				{
					component3.SetHit(this._weaponHit);
				}
			}
			this.SetProjectileWeaponDamage(component, (int)this.PathFollowingProjectileDamage);
			component.OnSplineCompletedEvent += this.OnSplineCompleted;
			component.OnSplineAdvancedEvent += this.OnSplineAdvanced;
			this._currentProjectile = component;
		}

		public void Shoot(BezierSpline spline, AnimationCurve curve, float totalSeconds, Vector3 origin)
		{
			spline.SetControlPoint(0, spline.transform.InverseTransformPoint(origin));
			spline.SetControlPoint(spline.ControlPointCount - 1, spline.transform.InverseTransformPoint(origin));
			this.Shoot(spline, curve, totalSeconds);
		}

		public void Shoot(BezierSpline spline, AnimationCurve curve, float totalSeconds, Vector3 origin, Vector3 end)
		{
			spline.SetControlPoint(0, spline.transform.InverseTransformPoint(origin));
			spline.SetControlPoint(spline.ControlPointCount - 1, spline.transform.InverseTransformPoint(end));
			this.Shoot(spline, curve, totalSeconds);
		}

		public SplineFollowingProjectile GetCurrentProjectile()
		{
			return this._currentProjectile;
		}

		private void OnSplineAdvanced(SplineFollowingProjectile p, float maxS, float elapS)
		{
			if (this.OnPathAdvanced != null)
			{
				this.OnPathAdvanced(this, maxS, elapS);
			}
		}

		private void OnSplineCompleted(SplineFollowingProjectile obj)
		{
			obj.OnSplineCompletedEvent -= this.OnSplineCompleted;
			this.lastPosition = obj.transform.position;
			UnityEngine.Debug.Log("ATTACK: SPLINE COMPLETED!!");
			if (this.OnPathFinished != null)
			{
				this.OnPathFinished(this);
			}
		}

		private Hit _weaponHit;

		public GameObject projectilePrefab;

		public Transform projectileSource;

		public int poolSize = 3;

		public Vector2 lastPosition;

		private SplineFollowingProjectile _currentProjectile;

		public float PathFollowingProjectileDamage;
	}
}
