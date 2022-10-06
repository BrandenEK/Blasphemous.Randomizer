using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Ghost
{
	public class GhostWaypoint : MonoBehaviour
	{
		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		private void Start()
		{
			this.index = 0f;
			this.ghostPath = base.GetComponentInParent<GhostPath>();
			this.speedY = Random.Range(0f, this.maxSpeedY);
		}

		private void Update()
		{
			this.index += Time.deltaTime;
			float num = Mathf.Sin(this.speedY * this.index) * this.amplitudeY;
			Vector2 vector;
			vector..ctor(base.transform.localPosition.x, num);
			base.transform.localPosition = vector;
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(base.transform.position, "Blasphemous/ghost.png", true);
			if (this.ghostPath != null && this.ghostPath.enableDebugLines && this.id != this.ghostPath.waypoints.Length - 1)
			{
				Gizmos.color = Color.cyan;
				GhostWaypoint ghostWaypoint = this.ghostPath.waypoints[this.id + 1];
				Gizmos.DrawLine(base.transform.position, ghostWaypoint.transform.position);
			}
		}

		public float amplitudeY = 3f;

		public GhostPath ghostPath;

		protected int id;

		protected float index;

		public float maxSpeedY = 4f;

		protected float speedY;

		protected Transform target;
	}
}
