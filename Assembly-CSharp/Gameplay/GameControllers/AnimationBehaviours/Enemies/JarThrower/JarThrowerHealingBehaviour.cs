using System;
using Gameplay.GameControllers.Enemies.JarThrower;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.JarThrower
{
	public class JarThrowerHealingBehaviour : StateMachineBehaviour
	{
		protected JarThrower JarThrower { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.JarThrower == null)
			{
				this.JarThrower = animator.GetComponentInParent<JarThrower>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.JarThrower.Behaviour.IsHealing = false;
		}
	}
}
