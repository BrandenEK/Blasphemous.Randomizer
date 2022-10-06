using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Ghost
{
	public class GhostFlight : MonoBehaviour
	{
		public GhostFlight()
		{
			this.TrajectoryHeight = 4f;
			this.EndPos = Vector3.zero;
			this.StartPos = Vector3.zero;
		}

		private void Start()
		{
			this.defaultAmplitudeX = this.amplitudeX;
			this.defaultAmplitudeY = this.amplitudeY;
			this.deltaAttackTime = 0f;
			this.ghost = base.GetComponentInParent<Ghost>();
			if (this.ghost != null)
			{
				int currentWayPointId = Random.Range(0, this.ghost.GhostPath.waypoints.Length);
				this.ghost.CurrentWayPointId = currentWayPointId;
				Vector3 randomWaypointPosition = this.GetRandomWaypointPosition();
				this.ghost.transform.position = randomWaypointPosition;
			}
		}

		public Vector3 StartPos { get; set; }

		public Vector3 EndPos { get; set; }

		public float TrajectoryHeight { get; set; }

		public bool GetLanding { get; set; }

		public void Floating()
		{
			this.index += Time.deltaTime;
			float num = this.amplitudeX * Mathf.Cos(this.speedX * this.index);
			float num2 = Mathf.Sin(this.speedY * this.index) * this.amplitudeY;
			base.transform.localPosition = new Vector3(num, num2, 0f);
		}

		public void EnableFloating(bool enable = true)
		{
			if (this.OnStopFloating != null)
			{
				this.OnStopFloating();
			}
		}

		public void Landing(bool down = true)
		{
			this.deltaAttackTime += Time.deltaTime * this.attackSpeed;
			Vector3 position = Vector3.Lerp(this.StartPos, this.EndPos, this.deltaAttackTime);
			float num = this.TrajectoryHeight * Mathf.Sin(Mathf.Clamp01(this.deltaAttackTime) * 3.1415927f);
			position.y -= num;
			base.transform.parent.position = position;
			if (this.V3Equal(base.transform.parent.position, this.EndPos) && !this.GetLanding)
			{
				this.GetLanding = true;
				if (this.OnLanding != null)
				{
					this.OnLanding();
				}
			}
		}

		public void SetTargetPosition(Vector3 currenStartPos, Vector3 targetPosition)
		{
			this.StartPos = currenStartPos;
			this.EndPos = targetPosition;
			this.GetLanding = false;
			this.resetDeltaAttackTime();
		}

		protected void resetDeltaAttackTime()
		{
			if (this.deltaAttackTime > 0f)
			{
				this.deltaAttackTime = 0f;
			}
		}

		protected bool V3Equal(Vector3 a, Vector3 b)
		{
			return Vector3.SqrMagnitude(a - b) < 0.0001f;
		}

		public Vector3 GetRandomWaypointPosition()
		{
			Vector3 result = Vector3.zero;
			do
			{
				int num = Random.Range(0, this.ghost.GhostPath.waypoints.Length);
				this.ghost.GhostPath.NextWayPointVisitedId = num;
				result = this.ghost.GhostPath.GetWaypointPosition(num);
			}
			while (this.ghost.CurrentWayPointId == this.ghost.GhostPath.NextWayPointVisitedId);
			this.ghost.CurrentWayPointId = this.ghost.GhostPath.NextWayPointVisitedId;
			return result;
		}

		public Core.SimpleEvent OnStopFloating;

		public Core.SimpleEvent OnLanding;

		protected Ghost ghost;

		public float amplitudeX = 10f;

		protected float defaultAmplitudeX;

		public float amplitudeY = 5f;

		protected float defaultAmplitudeY;

		public float speedX = 1f;

		public float speedY = 2f;

		private float index;

		private float deltaAttackTime;

		[Range(0f, 1f)]
		public float attackSpeed = 0.75f;
	}
}
