using System;
using Gameplay.GameControllers.Environment.Traps.SpikesTrap;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Traps
{
	public class SpikeTrapFallBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.spikeTrap == null)
			{
				this.spikeTrap = animator.GetComponent<SpikeTrap>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.spikeTrap.SpikeTrapCollider.enabled)
			{
				this.spikeTrap.SpikeTrapCollider.enabled = false;
			}
		}

		private SpikeTrap spikeTrap;
	}
}
