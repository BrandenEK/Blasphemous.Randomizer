using System;
using Gameplay.GameControllers.Enemies.ChasingHead;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ChasingHead
{
	public class ChasingHeadHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._chasingHead == null)
			{
				this._chasingHead = animator.GetComponentInParent<ChasingHead>();
			}
			this._chasingHead.Status.IsHurt = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._chasingHead.Status.IsHurt = false;
		}

		private ChasingHead _chasingHead;
	}
}
