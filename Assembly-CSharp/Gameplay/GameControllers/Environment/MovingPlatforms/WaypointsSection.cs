using System;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	[Serializable]
	public struct WaypointsSection
	{
		public Transform StartingPoint;

		public Transform EndingPoint;

		public Ease horizontalEase;

		public Ease verticalEase;

		public float speed;

		public float waitTimeAtDestination;
	}
}
