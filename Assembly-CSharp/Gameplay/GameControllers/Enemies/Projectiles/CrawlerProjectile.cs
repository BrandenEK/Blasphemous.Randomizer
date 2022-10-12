using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class CrawlerProjectile : StraightProjectile
	{
		public override void Init(Vector3 direction, float speed)
		{
			base.Init(direction, speed);
			if (direction.x < 0f)
			{
				this.SetOrientation(EntityOrientation.Left, false, false);
				this.particleTransform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
				this.particleTransform.localScale = new Vector3(1f, 1f, 1f);
			}
			this.motionChecker.SnapToGround(base.transform, 10f, 0.05f);
		}

		public override void Init(Vector3 origin, Vector3 target, float speed)
		{
			this.Init(target - origin, speed);
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.crawlUp = Quaternion.Euler(0f, 0f, 90f);
			this.crawlDown = Quaternion.Euler(0f, 0f, -90f);
		}

		protected override void OnUpdate()
		{
			this.CheckGround();
			this.UpdateMovement();
			this._currentTTL -= Time.deltaTime;
			if (this._currentTTL < 0f)
			{
				base.OnLifeEnded();
			}
			this.debugOrientation = this.Status.Orientation;
			if (this.Status.Orientation == EntityOrientation.Left)
			{
				this.velocity -= this.acceleration * Time.deltaTime;
			}
			else
			{
				this.velocity += this.acceleration * Time.deltaTime;
			}
			this.UpdateAnimSpeed();
		}

		private void UpdateAnimSpeed()
		{
			float speed = Mathf.Lerp(this.minAnimSpeed, this.maxAnimSpeed, (Mathf.Abs(this.velocity.x) - this.minSpeed) / (this.maxSpeed - this.minSpeed));
			this.anim.speed = speed;
		}

		private void CheckGround()
		{
			Quaternion lhs = (this.Status.Orientation != EntityOrientation.Right) ? this.crawlDown : this.crawlUp;
			Quaternion lhs2 = (this.Status.Orientation != EntityOrientation.Right) ? this.crawlUp : this.crawlDown;
			if (this.motionChecker.HitsBlock)
			{
				base.transform.rotation = lhs * base.transform.rotation;
				this.motionChecker.SnapToGround(base.transform, 2f, 0.05f);
			}
			else if (!this.motionChecker.HitsFloor)
			{
				base.transform.rotation = lhs2 * base.transform.rotation;
				this.motionChecker.SnapToGround(base.transform, 2f, 0.05f);
			}
		}

		private void UpdateMovement()
		{
			base.transform.Translate(this.velocity * Time.deltaTime, Space.Self);
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
			}
		}

		public EntityMotionChecker motionChecker;

		public EntityOrientation debugOrientation;

		private Quaternion crawlUp;

		private Quaternion crawlDown;

		public Vector2 acceleration;

		public Animator anim;

		public float minAnimSpeed = 0.6f;

		public float maxAnimSpeed = 1f;

		private float maxSpeed = 16f;

		private float minSpeed = 4f;

		public Transform particleTransform;
	}
}
