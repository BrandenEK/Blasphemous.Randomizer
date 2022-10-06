using System;
using Gameplay.GameControllers.Enemies.CauldronNun;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.CauldronNun
{
	public class CauldronNunDeathBehaviour : StateMachineBehaviour
	{
		public CauldronNun CauldronNun { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.CauldronNun == null)
			{
				this.CauldronNun = animator.GetComponentInParent<CauldronNun>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Object.Destroy(this.CauldronNun.gameObject);
		}
	}
}
