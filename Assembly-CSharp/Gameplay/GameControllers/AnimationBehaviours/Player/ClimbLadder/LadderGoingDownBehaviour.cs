using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Animator;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class LadderGoingDownBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._animatorInyector = this._penitent.GetComponentInChildren<AnimatorInyector>();
			}
			if (this._penitent.StartingGoingDownLadders)
			{
				this._penitent.StartingGoingDownLadders = !this._penitent.StartingGoingDownLadders;
			}
			this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
			if (this._animatorInyector.ForwardJump)
			{
				this._animatorInyector.ForwardJump = !this._animatorInyector.ForwardJump;
			}
			if (!this._penitent.CanJumpFromLadder)
			{
				this._penitent.CanJumpFromLadder = true;
			}
			if (this._penitent.IsCrouched)
			{
				this._penitent.IsCrouched = false;
			}
			this._lastPosition = new Vector2(this._penitent.transform.position.x, this._penitent.transform.position.y);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._currentPosition = this._penitent.transform.position;
			if (!this._penitent.IsClimbingLadder)
			{
				this._penitent.JumpFromLadder = true;
				animator.Play(this._jumpForwardAnimHash);
			}
			if (this._penitent.PlatformCharacterInput.FVerAxis > 0f && this._currentPosition != this._lastPosition)
			{
				this._lastPosition = this._currentPosition;
				animator.speed = 1f;
				animator.Play(this._ladderGoingUpAnimHash);
			}
			else if (this._penitent.PlatformCharacterInput.FVerAxis <= -this.verticalAxisThreshold)
			{
				animator.speed = 1f;
			}
			else
			{
				animator.speed = 0f;
			}
			if (!this._penitent.IsLadderSliding)
			{
				this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.speed = 1f;
		}

		private readonly int _jumpForwardAnimHash = Animator.StringToHash("Jump Forward");

		private readonly int _ladderGoingUpAnimHash = Animator.StringToHash("ladder_going_up");

		private Penitent _penitent;

		private AnimatorInyector _animatorInyector;

		private bool _soundClimbingLadder;

		private Vector2 _currentPosition;

		private Vector2 _lastPosition;

		[Range(0f, 1f)]
		public float verticalAxisThreshold = 0.5f;
	}
}
