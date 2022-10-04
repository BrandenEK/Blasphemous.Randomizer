using System;
using System.Collections.Generic;
using BezierSplines;
using UnityEngine;

[CreateAssetMenu(fileName = "InputIconLayout", menuName = "Blasphemous/Maikel/SplineDatabase")]
public class BezierSplineScriptableDatabase : ScriptableObject
{
	public void SaveSpline(BezierSpline s, SPLINE_TYPES type)
	{
		BezierSplineConfig bezierSplineConfig = this.splines.Find((BezierSplineConfig x) => x.type == type);
		if (bezierSplineConfig == null)
		{
			this.splines.Add(new BezierSplineConfig(s.ToData(), type));
		}
		else
		{
			bezierSplineConfig.spline = s.ToData();
		}
	}

	public BezierSplineData LoadSpline(SPLINE_TYPES type)
	{
		return this.splines.Find((BezierSplineConfig x) => x.type == type).spline;
	}

	public List<BezierSplineConfig> splines;
}
