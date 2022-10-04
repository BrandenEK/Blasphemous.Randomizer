using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	[RequireComponent(typeof(Animator))]
	public class PlatformGear : MonoBehaviour
	{
		public StraightMovingPlatform MovingPlatform { get; set; }

		private Animator GearAnimator { get; set; }

		private void Awake()
		{
			this.GearAnimator = base.GetComponent<Animator>();
			this.MovingPlatform = base.GetComponentInParent<StraightMovingPlatform>();
		}

		private void Update()
		{
			if (!this.MovingPlatform)
			{
				return;
			}
			this.EnableGearMotion(this.MovingPlatform.IsRunning);
		}

		public void EnableGearMotion(bool enableGear)
		{
			this.GearAnimator.speed = ((!enableGear) ? 0f : 1f);
		}
	}
}
