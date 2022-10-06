using System;
using BezierSplines;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.AI
{
	public class PatrollingFlyingEnemyBehaviour : EnemyBehaviour
	{
		public void SetPath(BezierSpline s)
		{
			this.currentPath = s;
			this._pathOrigin = this.currentPath.transform.position;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this._pathOrigin = this.currentPath.transform.position;
		}

		public override void OnStart()
		{
			base.OnStart();
			this.PatrollingFlyingEnemy = (PatrollingFlyingEnemy)this.Entity;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.followPath)
			{
				this.FollowPathUpdate();
			}
		}

		private void LateUpdate()
		{
			this.currentPath.transform.position = this._pathOrigin;
		}

		private void FollowPathUpdate()
		{
			float num = this.currentCurve.Evaluate(this._updateCounter / this.secondsToFullLoop);
			Vector3 point = this.currentPath.GetPoint(num);
			this.LookAtTarget(base.transform.position + (point - base.transform.position).normalized * 5f);
			base.transform.position = point;
			this._updateCounter += Time.deltaTime;
			this._updateCounter %= this.secondsToFullLoop;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity.Status.Dead)
			{
				return;
			}
			if (this.Entity.transform.position.x >= targetPos.x + 1f)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					if (this.OnTurning != null)
					{
						this.OnTurning();
					}
					this.Entity.SetOrientation(EntityOrientation.Left, true, false);
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				if (this.OnTurning != null)
				{
					this.OnTurning();
				}
				this.Entity.SetOrientation(EntityOrientation.Right, true, false);
			}
		}

		public void Death()
		{
			this.followPath = false;
			this.PatrollingFlyingEnemy.AnimatorInyector.Death();
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public BezierSpline currentPath;

		public AnimationCurve currentCurve;

		public float secondsToFullLoop;

		private Vector3 _pathOrigin;

		private float _updateCounter;

		private bool followPath = true;

		private PatrollingFlyingEnemy PatrollingFlyingEnemy;
	}
}
