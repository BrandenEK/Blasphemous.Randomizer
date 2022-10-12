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
			Vector3 from = base.transform.TransformPoint(points[points.Length - 1] + this.polygonCollider2D.offset);
			Vector3 to = base.transform.TransformPoint(points[0] + this.polygonCollider2D.offset);
			Gizmos.DrawLine(from, to);
			for (int i = 0; i < points.Length - 1; i++)
			{
				from = base.transform.TransformPoint(points[i] + this.polygonCollider2D.offset);
				to = base.transform.TransformPoint(points[i + 1] + this.polygonCollider2D.offset);
				Gizmos.DrawLine(from, to);
			}
		}

		private PolygonCollider2D polygonCollider2D;
	}
}
