using System;
using System.Diagnostics;
using BezierSplines;
using Sirenix.OdinInspector;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Vector2> OnMovingToNextPoint;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnMovementCompleted;

	public void SetData(SplineThrowData data)
	{
		this.spline = data.spline;
		this.movementCurve = data.curve;
		this.duration = data.duration;
	}

	private void Update()
	{
		if (this.followActivated)
		{
			this.FollowSpline();
		}
	}

	public Vector2 GetDirection()
	{
		float t = this.movementCurve.Evaluate(this.currentCounter / this.duration);
		return this.spline.GetDirection(t);
	}

	public bool HasFinished()
	{
		return this.currentCounter == this.duration;
	}

	public void StartFollowing(bool loop)
	{
		this.loop = loop;
		this.currentCounter = 0f;
		this.followActivated = true;
	}

	private void FollowSpline()
	{
		float t = this.movementCurve.Evaluate(this.currentCounter / this.duration);
		Vector3 point = this.spline.GetPoint(t);
		this.BeforeMoving(point);
		base.transform.position = point;
		if (!this.loop && this.currentCounter == this.duration)
		{
			this.followActivated = false;
			if (this.OnMovementCompleted != null)
			{
				this.OnMovementCompleted();
			}
			return;
		}
		this.currentCounter += Time.deltaTime;
		if (this.loop)
		{
			this.currentCounter %= this.duration;
		}
		else if (this.currentCounter > this.duration)
		{
			this.currentCounter = this.duration;
		}
	}

	private void BeforeMoving(Vector2 nextPoint)
	{
		if (this.OnMovingToNextPoint != null)
		{
			this.OnMovingToNextPoint(nextPoint);
		}
	}

	[FoldoutGroup("References", 0)]
	public BezierSpline spline;

	[FoldoutGroup("Following spline config", 0)]
	public float currentCounter;

	[FoldoutGroup("Following spline config", 0)]
	public float duration = 10f;

	[FoldoutGroup("Following spline config", 0)]
	public AnimationCurve movementCurve;

	[FoldoutGroup("Runtime Behaviour", 0)]
	public bool followActivated;

	[FoldoutGroup("Runtime Behaviour", 0)]
	public bool loop;
}
