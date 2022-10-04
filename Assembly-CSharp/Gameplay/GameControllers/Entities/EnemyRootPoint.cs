using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class EnemyRootPoint : MonoBehaviour
	{
		public Enemy Enemy { get; private set; }

		public Vector2 LocalStartPosition { get; set; }

		private void Start()
		{
			this.Enemy = base.GetComponentInParent<Enemy>();
			this.LocalStartPosition = new Vector2(base.transform.localPosition.x, base.transform.localPosition.y);
			if (this.Enemy != null)
			{
				return;
			}
			Debug.LogError("An Enemy component is needed!");
			base.enabled = false;
		}

		private void Update()
		{
			this.SetPosition();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}

		private void SetPosition()
		{
			EntityOrientation orientation = this.Enemy.Status.Orientation;
			Vector3 localPosition;
			if (orientation != EntityOrientation.Left)
			{
				if (orientation != EntityOrientation.Right)
				{
					throw new ArgumentOutOfRangeException();
				}
				localPosition = this.LocalStartPosition;
			}
			else
			{
				localPosition = Vector3.Reflect(this.LocalStartPosition, Vector2.right);
			}
			base.transform.localPosition = localPosition;
		}
	}
}
