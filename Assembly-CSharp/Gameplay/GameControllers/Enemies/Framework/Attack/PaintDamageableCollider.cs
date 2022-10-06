using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	[RequireComponent(typeof(PolygonCollider2D))]
	public class PaintDamageableCollider : MonoBehaviour
	{
		private void Awake()
		{
			this.polygonCollider2D = base.GetComponent<PolygonCollider2D>();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = ((!this.polygonCollider2D.enabled) ? Color.gray : Color.cyan);
			Vector2[] points = this.polygonCollider2D.points;
			Vector3 vector = base.transform.TransformPoint(points[points.Length - 1] + this.polygonCollider2D.offset);
			Vector3 vector2 = base.transform.TransformPoint(points[0] + this.polygonCollider2D.offset);
			Gizmos.DrawLine(vector, vector2);
			for (int i = 0; i < points.Length - 1; i++)
			{
				vector = base.transform.TransformPoint(points[i] + this.polygonCollider2D.offset);
				vector2 = base.transform.TransformPoint(points[i + 1] + this.polygonCollider2D.offset);
				Gizmos.DrawLine(vector, vector2);
			}
		}

		private PolygonCollider2D polygonCollider2D;
	}
}
