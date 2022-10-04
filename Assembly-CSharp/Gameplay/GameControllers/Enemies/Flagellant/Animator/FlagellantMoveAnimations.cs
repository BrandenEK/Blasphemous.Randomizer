using System;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Flagellant.Animator
{
	public class FlagellantMoveAnimations : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._flagellant = (Flagellant)this.OwnerEntity;
			this._floorChecker = this._flagellant.EnemyFloorChecker();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this._floorChecker.IsGrounded && !this._floorChecker.IsSideBlocked)
			{
				return;
			}
			if (this._flagellant.MotionLerper.IsLerping)
			{
				this._flagellant.MotionLerper.StopLerping();
			}
		}

		public void PlayFootStep()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlayFootStep();
			}
		}

		public void PlayRunning()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlayRunning();
			}
		}

		public void PlayLanding()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlayLandingSound();
			}
		}

		private Flagellant _flagellant;

		private EnemyFloorChecker _floorChecker;
	}
}
