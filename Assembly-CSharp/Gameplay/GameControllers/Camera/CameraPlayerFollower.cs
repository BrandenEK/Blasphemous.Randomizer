using System;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	[RequireComponent(typeof(ProCamera2D))]
	public class CameraPlayerFollower : MonoBehaviour
	{
		private void Start()
		{
			this.procamera2D = base.GetComponent<ProCamera2D>();
		}

		public void FollowPlayer(bool follow)
		{
			this.procamera2D.FollowHorizontal = follow;
			this.procamera2D.FollowVertical = follow;
		}

		private ProCamera2D procamera2D;
	}
}
