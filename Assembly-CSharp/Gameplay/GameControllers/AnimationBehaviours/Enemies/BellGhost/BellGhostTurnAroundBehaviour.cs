using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellGhost
{
	public class BellGhostTurnAroundBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bellGhost == null)
			{
				this._bellGhost = animator.GetComponentInParent<BellGhost>();
			}
			if (!this._bellGhost.Behaviour.TurningAround)
			{
				this._bellGhost.Behaviour.TurningAround = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._bellGhost.Behaviour.TurningAround)
			{
				this._bellGhost.Behaviour.TurningAround = false;
			}
			EntityOrientation orientation = this._bellGhost.Status.Orientation;
			this._bellGhost.SetOrientation(orientation, true, false);
		}

		private BellGhost _bellGhost;
	}
}
