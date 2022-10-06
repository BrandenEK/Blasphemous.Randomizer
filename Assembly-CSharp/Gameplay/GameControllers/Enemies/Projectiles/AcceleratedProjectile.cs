using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class AcceleratedProjectile : StraightProjectile
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._attackArea = base.GetComponentInChildren<AttackArea>();
			this._originalLayerMask = this._attackArea.enemyLayerMask;
			this.originalDamage = base.GetComponent<ProjectileWeapon>().damage;
			if (this.changeExplosionMaskAfterDeflection)
			{
				GameObject explosion = base.GetComponent<ProjectileWeapon>().explosion;
				AttackArea componentInChildren = explosion.GetComponentInChildren<AttackArea>();
				this._defaultExplosionMask = componentInChildren.enemyLayerMask;
			}
			if (this.changeProjectileMaskAfterDeflection)
			{
				ProjectileWeapon component = base.GetComponent<ProjectileWeapon>();
				AttackArea componentInChildren2 = component.GetComponentInChildren<AttackArea>();
				this._defaultProjectileMask = componentInChildren2.enemyLayerMask;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.deflectWithSword)
			{
				this.projectileReaction.OnProjectileHit += this.AcceleratedProjectile_OnProjectileHit;
			}
		}

		public override void Init(Vector3 direction, float speed)
		{
			base.Init(direction, speed);
			base.GetComponent<ProjectileWeapon>().SetDamage(this.originalDamage);
			this._attackArea.SetLayerMask(this._originalLayerMask);
			this._accel = this.acceleration;
			if (this.directionInvertsAcceleration && direction.x < 0f)
			{
				this._accel.x = this._accel.x * -1f;
			}
			if (this.changeExplosionMaskAfterDeflection)
			{
				GameObject explosion = base.GetComponent<ProjectileWeapon>().explosion;
				AttackArea componentInChildren = explosion.GetComponentInChildren<AttackArea>();
				componentInChildren.SetLayerMask(this._defaultExplosionMask);
			}
			if (this.changeProjectileMaskAfterDeflection)
			{
				ProjectileWeapon component = base.GetComponent<ProjectileWeapon>();
				AttackArea componentInChildren2 = component.GetComponentInChildren<AttackArea>();
				componentInChildren2.SetLayerMask(this._defaultProjectileMask);
			}
		}

		public override void Init(Vector3 origin, Vector3 target, float speed)
		{
			this.Init((target - origin).normalized, speed);
		}

		public void SetBouncebackData(Transform t, Vector2 offset, int dmg = 4)
		{
			this._targetToBounceBack = t;
			this._targetToBounceBackOffset = offset;
			this._bounceBackDamage = dmg;
		}

		private void AcceleratedProjectile_OnProjectileHit(ProjectileReaction obj)
		{
			this.acceleration = Vector2.zero;
			Vector3 vector = Core.Logic.Penitent.transform.position + Vector3.up;
			Vector2 vector2 = (base.transform.position - vector).normalized;
			base.GetComponent<ProjectileWeapon>().SetDamage(this._bounceBackDamage);
			if (this.bounceBackToTarget && this._targetToBounceBack != null)
			{
				Vector2 vector3 = (this._targetToBounceBack.position + this._targetToBounceBackOffset - base.transform.position).normalized;
				this._attackArea.SetLayerMask(this.layerMaskOnBounceBack);
				if (Vector2.Angle(vector3, vector2) < this.validBounceBackAngle)
				{
					this._accel = Vector2.zero;
					vector2 = vector3;
					Debug.DrawRay(base.transform.position, vector2 * 10f, Color.green, 10f);
				}
			}
			this.velocity = vector2 * (this.velocity.magnitude * this.deflectedVelocityMultiplier);
			if (this.faceVelocityDirection)
			{
				Vector2 normalized = this.velocity.normalized;
				float num = 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
				base.transform.eulerAngles = new Vector3(0f, 0f, num);
			}
			if (this.changeExplosionMaskAfterDeflection)
			{
				GameObject explosion = base.GetComponent<ProjectileWeapon>().explosion;
				AttackArea componentInChildren = explosion.GetComponentInChildren<AttackArea>();
				componentInChildren.SetLayerMask(this.altExplosionMask);
			}
			if (this.changeProjectileMaskAfterDeflection)
			{
				ProjectileWeapon component = base.GetComponent<ProjectileWeapon>();
				AttackArea componentInChildren2 = component.GetComponentInChildren<AttackArea>();
				componentInChildren2.SetLayerMask(this.altProjectileMask);
			}
		}

		public void SetAcceleration(Vector2 a)
		{
			this._accel = a;
			this.acceleration = a;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.velocity += this._accel * Time.deltaTime;
			this.velocity += this.velocity * ((this.velocityMultiplier - 1f) * Time.deltaTime);
			if (this.clampsMaxSpeed)
			{
				this.velocity = Vector2.ClampMagnitude(this.velocity, this.maxSpeed);
			}
		}

		private void OnDisable()
		{
			if (this.resetVelocityAndAccelerationWhenDisabled)
			{
				this.velocity = Vector2.zero;
				this.acceleration = Vector2.zero;
			}
		}

		public Vector2 acceleration;

		public float velocityMultiplier = 1f;

		private Vector2 _accel;

		public bool directionInvertsAcceleration = true;

		public float deflectedVelocityMultiplier = 1.4f;

		public ProjectileReaction projectileReaction;

		public bool resetVelocityAndAccelerationWhenDisabled = true;

		[FoldoutGroup("Max speed settings", 0)]
		public bool clampsMaxSpeed;

		[FoldoutGroup("Max speed settings", 0)]
		[ShowIf("clampsMaxSpeed", true)]
		public float maxSpeed;

		[FoldoutGroup("Deflection settings", 0)]
		public bool deflectWithSword;

		[ShowIf("deflectWithSword", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public bool bounceBackToTarget;

		[ShowIf("deflectWithSword", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public LayerMask layerMaskOnBounceBack;

		[ShowIf("deflectWithSword", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public float validBounceBackAngle = 30f;

		[ShowIf("deflectWithSword", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public bool changeProjectileMaskAfterDeflection;

		[ShowIf("deflectWithSword", true)]
		[ShowIf("changeProjectileMaskAfterDeflection", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public LayerMask altProjectileMask;

		[ShowIf("deflectWithSword", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public bool changeExplosionMaskAfterDeflection;

		[ShowIf("deflectWithSword", true)]
		[ShowIf("changeExplosionMaskAfterDeflection", true)]
		[FoldoutGroup("Deflection settings", 0)]
		public LayerMask altExplosionMask;

		private Vector2 _targetToBounceBackOffset;

		private Transform _targetToBounceBack;

		private LayerMask _originalLayerMask;

		private AttackArea _attackArea;

		private int _bounceBackDamage;

		private LayerMask _defaultExplosionMask;

		private LayerMask _defaultProjectileMask;
	}
}
