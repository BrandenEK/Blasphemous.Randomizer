using System;
using System.Collections.Generic;
using BezierSplines;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce
{
	[Serializable]
	public struct SplinePointInfo
	{
		public Transform point;

		public QuirceBossFightPoints.QUIRCE_FIGHT_SIDES fightSide;

		public List<QuirceBossFightPoints.QUIRCE_FIGHT_SIDES> nextValidPoints;

		public BezierSpline spline;

		public AnimationCurve speedCurve;

		public float time;

		public bool active;
	}
}
