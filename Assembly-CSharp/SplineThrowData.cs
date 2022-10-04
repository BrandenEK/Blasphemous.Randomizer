using System;
using BezierSplines;
using UnityEngine;

[Serializable]
public class SplineThrowData
{
	public float duration;

	public AnimationCurve curve;

	public BezierSpline spline;
}
