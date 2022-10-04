using System;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class LadderSlidingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._setMaxSpeed = false;
			this._isSlidingLoopSound = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.IsLadderSliding && !this._setMaxSpeed)
			{
				this._setMaxSpeed = true;
				this._penitent.GrabLadder.SetClimbingSpeed(6.75f);
			}
			if (!this._isSlidingLoopSound)
			{
				this._isSlidingLoopSound = true;
				this._penitent.Audio.SlidingLadder(out this._slidingSoundEvent);
			}
			if (this._penitent.ReachBottonLadder)
			{
				this._isSlidingLoopSound = false;
				this._penitent.GrabLadder.EnableClimbLadderAbility(false);
				animator.Play("Idle");
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._isSlidingLoopSound)
			{
				this._isSlidingLoopSound = !this._isSlidingLoopSound;
			}
			this._penitent.Audio.StopSlidingLadder(this._slidingSoundEvent);
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
		}

		private void TriggerLadderLandingFxs()
		{
			if (this._penitent.PlatformCharacterController.GroundDist >= 0.5f)
			{
				return;
			}
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("LadderLanding");
			this._penitent.PenitentMoveAnimations.PlaySlidingLadderLanding();
		}

		private Penitent _penitent;

		private bool _isSlidingLoopSound;

		private bool _setMaxSpeed;

		private EventInstance _slidingSoundEvent;
	}
}
