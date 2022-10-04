using System;
using Gameplay.GameControllers.Enemies.Legionary;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Legionary
{
	public class LegionaryLightningSummonBehaviour : StateMachineBehaviour
	{
		protected Legionary Legionary { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Legionary == null)
			{
				this.Legionary = animator.GetComponentInParent<Legionary>();
			}
			this.Legionary.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Legionary.IsAttacking = false;
			this.Legionary.Behaviour.ResetHitsCounter();
		}
	}
}
