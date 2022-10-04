using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Running
{
	public class AcolyteWalkBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			if (this._acolyte.IsAttacking)
			{
				this._acolyte.IsAttacking = false;
			}
		}

		private Acolyte _acolyte;
	}
}
