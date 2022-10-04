using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Crouch
{
	public class CrouchDownBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.IsFallingStunt)
			{
				animator.Play("Landing");
			}
			this._penitent.BeginCrouch = true;
			if (this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = !this._penitent.IsCrouchAttacking;
			}
			float timeGrounded = this._penitent.AnimatorInyector.TimeGrounded;
			if (timeGrounded < 0.05f)
			{
				this._penitent.Audio.PlayLandingForward();
			}
			if (this._penitent.StepOnLadder)
			{
				this._penitent.GrabLadder.SetClimbingSpeed(0f);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(CrouchDownBehaviour.CrouchAttacking, false);
			if (stateInfo.normalizedTime >= stateInfo.length * this.allowJumpOffReadyLenght)
			{
				this._penitent.isJumpOffReady = true;
			}
			if (this._penitent.PlatformCharacterInput.Attack && !this._penitent.IsCrouchAttacking)
			{
				animator.SetBool(CrouchDownBehaviour.CrouchAttacking, true);
				animator.Play("Crouch Attack");
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.BeginCrouch = false;
			this._penitent.isJumpOffReady = false;
			if (this._penitent.GrabLadder.GetClimbingSpeed() < 2.25f)
			{
				this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
			}
		}

		private Penitent _penitent;

		[Range(0f, 1f)]
		public float allowJumpOffReadyLenght;

		private static readonly int CrouchAttacking = Animator.StringToHash("CROUCH_ATTACKING");
	}
}
