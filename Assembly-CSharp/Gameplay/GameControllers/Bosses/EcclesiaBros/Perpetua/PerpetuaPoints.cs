using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	public class PerpetuaPoints : MonoBehaviour
	{
		public Vector2 GetCenterPos()
		{
			return this.centerPoint.position;
		}

		public Transform centerPoint;
	}
}
