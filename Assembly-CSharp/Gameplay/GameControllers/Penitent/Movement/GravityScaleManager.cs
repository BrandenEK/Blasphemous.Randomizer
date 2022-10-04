using System;
using CreativeSpore.SmartColliders;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.InputSystem;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Movement
{
	[RequireComponent(typeof(Entity))]
	[RequireComponent(typeof(PlatformCharacterController))]
	[RequireComponent(typeof(PlatformCharacterInput))]
	public class GravityScaleManager : MonoBehaviour
	{
		private void Awake()
		{
			this._platformCharacterController = base.GetComponent<PlatformCharacterController>();
		}

		private void Start()
		{
			this._gravityIsModified = this._platformCharacterController.IsGrounded;
		}

		private void Update()
		{
			if (this.gravityScaleEnabled)
			{
				this.characterGravityScaling();
			}
		}

		private void characterGravityScaling()
		{
			if (!this._platformCharacterController.IsGrounded)
			{
				if (this._gravityIsModified)
				{
					return;
				}
				this._gravityIsModified = true;
				this.SetGravityScale(2.5f);
			}
			else
			{
				if (!this._gravityIsModified)
				{
					return;
				}
				this._gravityIsModified = !this._gravityIsModified;
				this.SetGravityScale(0f);
			}
		}

		public void SetGravityScale(float gravityScale)
		{
			if (!Mathf.Approximately(this._platformCharacterController.PlatformCharacterPhysics.GravityScale, gravityScale))
			{
				this._platformCharacterController.PlatformCharacterPhysics.GravityScale = gravityScale;
			}
		}

		public void SetGravity(float gravity)
		{
			if (!Mathf.Approximately(this._platformCharacterController.PlatformCharacterPhysics.Gravity.y, gravity))
			{
				Vector3 gravity2 = new Vector3(0f, gravity, 0f);
				this._platformCharacterController.PlatformCharacterPhysics.Gravity = gravity2;
			}
		}

		private PlatformCharacterController _platformCharacterController;

		public const float MAX_GRAVITY_SCALE = 2.5f;

		public const float MIN_GRAVITY_SCALE = 0f;

		public const float MAX_GRAVITY = 9.8f;

		public bool gravityScaleEnabled;

		private bool _gravityIsModified;
	}
}
