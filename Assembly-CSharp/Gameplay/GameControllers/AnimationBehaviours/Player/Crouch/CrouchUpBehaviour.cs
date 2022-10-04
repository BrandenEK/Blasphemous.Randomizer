using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Crouch
{
	public class CrouchUpBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.PlatformCharacterInput.CancelPlatformDropDown();
			this._penitent.Physics.EnableColliders(true);
			animator.ResetTrigger(CrouchUpBehaviour.JumpOff);
			this._penitent.isJumpOffReady = true;
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this._penitent.PlatformCharacterController.InstantVelocity = Vector3.zero;
			this._penitent.GrabLadder.IsTopLadderReposition = false;
			this._penitent.Dash.CrouchAfterDash = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = !this._penitent.IsCrouchAttacking;
			}
			animator.SetBool(CrouchUpBehaviour.CrouchAttacking, false);
		}

		private Penitent _penitent;

		private static readonly int JumpOff = Animator.StringToHash("JUMP_OFF");

		private static readonly int CrouchAttacking = Animator.StringToHash("CROUCH_ATTACKING");
	}
}
