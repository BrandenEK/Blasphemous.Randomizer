using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce
{
	public class QuirceBossFightPoints : MonoBehaviour
	{
		public Transform GetSwordWallPoint()
		{
			return this.swordWallPoint;
		}

		public Transform GetTeleportPlungeTransform()
		{
			return this.teleportPlungePoints[Random.Range(0, this.teleportPlungePoints.Count)];
		}

		public Transform GetHangTransform(List<QuirceBossFightPoints.QUIRCE_FIGHT_SIDES> valid)
		{
			List<SplinePointInfo> list = this.ceilingHangPoints.FindAll((SplinePointInfo x) => valid.Contains(x.fightSide));
			list.RemoveAll((SplinePointInfo x) => !x.active);
			return list[Random.Range(0, list.Count)].point;
		}

		public SplinePointInfo GetHangPointInfo(Transform t)
		{
			return this.ceilingHangPoints.Find((SplinePointInfo x) => x.point == t);
		}

		public Transform GetDashPointTransform(List<QuirceBossFightPoints.QUIRCE_FIGHT_SIDES> valid)
		{
			List<SplinePointInfo> list = this.startDashPoint.FindAll((SplinePointInfo x) => valid.Contains(x.fightSide));
			list.RemoveAll((SplinePointInfo x) => !x.active);
			return list[Random.Range(0, list.Count)].point;
		}

		public SplinePointInfo GetDashPointInfo(Transform t)
		{
			return this.startDashPoint.Find((SplinePointInfo x) => x.point == t);
		}

		public Vector3 GetCenter()
		{
			return this.roomCenter.position;
		}

		public Vector3 GetTossPoint()
		{
			return this.spiralPoint[Random.Range(0, this.spiralPoint.Count)].position;
		}

		public void ActivateWallMask(bool v)
		{
			this.wallSwordMask.SetActive(v);
		}

		public GameObject wallSwordMask;

		public List<SplinePointInfo> ceilingHangPoints;

		public List<SplinePointInfo> startDashPoint;

		public List<Transform> teleportPlungePoints;

		public List<Transform> spiralPoint;

		public SplinePointInfo spiralPointInfo;

		public Transform roomCenter;

		public Transform swordWallPoint;

		public enum QUIRCE_FIGHT_SIDES
		{
			LEFT,
			RIGHT
		}
	}
}
