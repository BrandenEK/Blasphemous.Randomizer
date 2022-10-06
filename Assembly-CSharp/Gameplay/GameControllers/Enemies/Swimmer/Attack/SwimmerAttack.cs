using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer.Attack
{
	public class SwimmerAttack : EnemyAttack
	{
		public Vector3 JumpPosition { get; set; }

		public bool IsTargetTouched { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.swimmer = (Swimmer)base.EntityOwner;
			this.swimmer.GetComponentInChildren<AttackArea>().OnEnter += this.OnEnter;
			this.swimmer.GetComponentInChildren<AttackArea>().OnExit += this.OnExit;
		}

		private void OnExit(object sender, Collider2DParam e)
		{
			this.IsTargetTouched = false;
		}

		private void OnEnter(object sender, Collider2DParam e)
		{
			this.IsTargetTouched = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.swimmer.DamageByContact = !this.swimmer.Status.IsGrounded;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		public void ThrowProjectile()
		{
			Vector2[] projectileTargets = this.GetProjectileTargets();
			Vector3 position = base.transform.position;
			Vector2 vector = Vector2.zero;
			for (int i = 0; i < projectileTargets.Length; i++)
			{
				PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.Projectile, position, Quaternion.identity, false, 1);
				if (objectInstance == null)
				{
					return;
				}
				SwimmerWeapon componentInChildren = objectInstance.GameObject.GetComponentInChildren<SwimmerWeapon>();
				if (componentInChildren != null)
				{
					componentInChildren.SetOwner(base.EntityOwner);
				}
				if (vector == Vector2.zero)
				{
					vector = this.GetProjectileMotion(position, projectileTargets[i]);
				}
				float num = (i % 2 != 0) ? (-vector.x) : vector.x;
				Vector2 vel;
				vel..ctor(num, vector.y);
				this.SetProjectileSpeed(objectInstance.GameObject, vel);
			}
		}

		private void SetProjectileSpeed(GameObject projectile, Vector2 vel)
		{
			Rigidbody2D component = projectile.GetComponent<Rigidbody2D>();
			component.velocity = vel;
		}

		private Vector3 GetProjectileMotion(Vector3 startPosition, Vector2 target)
		{
			float num = target.x - startPosition.x;
			float num2 = target.y - startPosition.y;
			float num3 = Mathf.Atan((num2 + this.FiringAngle) / num);
			float num4 = num / Mathf.Cos(num3);
			float num5 = num4 * Mathf.Cos(num3);
			float num6 = num4 * Mathf.Sin(num3);
			return new Vector2(num5, num6);
		}

		public Vector2[] GetProjectileTargets()
		{
			Vector2 vector = this.JumpPosition;
			Vector2 vector2;
			vector2..ctor(vector.x - this.ProjectileRange, vector.y);
			Vector2 vector3;
			vector3..ctor(vector.x + this.ProjectileRange, vector.y);
			return new Vector2[]
			{
				vector2,
				vector3
			};
		}

		private void OnDestroy()
		{
			this.swimmer.GetComponentInChildren<AttackArea>().OnEnter -= this.OnEnter;
			this.swimmer.GetComponentInChildren<AttackArea>().OnExit -= this.OnExit;
		}

		public float RndThrowHorOffset = 1f;

		public float ProjectileForce = 3f;

		public float FiringAngle;

		protected Swimmer swimmer;

		public float ProjectileRange = 4f;

		public GameObject Projectile;
	}
}
