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
			Vector2 v = new Vector2(base.gameObject.transform.position.x, base.gameObject.transform.position.y);
			if (!this.FollowsParentsX)
			{
				v.x = this.FixedX;
			}
			else
			{
				v.x = this.ReferenceTransform.position.x;
			}
			if (!this.FollowsParentsY)
			{
				v.y = this.FixedY;
			}
			else
			{
				v.y = this.ReferenceTransform.position.y;
			}
			base.gameObject.transform.position = v;
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
