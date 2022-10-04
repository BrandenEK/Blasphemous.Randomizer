using System;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class LandingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._cameraManager == null)
			{
				this._cameraManager = Core.Logic.CameraManager;
			}
			this._setSlidingEffects = false;
			if (this._penitent.IsFallingStunt)
			{
				this._penitent.AnimatorInyector.ResetStuntByFall();
				this._cameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
				this._penitent.GetComponentInChildren<RangeAttack>().enabled = false;
				animator.Play(this._hardLandingHashAnim, 0, 0f);
			}
			else
			{
				this._penitent.Audio.PlayLandingSound();
			}
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._penitent.Status.IsGrounded)
			{
				return;
			}
			if (this._penitent.ReachBottonLadder && !this._setSlidingEffects && this._penitent.IsGrabbingLadder)
			{
				this._setSlidingEffects = true;
				Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("LadderLanding");
				this._penitent.PenitentMoveAnimations.PlaySlidingLadderLanding();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
		}

		private Penitent _penitent;

		private CameraManager _cameraManager;

		private bool _setSlidingEffects;

		private readonly int _hardLandingHashAnim = Animator.StringToHash("HardLanding");
	}
}
