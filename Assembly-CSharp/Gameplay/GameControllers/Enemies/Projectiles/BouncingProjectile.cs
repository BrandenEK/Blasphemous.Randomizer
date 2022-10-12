using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Entities;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class BouncingProjectile : StraightProjectile
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.projectileWeapon = base.GetComponent<ProjectileWeapon>();
			PoolManager.Instance.CreatePool(this.splashOnBounce, 5);
			this.instantiationCounter = -1f;
			this.currentYImpulse = this.yImpulse;
			this.currentHits = this.hits;
		}

		protected override void OnUpdate()
		{
			base.transform.Translate(this.velocity * Time.deltaTime, Space.World);
			this.UpdateGravity();
			this.CheckRebound();
			base.UpdateOrientation();
			this.UpdateRotation();
			this.UpdateCounter();
		}

		private void UpdateCounter()
		{
			if (this.instantiationCounter > 0f)
			{
				this.instantiationCounter -= Time.deltaTime;
			}
		}

		private void UpdateRotation()
		{
			if (this.faceVelocityDirection)
			{
				Vector2 normalized = this.velocity.normalized;
				float num = 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
				if (normalized.x < 0f)
				{
					num += 180f;
				}
				this.spriteRenderer.transform.eulerAngles = new Vector3(0f, 0f, num);
				this.motionChecker.transform.eulerAngles = new Vector3(num, 0f, 0f);
			}
		}

		public override void Init(Vector3 direction, float speed)
		{
			base.Init(direction, speed);
		}

		private void UpdateGravity()
		{
			this.velocity.y = this.velocity.y - this.gravity * Time.deltaTime;
		}

		public bool CanInstanceOnBounce()
		{
			return this.instantiationCounter <= 0f;
		}

		private void CheckRebound()
		{
			if (this.motionChecker.HitsFloor)
			{
				this.velocity.y = this.currentYImpulse;
				if (this.CanInstanceOnBounce())
				{
					PoolManager.Instance.ReuseObject(this.splashOnBounce, base.transform.position, Quaternion.identity, false, 1);
					this.instantiationCounter = this.instantiationCooldown;
				}
				if (this.loseImpulseAfterBounce)
				{
					this.currentYImpulse *= 0.95f;
					if (this.currentYImpulse <= 2f)
					{
						this.Deactivate();
					}
				}
				if (!this.BounceFxSound.IsNullOrWhitespace())
				{
					Core.Audio.PlaySfx(this.BounceFxSound, 0f);
				}
			}
			else if (this.motionChecker.HitsBlock)
			{
				this.velocity.x = this.velocity.x * -1f;
				this.currentHits--;
				if (!this.BounceFxSound.IsNullOrWhitespace())
				{
					Core.Audio.PlaySfx(this.BounceFxSound, 0f);
				}
				if (this.currentHits == 0)
				{
					this.Deactivate();
				}
			}
		}

		private void Deactivate()
		{
			if (this.projectileWeapon != null)
			{
				this.currentYImpulse = this.yImpulse;
				this.currentHits = this.hits;
				this.projectileWeapon.ForceDestroy();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public float yImpulse = 10f;

		public float gravity = 9.8f;

		public EntityMotionChecker motionChecker;

		public int hits = 5;

		[EventRef]
		public string BounceFxSound;

		public GameObject splashOnBounce;

		public ProjectileWeapon projectileWeapon;

		public bool loseImpulseAfterBounce;

		public float instantiationCooldown = 0.3f;

		private float instantiationCounter;

		private float currentYImpulse;

		private int currentHits;
	}
}
