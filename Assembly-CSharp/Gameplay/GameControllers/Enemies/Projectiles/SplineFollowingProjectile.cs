using System;
using System.Diagnostics;
using BezierSplines;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class SplineFollowingProjectile : Projectile
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SplineFollowingProjectile, float, float> OnSplineAdvancedEvent;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SplineFollowingProjectile> OnSplineCompletedEvent;

		public virtual void Init(Vector3 origin, BezierSpline spline, float totalSeconds, AnimationCurve curve)
		{
			this._spline = spline;
			this.totalSeconds = totalSeconds;
			this.elapsedSeconds = 0f;
			this._curve = curve;
			base.transform.position = spline.GetPoint(0f);
			this.play = true;
			this.finished = false;
		}

		protected override void OnUpdate()
		{
			if (this.play)
			{
				float time = this.elapsedSeconds / this.totalSeconds;
				Vector3 point = this._spline.GetPoint(this._curve.Evaluate(time));
				base.transform.position = point;
				if (this.OnSplineAdvancedEvent != null)
				{
					this.OnSplineAdvancedEvent(this, this.totalSeconds, this.elapsedSeconds);
				}
				this.elapsedSeconds += Time.deltaTime;
				if (this.elapsedSeconds >= this.totalSeconds)
				{
					this.OnTotalSecondsElapsed();
				}
			}
		}

		public void Stop()
		{
			this.play = false;
			this.finished = true;
		}

		public bool IsFollowing()
		{
			return this.play;
		}

		private void OnTotalSecondsElapsed()
		{
			this.finished = true;
			this.play = false;
			if (this.OnSplineCompletedEvent != null)
			{
				this.OnSplineCompletedEvent(this);
			}
		}

		public void ForceDestroy()
		{
			base.OnLifeEnded();
		}

		public bool faceVelocityDirection;

		private BezierSpline _spline;

		public float totalSeconds;

		public float elapsedSeconds;

		private AnimationCurve _curve;

		private bool play;

		private bool finished;
	}
}
