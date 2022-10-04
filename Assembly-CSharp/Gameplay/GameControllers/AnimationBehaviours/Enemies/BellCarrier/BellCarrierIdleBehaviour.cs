using System;
using Gameplay.GameControllers.Enemies.BellCarrier;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bellCarrier == null)
			{
				this._bellCarrier = animator.GetComponentInParent<BellCarrier>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!this._bellCarrier.BellCarrierBehaviour.TargetInLine && animator.GetBool("CHASING"))
			{
				animator.Play(this._turnAround);
			}
		}

		private BellCarrier _bellCarrier;

		private readonly int _turnAround = Animator.StringToHash("TurnAround");
	}
}
