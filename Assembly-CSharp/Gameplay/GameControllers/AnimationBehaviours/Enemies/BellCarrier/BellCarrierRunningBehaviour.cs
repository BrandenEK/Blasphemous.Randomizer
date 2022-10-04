using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.BellCarrier;
using Gameplay.GameControllers.Enemies.BellCarrier.IA;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierRunningBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bellCarrier == null)
			{
				this._bellCarrier = animator.GetComponentInParent<BellCarrier>();
				this._bellCarrierBehaviour = this._bellCarrier.GetComponentInChildren<BellCarrierBehaviour>();
			}
			this._bellCarrier.BodyBarrier.DisableCollider();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (this._bellCarrierBehaviour.TurningAround)
			{
				this._bellCarrierBehaviour.TurningAround = false;
			}
			if (this._bellCarrierBehaviour.stopWhileChasing)
			{
				this._bellCarrierBehaviour.stopWhileChasing = !this._bellCarrierBehaviour.stopWhileChasing;
			}
			float horizontalInput = (this._bellCarrier.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._bellCarrier.Inputs.HorizontalInput = horizontalInput;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (!this._bellCarrier.BodyBarrier.AvoidCollision)
			{
				this._bellCarrier.BodyBarrier.AvoidCollision = false;
			}
			this._bellCarrier.BodyBarrier.EnableCollider();
		}

		private BellCarrier _bellCarrier;

		private BellCarrierBehaviour _bellCarrierBehaviour;
	}
}
