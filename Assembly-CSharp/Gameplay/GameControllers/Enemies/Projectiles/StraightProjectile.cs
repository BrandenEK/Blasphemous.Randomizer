using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class StraightProjectile : Projectile
	{
		public int OriginalDamage
		{
			get
			{
				return this.originalDamage;
			}
			set
			{
				this.originalDamage = value;
			}
		}

		public virtual void Init(Vector3 origin, Vector3 target, float speed)
		{
			this.Init((target - origin).normalized, speed);
			ResetTrailRendererOnEnable componentInChildren = base.GetComponentInChildren<ResetTrailRendererOnEnable>();
			if (componentInChildren)
			{
				componentInChildren.Clean();
			}
		}

		public virtual void Init(Vector3 direction, float speed)
		{
			this.velocity = direction.normalized * speed;
			if (this.faceVelocityDirection)
			{
				Vector2 normalized = this.velocity.normalized;
				float num = 57.29578f * Mathf.Atan2(normalized.y, normalized.x);
				base.transform.eulerAngles = new Vector3(0f, 0f, num);
			}
		}

		public bool faceVelocityDirection;

		protected int originalDamage;
	}
}
