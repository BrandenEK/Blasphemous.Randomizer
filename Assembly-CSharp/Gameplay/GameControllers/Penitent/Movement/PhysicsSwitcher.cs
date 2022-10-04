using System;
using CreativeSpore.SmartColliders;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Movement
{
	public class PhysicsSwitcher : MonoBehaviour
	{
		public SmartPlatformCollider Collider { get; private set; }

		private void Awake()
		{
			this._playerController = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
		}

		public void EnablePhysics(bool enable = true)
		{
			if (!this._playerController.enabled && enable)
			{
				this._playerController.enabled = true;
			}
			else if (this._playerController.enabled && !enable)
			{
				this._playerController.enabled = false;
			}
		}

		public void Enable2DCollision(bool enable = true)
		{
			if (!this.Collider.EnableCollision2D && enable)
			{
				this.Collider.EnableCollision2D = true;
			}
			else if (this.Collider.EnableCollision2D && !enable)
			{
				this.Collider.EnableCollision2D = false;
			}
		}

		public void EnableColliders(bool enable = true)
		{
			if (!this.Collider.enabled && enable)
			{
				this.Collider.enabled = true;
			}
			else if (this.Collider.enabled && !enable)
			{
				this.Collider.enabled = false;
			}
		}

		private bool _enablePhysics;

		private PlatformCharacterController _playerController;
	}
}
