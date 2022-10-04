using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class ReleaseBottomLadderBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			animator.speed = 1f;
			this._penitent.Physics.EnablePhysics(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.IsGrabbingLadder = false;
			this._penitent.Physics.EnablePhysics(true);
		}

		private Penitent _penitent;
	}
}
