using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceBossFightPoints : MonoBehaviour
	{
		public Transform GetNunPoint()
		{
			return this.nunPoint;
		}

		public Transform GetHeadPoint(string id)
		{
			List<Transform> points = this.headPoints.Find((PointsByPatternId x) => x.id == id).points;
			return points[Random.Range(0, points.Count)];
		}

		public Transform GetRosaryPoint(string id, bool secondary = false)
		{
			if (secondary)
			{
				id = string.Format("{0}_{1}", id, "SEC");
			}
			List<Transform> list = new List<Transform>(this.rosaryPoints.Find((PointsByPatternId x) => x.id == id).points);
			list.Remove(this._lastRosaryPoint);
			Transform transform = list[Random.Range(0, list.Count)];
			this._lastRosaryPoint = transform;
			return transform;
		}

		public List<PointsByPatternId> rosaryPoints;

		public List<PointsByPatternId> headPoints;

		public Transform nunPoint;

		private Transform _lastRosaryPoint;
	}
}
