using System;
using Gameplay.GameControllers.Enemies.WheelCarrier;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.CrossCrawler
{
	public class WheelCarrierAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.WheelCarrier == null)
			{
				this.WheelCarrier = animator.GetComponentInParent<WheelCarrier>();
			}
			this.WheelCarrier.IsAttacking = true;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this.WheelCarrier.Behaviour.ResetCoolDown();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.WheelCarrier.IsAttacking = false;
			this.WheelCarrier.Behaviour.LookAtTarget(this.WheelCarrier.Target.transform.position);
		}

		protected WheelCarrier WheelCarrier;
	}
}
