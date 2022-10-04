using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class HardLandingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = animator.GetComponentInParent<Penitent>();
			}
			this._landingPosition = new Vector2(this._penitent.transform.position.x, this._penitent.transform.position.y);
			this._penitent.PlatformCharacterInput.ResetInputs();
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.ClampHorizontalMovement(this._landingPosition);
			this._penitent.PlatformCharacterInput.ResetInputs();
			this._penitent.PlatformCharacterController.SetActionState(eControllerActions.Jump, false);
			this._penitent.PlatformCharacterInput.IsAttacking = true;
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			this._penitent.Dash.enabled = false;
			this._penitent.Rumble.UsePreset("HardLanding");
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this.ClampHorizontalMovement(this._landingPosition);
			this._penitent.PlatformCharacterInput.IsAttacking = true;
			if (stateInfo.normalizedTime <= 0.5f)
			{
				return;
			}
			if (this._penitent.PlatformCharacterInput.Rewired.GetButton(7))
			{
				this._penitent.CancelEffect.PlayCancelEffect();
				this._penitent.Parry.StopCast();
				this._penitent.Animator.SetTrigger("DASH");
				this._penitent.Animator.ResetTrigger("JUMP");
				this._penitent.Animator.SetBool("DASHING", true);
				this._penitent.Dash.Cast();
			}
			if (Core.InventoryManager.IsRosaryBeadEquipped("RB15"))
			{
				return;
			}
			animator.speed = 1.25f;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitent.PlatformCharacterInput.IsAttacking = false;
			this._penitent.Dash.enabled = true;
			this._penitent.GetComponentInChildren<RangeAttack>().enabled = true;
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
			this._penitent.PlatformCharacterInput.ResetInputs();
			animator.speed = 1f;
		}

		private void ClampHorizontalMovement(Vector2 pos)
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
			float x = (!this._penitent.FloorChecker.OnMovingPlatform) ? pos.x : this._penitent.transform.position.x;
			this._penitent.transform.position = new Vector2(x, this._penitent.transform.position.y);
		}

		private Penitent _penitent;

		private Vector2 _landingPosition;
	}
}
