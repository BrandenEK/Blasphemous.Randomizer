using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbClifLede
{
	public class HangOnCliffLedeBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this.rootMotionDriver == null)
			{
				this.rootMotionDriver = this._penitent.GetComponentInChildren<RootMotionDriver>();
			}
			this.alreadySetCanClimbCliffLede = false;
			animator.ResetTrigger("FORWARD_JUMP");
			animator.ResetTrigger("JUMP");
			this._penitent.IsClimbingCliffLede = true;
			this._penitent.canClimbCliffLede = false;
			this._penitent.PlatformCharacterInput.CancelJump();
			this._penitent.CanLowerCliff = false;
			this._penitent.AnimatorInyector.ResetStuntByFall();
			this._penitent.Physics.Enable2DCollision(false);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.cliffLedeClimbingStarted = true;
			this.SetHangOnOrientation();
			if (!this.alreadySetCanClimbCliffLede && stateInfo.normalizedTime >= 0.2f)
			{
				this.alreadySetCanClimbCliffLede = true;
				this._penitent.canClimbCliffLede = true;
			}
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this._penitent.PlatformCharacterController.InstantVelocity = Vector3.zero;
			this._penitent.Physics.EnablePhysics(false);
			if (stateInfo.normalizedTime <= 0.1f)
			{
				this._penitent.transform.position = this._penitent.RootTargetPosition;
			}
			if (this._penitent.Status.Dead)
			{
				animator.Play(this._hurtInTheAirAnimHash);
			}
			else if (this._penitent.PlatformCharacterInput.Rewired.GetButtonDown(65) && !Core.Input.InputBlocked)
			{
				this.HangOffCliff();
			}
			else if (stateInfo.normalizedTime >= 0.5f && this._penitent.PlatformCharacterInput.isJoystickDown && !Core.Input.InputBlocked)
			{
				this.HangOffCliff();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.Physics.Enable2DCollision(true);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Acceleration = Vector3.zero;
			this._penitent.PlatformCharacterController.InstantVelocity = Vector3.zero;
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this._penitent.Animator.ResetTrigger("AIR_ATTACK");
		}

		private void HangOffCliff()
		{
			if (this._penitent.CanLowerCliff)
			{
				return;
			}
			this._penitent.Physics.EnablePhysics(true);
			this._penitent.CanLowerCliff = true;
			this.HangOffDisplacement();
		}

		private void SetHangOnOrientation()
		{
			if (this._penitent.Status.Orientation != this._penitent.CliffLedeOrientation)
			{
				this._penitent.SetOrientation(this._penitent.CliffLedeOrientation, true, false);
			}
			this.SetMotionDriverPosition(this._penitent.CliffLedeOrientation);
		}

		private void SetMotionDriverPosition(EntityOrientation playerOrientation)
		{
			this._penitent.RootMotionDrive = ((playerOrientation != EntityOrientation.Right) ? this.rootMotionDriver.ReversePosition : this.rootMotionDriver.transform.position);
		}

		private void HangOffDisplacement()
		{
			float num = (this._penitent.Status.Orientation != EntityOrientation.Left) ? -0.5f : 0.5f;
			float endValue = this._penitent.transform.position.x + num;
			this._penitent.transform.DOMoveX(endValue, 0.1f, false).SetEase(Ease.OutSine);
		}

		private Penitent _penitent;

		private RootMotionDriver rootMotionDriver;

		private readonly int _hurtInTheAirAnimHash = Animator.StringToHash("Hurt In The Air");

		private bool alreadySetCanClimbCliffLede;
	}
}
