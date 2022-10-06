using System;
using Gameplay.GameControllers.Enemies.ChasingHead;
using Gameplay.GameControllers.Enemies.HeadThrower.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ChasingHead
{
	public class ChasingHeadDeathBehaviour : StateMachineBehaviour
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
			if (this._chasingHead.OwnHeadThrower != null)
			{
				this._chasingHead.OwnHeadThrower.GetComponentInChildren<HeadThrowerBehaviour>().RemoveSpawnedHeadFromList(this._chasingHead.gameObject);
			}
			Object.Destroy(animator.transform.root.gameObject);
		}

		private ChasingHead _chasingHead;
	}
}
