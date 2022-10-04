using System;
using Gameplay.GameControllers.Enemies.BellCarrier;
using Gameplay.GameControllers.Enemies.BellCarrier.IA;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierTurnAroundBehaviour : StateMachineBehaviour
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
			this._bellCarrier.AnimatorInyector.ResetTurnAround();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._bellCarrier.EnemyBehaviour.ReverseOrientation();
			if (this._bellCarrierBehaviour.TurningAround)
			{
				this._bellCarrierBehaviour.TurningAround = false;
			}
			if (this._bellCarrier.BellCarrierBehaviour.WallHit)
			{
				this._bellCarrier.BellCarrierBehaviour.WallHit = false;
			}
			if (this._bellCarrierBehaviour.IsChasing)
			{
				this._bellCarrierBehaviour.ResetTimeChasing();
			}
		}

		private BellCarrier _bellCarrier;

		private BellCarrierBehaviour _bellCarrierBehaviour;
	}
}
