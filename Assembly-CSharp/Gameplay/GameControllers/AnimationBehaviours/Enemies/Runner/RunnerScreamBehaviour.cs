using System;
using Gameplay.GameControllers.Enemies.Runner;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Runner
{
	public class RunnerScreamBehaviour : StateMachineBehaviour
	{
		protected Runner Runner { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Runner == null)
			{
				this.Runner = animator.GetComponentInParent<Runner>();
			}
			this.Runner.Behaviour.IsScreaming = true;
			this.Runner.Behaviour.Stop();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Runner.Behaviour.IsScreaming = false;
		}
	}
}
