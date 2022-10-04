using System;
using BezierSplines;
using Sirenix.OdinInspector;
using UnityEngine;

public class BezierSplineDataAccess : MonoBehaviour
{
	[Button(ButtonSizes.Small)]
	public void LoadSpline()
	{
		BezierSplineData bsp = this.database.LoadSpline(this.splineReference);
		this.spline.FromData(bsp);
	}

	[Button(ButtonSizes.Small)]
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
