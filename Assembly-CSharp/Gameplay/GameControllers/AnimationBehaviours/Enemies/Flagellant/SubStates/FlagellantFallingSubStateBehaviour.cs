using System;
using Gameplay.GameControllers.Enemies.Flagellant;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant.SubStates
{
	public class FlagellantFallingSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Flagellant>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._flagellant.IsFalling = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._flagellant.IsFalling = false;
		}

		private Flagellant _flagellant;
	}
}
