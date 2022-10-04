using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Running
{
	public class AcolyteRunningBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			if (this._acolyte.IsAttacking)
			{
				this._acolyte.IsAttacking = false;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				return;
			}
			if (!this._acolyte.IsChasing)
			{
				this._acolyte.IsChasing = true;
			}
		}

		private Acolyte _acolyte;
	}
}
