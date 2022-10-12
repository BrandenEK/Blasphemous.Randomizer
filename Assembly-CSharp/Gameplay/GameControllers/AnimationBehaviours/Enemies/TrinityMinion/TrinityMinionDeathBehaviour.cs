using System;
using Gameplay.GameControllers.Enemies.TrinityMinion;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.TrinityMinion
{
	public class TrinityMinionDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._TrinityMinion == null)
			{
				this._TrinityMinion = animator.GetComponentInParent<TrinityMinion>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if ((double)stateInfo.normalizedTime > 0.9)
			{
				UnityEngine.Object.Destroy(this._TrinityMinion.gameObject);
			}
		}

		private TrinityMinion _TrinityMinion;
	}
}
