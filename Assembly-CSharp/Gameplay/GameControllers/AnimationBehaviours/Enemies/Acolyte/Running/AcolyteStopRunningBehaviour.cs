using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Running
{
	public class AcolyteStopRunningBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
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

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				return;
			}
			if (this._acolyte.IsChasing)
			{
				this._acolyte.IsChasing = !this._acolyte.IsChasing;
			}
		}

		private Acolyte _acolyte;
	}
}
