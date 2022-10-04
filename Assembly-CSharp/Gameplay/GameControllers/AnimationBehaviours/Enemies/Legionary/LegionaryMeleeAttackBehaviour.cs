using System;
using Gameplay.GameControllers.Enemies.Legionary;
using Gameplay.GameControllers.Enemies.Legionary.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Legionary
{
	public class LegionaryMeleeAttackBehaviour : StateMachineBehaviour
	{
		protected Legionary Legionary { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Legionary == null)
			{
				this.Legionary = animator.GetComponentInParent<Legionary>();
			}
			if (!this.Legionary)
			{
				return;
			}
			this.Legionary.IsAttacking = true;
			this.Legionary.Behaviour.ResetHitsCounter();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (!this.Legionary)
			{
				return;
			}
			this.StopSpinningAttackAudio();
			this.Legionary.IsAttacking = false;
			this.Legionary.Behaviour.LookAtTarget(this.Legionary.Target.transform.position);
		}

		private void StopSpinningAttackAudio()
		{
			LegionaryAudio componentInChildren = this.Legionary.GetComponentInChildren<LegionaryAudio>();
			if (componentInChildren)
			{
				componentInChildren.StopSlideAttack_AUDIO();
			}
		}
	}
}
