using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class LandingRunningBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.IsFallingStunt)
			{
				animator.Play(this._landingAnim);
			}
			else
			{
				this._penitent.PenitentMoveAnimations.PlayLandingRunning();
			}
			if (this._penitent.PlatformCharacterController.CurrentClimbingCollider != null)
			{
				this._penitent.PlatformCharacterController.CurrentClimbingCollider = null;
			}
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
		}

		private Penitent _penitent;

		private bool _setSlidingEffect;

		private readonly int _landingAnim = Animator.StringToHash("Landing");
	}
}
