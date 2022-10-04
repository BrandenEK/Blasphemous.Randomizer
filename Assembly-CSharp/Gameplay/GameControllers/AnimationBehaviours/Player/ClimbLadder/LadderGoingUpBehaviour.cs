using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Animator;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class LadderGoingUpBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._animatorInyector = this._penitent.GetComponentInChildren<AnimatorInyector>();
			}
			this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
			this._penitent.DamageArea.IncludeEnemyLayer(false);
			if (this._animatorInyector.ForwardJump)
			{
				this._animatorInyector.ForwardJump = !this._animatorInyector.ForwardJump;
			}
			if (!this._penitent.CanJumpFromLadder)
			{
				this._penitent.CanJumpFromLadder = true;
			}
			this._lastPosition = this._penitent.transform.position;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._currentPosition = this._penitent.transform.position;
			if (!this._penitent.IsClimbingLadder)
			{
				this._penitent.JumpFromLadder = true;
				animator.Play(this._jumpForwardAnimHash);
			}
			if (this._penitent.ReachTopLadder)
			{
				this._penitent.transform.position = this._penitent.RootMotionDrive;
				animator.Play(this._releaseLadderToUpAnimHash);
			}
			else if (this._penitent.PlatformCharacterInput.FVerAxis > this.verticalAxisThreshold)
			{
				animator.speed = 1f;
			}
			else if (this._penitent.PlatformCharacterInput.FVerAxis < 0f && this._currentPosition != this._lastPosition)
			{
				animator.speed = 1f;
				animator.Play(this._ladderGoingDownAnimHash);
			}
			else
			{
				animator.speed = 0f;
			}
			this._lastPosition = this._currentPosition;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.DamageArea.IncludeEnemyLayer(true);
			animator.speed = 1f;
			this._penitent.ReachTopLadder = false;
			this._penitent.RootMotionDrive = Vector3.zero;
		}

		private readonly int _jumpForwardAnimHash = Animator.StringToHash("Jump Forward");

		private readonly int _releaseLadderToUpAnimHash = Animator.StringToHash("release_ladder_to_floor_up");

		private readonly int _ladderGoingDownAnimHash = Animator.StringToHash("ladder_going_down");

		private Penitent _penitent;

		private AnimatorInyector _animatorInyector;

		private bool _soundClimbingLadder;

		private Vector2 _currentPosition;

		private Vector2 _lastPosition;

		[Range(0f, 1f)]
		public float verticalAxisThreshold = 0.5f;
	}
}
