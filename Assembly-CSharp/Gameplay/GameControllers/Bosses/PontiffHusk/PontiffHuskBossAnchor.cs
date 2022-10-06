using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	public class PontiffHuskBossAnchor : MonoBehaviour
	{
		private void Start()
		{
			this.FixedX = base.gameObject.transform.position.x;
			this.FixedY = base.gameObject.transform.position.y;
		}

		private void LateUpdate()
		{
			Vector2 vector;
			vector..ctor(base.gameObject.transform.position.x, base.gameObject.transform.position.y);
			if (!this.FollowsParentsX)
			{
				vector.x = this.FixedX;
			}
			else
			{
				vector.x = this.ReferenceTransform.position.x;
			}
			if (!this.FollowsParentsY)
			{
				vector.y = this.FixedY;
			}
			else
			{
				vector.y = this.ReferenceTransform.position.y;
			}
			base.gameObject.transform.position = vector;
		}

		public Transform ReferenceTransform;

		public bool FollowsParentsX = true;

		public bool FollowsParentsY;

		[HideInInspector]
		public float FixedX;

		[HideInInspector]
		public float FixedY;
	}
}
