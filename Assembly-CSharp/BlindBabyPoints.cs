using System;
using System.Collections.Generic;
using BezierSplines;
using Gameplay.GameControllers.Bosses.BlindBaby;
using UnityEngine;

public class BlindBabyPoints : MonoBehaviour
{
	public List<Transform> GetMultiAttackPoints(WickerWurmBehaviour.WICKERWURM_SIDES side)
	{
		if (side == WickerWurmBehaviour.WICKERWURM_SIDES.LEFT)
		{
			return this.multiAttackPointsLeft;
		}
		return this.multiAttackPointsRight;
	}

	public BlindBabyPoints.WickerWurmPathConfig GetPathConfig(BlindBabyPoints.WURM_PATHS name)
	{
		List<BlindBabyPoints.WickerWurmPathConfig> list = this.paths.FindAll((BlindBabyPoints.WickerWurmPathConfig x) => x.pathName == name);
		int index = Random.Range(0, list.Count);
		return list[index];
	}

	public BlindBabyPoints.WickerWurmPathConfig GetPathConfig(int index)
	{
		return this.paths[index];
	}

	public List<Transform> multiAttackPointsRight;

	public List<Transform> multiAttackPointsLeft;

	public List<BlindBabyPoints.WickerWurmPathConfig> paths;

	public enum WURM_PATHS
	{
		TO_RIGHT,
		RIGHT_TO_FIX,
		TO_LEFT,
		LEFT_TO_FIX
	}

	[Serializable]
	public struct WickerWurmPathConfig
	{
		public BlindBabyPoints.WURM_PATHS pathName;

		public AnimationCurve curve;

		public float duration;

		public BezierSpline spline;

		public WickerWurmBehaviour.WICKERWURM_SIDES side;
	}
}
