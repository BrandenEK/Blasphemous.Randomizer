using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.Attack
{
	public class RollerProjectile : Weapon
	{
		public Animator ProjectileAnimator { get; private set; }

		public StraightProjectile Motion { get; private set; }

		public AttackArea AttackArea { get; private set; }

		public SpriteRenderer SpriteRenderer { get; private set; }

		public CollisionSensor CollisionSensor { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Motion = base.GetComponent<StraightProjectile>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this.ProjectileAnimator = base.GetComponent<Animator>();
			this._defaultMotionVelocity = new Vector2(this.Motion.velocity.x, this.Motion.velocity.y);
			this.CollisionSensor = base.GetComponentInChildren<CollisionSensor>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
			this._projectileHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = this.DamageAmount,
				HitSoundId = this.HitSoundId
			};
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.SpriteRenderer.isVisible)
			{
				base.Destroy();
			}
			if (this.CollisionSensor.IsColliding() && !this._wallImpact)
			{
				this._wallImpact = true;
				this.StopByImpact();
			}
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			this.Attack(this._projectileHit);
			this.StopByImpact();
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		private void StopByImpact()
		{
			this.Motion.velocity = Vector2.zero;
			this.ProjectileAnimator.SetTrigger("IMPACT");
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this._wallImpact = false;
			this.Motion.velocity.x = Mathf.Abs(this._defaultMotionVelocity.x);
		}

		public void SetOwner(Entity owner)
		{
			this.Motion.owner = owner;
			this.AttackArea.Entity = owner;
			this.WeaponOwner = owner;
			this.Motion.velocity = this._defaultMotionVelocity;
			if (owner.Status.Orientation == EntityOrientation.Left)
			{
				StraightProjectile motion = this.Motion;
				motion.velocity.x = motion.velocity.x * -1f;
			}
			this.SpriteRenderer.flipX = (owner.Status.Orientation == EntityOrientation.Left);
		}

		public void Dispose()
		{
			base.Destroy();
		}

		private void OnDestroy()
		{
			this.AttackArea.OnEnter -= this.OnEnterAttackArea;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		public void Damage(Hit hit)
		{
			Core.Audio.PlaySfx(this.DestroyedByHitFx, 0f);
			this.StopByImpact();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private Hit _projectileHit;

		public float DamageAmount = 10f;

		private Vector2 _defaultMotionVelocity;

		private bool _wallImpact;

		[EventRef]
		public string HitSoundId;

		[EventRef]
		public string DestroyedByHitFx;
	}
}
