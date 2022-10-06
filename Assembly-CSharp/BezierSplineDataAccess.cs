using System;
using BezierSplines;
using Sirenix.OdinInspector;
using UnityEngine;

public class BezierSplineDataAccess : MonoBehaviour
{
	[Button(0)]
	public void LoadSpline()
	{
		BezierSplineData bezierSplineData = this.database.LoadSpline(this.splineReference);
		this.spline.FromData(bezierSplineData);
	}

	[Button(0)]
	public void SaveSpline()
	{
		this.database.SaveSpline(this.spline, this.splineReference);
	}

	private void SetSpline(BezierSpline newSpline)
	{
		this.spline = newSpline;
	}

	public BezierSpline spline;

	public BezierSplineScriptableDatabase database;

	public SPLINE_TYPES splineReference;
}
