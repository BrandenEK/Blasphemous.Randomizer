using System;
using Gameplay.GameControllers.Enemies.BellCarrier;
using Gameplay.GameControllers.Enemies.BellCarrier.IA;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierStartRunBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bellCarrier != null)
			{
				return;
			}
			this._bellCarrier = animator.GetComponentInParent<BellCarrier>();
			this._bellCarrierBehaviour = this._bellCarrier.GetComponentInChildren<BellCarrierBehaviour>();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime < this.NormalizedTimeBeforeRunning)
			{
				return;
			}
			if (this._bellCarrierBehaviour.IsChasing && !this._bellCarrierBehaviour.IsBlocked)
			{
				this._bellCarrierBehaviour.StartMovement();
			}
		}

		private BellCarrier _bellCarrier;

		private BellCarrierBehaviour _bellCarrierBehaviour;

		[Range(0f, 1f)]
		public float NormalizedTimeBeforeRunning;
	}
}
