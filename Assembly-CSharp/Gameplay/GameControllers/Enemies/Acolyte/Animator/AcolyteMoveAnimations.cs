using System;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Acolyte.Animator
{
	public class AcolyteMoveAnimations : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._acolyte = (Acolyte)this.OwnerEntity;
			this._floorChecker = this._acolyte.EnemyFloorChecker();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._floorChecker.IsGrounded && this._acolyte.MotionLerper.IsLerping)
			{
				this._acolyte.MotionLerper.StopLerping();
			}
		}

		public void PlayFootStep()
		{
			if (this._acolyte.Status.IsVisibleOnCamera)
			{
				this._acolyte.Audio.PlayFootStep();
			}
		}

		public void PlayRunning()
		{
			if (this._acolyte.Status.IsVisibleOnCamera)
			{
				this._acolyte.Audio.PlayRunning();
			}
		}

		public void PlayStopRunning()
		{
			if (this._acolyte.Status.IsVisibleOnCamera)
			{
				this._acolyte.Audio.PlayStopRunning();
			}
		}

		public void PlayLanding()
		{
			if (this._acolyte.Status.IsVisibleOnCamera)
			{
				this._acolyte.Audio.PlayLanding();
			}
		}

		private Acolyte _acolyte;

		private EnemyFloorChecker _floorChecker;
	}
}
