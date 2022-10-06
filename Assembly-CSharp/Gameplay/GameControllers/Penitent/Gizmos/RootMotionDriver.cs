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
				float num = position.x - localPosition.x * 2f;
				return new Vector3(num, position.y, 0f);
			}
		}

		public Vector3 FlipedPosition
		{
			get
			{
				Vector3 localPosition = base.transform.localPosition;
				Vector2 vector;
				vector..ctor(-localPosition.x, localPosition.y);
				Vector3 vector2 = base.transform.TransformPoint(vector);
				return new Vector2(vector2.x, vector2.y - localPosition.y);
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
