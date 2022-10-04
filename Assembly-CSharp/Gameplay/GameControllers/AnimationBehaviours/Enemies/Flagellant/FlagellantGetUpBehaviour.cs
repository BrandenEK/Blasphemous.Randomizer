using System;
using Gameplay.GameControllers.Enemies.Flagellant;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant
{
	public class FlagellantGetUpBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Flagellant>();
			}
			if (!this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = !this._flagellant.Status.IsHurt;
			}
		}

		private Flagellant _flagellant;
	}
}
