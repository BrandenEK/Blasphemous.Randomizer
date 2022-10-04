using System;
using Gameplay.GameControllers.Effects.Player.Dust;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Player
{
	public class StepDustBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._stepDust == null)
			{
				this._stepDust = animator.gameObject.GetComponent<StepDust>();
			}
			if (this._stepDustSpawner == null)
			{
				this._stepDustSpawner = this._stepDust.Owner.GetComponentInChildren<StepDustSpawner>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._stepDustSpawner != null)
			{
				this._stepDustSpawner.StoreStepDust(this._stepDust);
			}
		}

		private StepDust _stepDust;

		private StepDustSpawner _stepDustSpawner;
	}
}
