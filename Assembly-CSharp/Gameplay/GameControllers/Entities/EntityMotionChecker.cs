using System;
using Framework.FrameworkCore;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class EntityMotionChecker : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._bottomHits = new RaycastHit2D[1];
			this._forwardsHits = new RaycastHit2D[1];
			this._patrolHits = new RaycastHit2D[2];
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			int num = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			Vector2 vector = base.transform.position + (float)num * base.transform.right * this.xOffset + base.transform.up * this.yOffset;
			Vector2 vector2 = (!this.useDifferentOffsetForGroundSensors) ? vector : (base.transform.position + (float)num * base.transform.right * this.xOffsetGround + base.transform.up * this.yOffsetGround);
			this.HitsFloor = (Physics2D.LinecastNonAlloc(vector2, vector2 - base.transform.up * this.RangeGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
			Color color = (!this.HitsFloor) ? Color.yellow : Color.green;
			Debug.DrawLine(vector2, vector2 - base.transform.up * this.RangeGroundDetection, color);
			this.HitsBlock = (Physics2D.LinecastNonAlloc(vector, vector + (float)num * base.transform.right * this.RangeBlockDetection, this._forwardsHits, this.BlockLayerMask) > 0);
			int num2 = Physics2D.LinecastNonAlloc(vector, vector + (float)num * base.transform.right * this.RangeBlockDetection, this._patrolHits, this.PatrolBlockLayerMask);
			this.HitsPatrolBlock = (num2 > 0);
			if (num2 == 1)
			{
				Enemy componentInParent = this._patrolHits[0].collider.GetComponentInParent<Enemy>();
				if (componentInParent != null && componentInParent == base.EntityOwner)
				{
					this.HitsPatrolBlock = false;
				}
			}
			color = ((!this.HitsBlock) ? Color.yellow : Color.green);
			Debug.DrawLine(vector, vector + (float)num * base.transform.right * this.RangeBlockDetection, color);
			color = ((!this.HitsPatrolBlock) ? Color.grey : Color.cyan);
			Debug.DrawLine(vector + Vector2.up * 0.05f, vector + Vector2.up * 0.05f + (float)num * base.transform.right * this.RangeBlockDetection, color);
		}

		public bool HitsBlockInPosition(Vector2 pos, Vector2 dir, float range, out Vector2 hitPoint, bool show = false)
		{
			hitPoint = Vector2.zero;
			Vector2 vector = pos + dir * range;
			bool flag = Physics2D.LinecastNonAlloc(pos, vector, this._forwardsHits, this.BlockLayerMask) > 0;
			if (show)
			{
				Color color = (!flag) ? Color.grey : Color.cyan;
				Debug.DrawLine(pos, vector, color, 0.5f);
			}
			if (flag)
			{
				hitPoint = this._forwardsHits[0].point;
			}
			return flag;
		}

		public bool HitsFloorInPosition(Vector2 pos, float range, out Vector2 hitPoint, bool show = false)
		{
			hitPoint = Vector2.zero;
			bool flag = Physics2D.LinecastNonAlloc(pos, pos - base.transform.up * range, this._bottomHits, this.BlockLayerMask) > 0;
			if (show)
			{
				Color color = (!flag) ? Color.grey : Color.cyan;
				Debug.DrawLine(pos, pos - base.transform.up * range, color, 0.5f);
			}
			if (flag)
			{
				hitPoint = this._bottomHits[0].point;
			}
			return flag;
		}

		public void SnapToGround(Transform t, float distance, float skin = 0.05f)
		{
			int num = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			Vector2 vector = base.transform.position + (float)num * base.transform.right * this.xOffset + base.transform.up * this.yOffset;
			bool flag = Physics2D.LinecastNonAlloc(vector, vector - base.transform.up * distance, this._bottomHits, this.BlockLayerMask) > 0;
			if (flag)
			{
				GameplayUtils.DrawDebugCross(base.transform.position, Color.magenta, 10f);
				Debug.DrawLine(vector, this._bottomHits[0].point, Color.green, 10f);
				t.position += this._bottomHits[0].point - vector;
				t.position += base.transform.up * skin;
			}
			else
			{
				Debug.DrawLine(vector, vector - base.transform.up * distance, Color.red, 10f);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position + base.transform.TransformDirection(new Vector2(this.xOffset, this.yOffset)), 0.05f);
			Vector2 vector = base.transform.position + 1f * base.transform.right * this.xOffset + base.transform.up * this.yOffset;
			Vector2 vector2 = (!this.useDifferentOffsetForGroundSensors) ? vector : (base.transform.position + 1f * base.transform.right * this.xOffsetGround + base.transform.up * this.yOffsetGround);
			Gizmos.DrawLine(vector2, vector2 - base.transform.up * this.RangeGroundDetection);
			Gizmos.DrawLine(vector, vector + base.transform.right * this.RangeBlockDetection);
		}

		public LayerMask BlockLayerMask;

		public LayerMask PatrolBlockLayerMask;

		private RaycastHit2D[] _bottomHits;

		private RaycastHit2D[] _forwardsHits;

		private RaycastHit2D[] _patrolHits;

		[Header("Sensors variables")]
		[SerializeField]
		private float xOffset;

		[SerializeField]
		private float yOffset;

		[Space]
		[SerializeField]
		private bool useDifferentOffsetForGroundSensors;

		[SerializeField]
		private float xOffsetGround;

		[SerializeField]
		private float yOffsetGround;

		[Header("Detection Range")]
		[Tooltip("The length of the block detection raycast")]
		[Range(0f, 1f)]
		public float RangeBlockDetection = 0.5f;

		[Tooltip("The length og the ground detection raycast")]
		[Range(0f, 10f)]
		public float RangeGroundDetection = 2f;

		public bool HitsFloor;

		public bool HitsBlock;

		public bool HitsPatrolBlock;
	}
}
