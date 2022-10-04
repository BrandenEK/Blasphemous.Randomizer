using System;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Gizmos
{
	public class RootMotionDriver : MonoBehaviour
	{
		public Vector3 ReversePosition
		{
			get
			{
				Vector3 position = base.transform.position;
				Vector3 localPosition = base.transform.localPosition;
				float x = position.x - localPosition.x * 2f;
				return new Vector3(x, position.y, 0f);
			}
		}

		public Vector3 FlipedPosition
		{
			get
			{
				Vector3 localPosition = base.transform.localPosition;
				Vector2 v = new Vector2(-localPosition.x, localPosition.y);
				Vector3 vector = base.transform.TransformPoint(v);
				return new Vector2(vector.x, vector.y - localPosition.y);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(this.FlipedPosition, 0.1f);
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(this.ReversePosition, 0.1f);
		}
	}
}
