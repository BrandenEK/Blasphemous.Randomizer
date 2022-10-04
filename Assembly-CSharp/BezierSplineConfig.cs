using System;
using BezierSplines;

[Serializable]
public class BezierSplineConfig
{
	public BezierSplineConfig(BezierSplineData _spline, SPLINE_TYPES _type)
	{
		this.spline = _spline;
		this.type = _type;
	}

	public BezierSplineData spline;

	public SPLINE_TYPES type;
}
