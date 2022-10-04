using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Ghost
{
	public class GhostPath : MonoBehaviour
	{
		public int NextWayPointVisitedId
		{
			get
			{
				return this.nextWaypointVisitedId;
			}
			set
			{
				this.nextWaypointVisitedId = value;
			}
		}

		private void Start()
		{
		}

		public Vector3 GetWaypointPosition(int id)
		{
			Vector3 result = Vector3.zero;
			if (this.waypoints != null && this.waypoints.Length > 0)
			{
				int num = Mathf.Clamp(id, 0, this.waypoints.Length);
				result = base.transform.TransformPoint(this.waypoints[num].transform.localPosition);
			}
			return result;
		}

		private void OnDrawGizmos()
		{
			this.setWaypointsId();
		}

		protected void setWaypointsId()
		{
			if (this.waypoints.Length > 0)
			{
				for (int i = 0; i < this.waypoints.Length; i++)
				{
					this.waypoints[i].Id = i;
				}
			}
		}

		public bool enableDebugLines;

		protected int nextWaypointVisitedId;

		public GhostWaypoint[] waypoints;
	}
}
