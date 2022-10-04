using System;
using Framework.FrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class VisionCone : Trait
	{
		public bool CanSeeTarget(Transform t, string targetLayer, bool useColliderBounds = false)
		{
			if (t == null)
			{
				return false;
			}
			Vector2 vector = base.transform.position + this.sightOffset;
			Vector2 vector2 = t.position + Vector3.up * this.targetHeightOffset;
			if (useColliderBounds)
			{
				Enemy component = t.GetComponent<Enemy>();
				if (component.EntityDamageArea != null)
				{
					vector2 = component.EntityDamageArea.DamageAreaCollider.bounds.center;
				}
			}
			float num = Vector2.Distance(vector, t.position);
			if (num > this.sightDistance)
			{
				return false;
			}
			if (num < this.closeRadius)
			{
				return true;
			}
			Vector2 vector3 = vector2 - vector;
			int num2 = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			float num3 = Mathf.Atan2(Mathf.Abs(vector3.y), Mathf.Abs(vector3.x)) * 57.29578f;
			if (this.cantSeeBackwards && ((num2 == 1 && vector3.x < 0f) || (num2 == -1 && vector3.x > 0f)))
			{
				return false;
			}
			if (num3 > this.visionAngle)
			{
				Debug.DrawLine(vector, vector2, Color.black, 1f);
				return false;
			}
			RaycastHit2D[] array = new RaycastHit2D[1];
			bool flag = Physics2D.LinecastNonAlloc(vector, vector2, array, this.sightCollisionMask) > 0;
			if (flag)
			{
				if (array[0].collider.gameObject.layer == LayerMask.NameToLayer(targetLayer))
				{
					Debug.DrawLine(vector, array[0].point, Color.green, 1f);
					return true;
				}
				Debug.DrawLine(vector, array[0].point, Color.red, 1f);
			}
			else
			{
				Debug.DrawLine(vector, vector2, Color.red, 1f);
			}
			return false;
		}

		private void OnDrawGizmosSelected()
		{
			if (!this.gizmoOn)
			{
				return;
			}
			Gizmos.color = this.gizmoConeLimitsColor;
			int num = 1;
			if (base.EntityOwner != null)
			{
				num = ((base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1 : 1);
			}
			Vector2 vector = base.transform.position + this.sightOffset;
			Vector2 vector2 = Quaternion.Euler(0f, 0f, this.visionAngle) * Vector2.right * this.sightDistance * (float)num;
			Vector2 v = vector + vector2;
			Vector2 b = Quaternion.Euler(0f, 0f, -this.visionAngle) * Vector2.right * this.sightDistance * (float)num;
			Vector2 v2 = vector + b;
			Gizmos.DrawWireSphere(base.transform.position + this.sightOffset, this.closeRadius);
			Gizmos.DrawLine(vector, v);
			Gizmos.DrawLine(vector, v2);
			int num2 = this.gizmoNumberOfRays;
			Gizmos.color = this.gizmoRayColor;
			for (int i = 1; i < num2; i++)
			{
				Vector2 v3 = vector + Quaternion.Euler(0f, 0f, (float)i * -this.visionAngle * 2f / (float)num2) * vector2;
				Gizmos.DrawLine(vector, v3);
			}
		}

		[FoldoutGroup("Vision Settings", true, 0)]
		public LayerMask sightCollisionMask;

		[FoldoutGroup("Vision Settings", true, 0)]
		public Vector2 sightOffset;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float visionAngle;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float sightDistance;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float closeRadius;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float targetHeightOffset = 0.5f;

		[FoldoutGroup("Vision Settings", true, 0)]
		public bool cantSeeBackwards;

		[FoldoutGroup("Gizmo", true, 0)]
		public bool gizmoOn = true;

		[FoldoutGroup("Gizmo", true, 0)]
		[ShowIf("gizmoOn", true)]
		public Color gizmoConeLimitsColor = Color.magenta;

		[FoldoutGroup("Gizmo", true, 0)]
		[ShowIf("gizmoOn", true)]
		public Color gizmoRayColor = Color.cyan;

		[FoldoutGroup("Gizmo", true, 0)]
		[ShowIf("gizmoOn", true)]
		public int gizmoNumberOfRays = 30;
	}
}
