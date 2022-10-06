using System;
using Gameplay.GameControllers.Enemies.BellCarrier;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierDeathBehaviour : StateMachineBehaviour
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
			if ((double)stateInfo.normalizedTime > 0.9)
			{
				Object.Destroy(this._bellCarrier.gameObject);
			}
		}

		private BellCarrier _bellCarrier;
	}
}
