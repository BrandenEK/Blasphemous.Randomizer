using System;
using Gameplay.GameControllers.Enemies.JarThrower;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.JarThrower
{
	public class JarThrowerLandingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Jarthrower == null)
			{
				this.Jarthrower = animator.GetComponentInParent<JarThrower>();
			}
			this.Jarthrower.IsRunLanding = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Jarthrower.IsRunLanding = false;
		}

		protected JarThrower Jarthrower;
	}
}
