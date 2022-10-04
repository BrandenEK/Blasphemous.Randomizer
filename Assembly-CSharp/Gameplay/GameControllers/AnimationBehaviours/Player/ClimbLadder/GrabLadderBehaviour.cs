using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class GrabLadderBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.IsGrabbingLadder = true;
			this._penitent.GrabLadder.SetClimbingSpeed(0f);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._penitent.Status.IsGrounded)
			{
				animator.Play(this._ladderGoingUpAnimHash);
			}
			if (stateInfo.normalizedTime >= 0.95f)
			{
				animator.Play(this._ladderGoingUpAnimHash);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.IsClimbingLadder = true;
			this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
		}

		private Penitent _penitent;

		private readonly int _ladderGoingUpAnimHash = Animator.StringToHash("ladder_going_up");
	}
}
