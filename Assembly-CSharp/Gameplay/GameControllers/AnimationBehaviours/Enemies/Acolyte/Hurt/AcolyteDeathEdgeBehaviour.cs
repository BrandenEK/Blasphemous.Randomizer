using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using Gameplay.GameControllers.Enemies.Acolyte.Animator;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Hurt
{
	public class AcolyteDeathEdgeBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.acolyte == null)
			{
				this.acolyte = animator.GetComponentInParent<Acolyte>();
				this.acolyteAttackAnimations = this.acolyte.GetComponent<AcolyteAttackAnimations>();
			}
			if (this.acolyteAttackAnimations != null)
			{
				this.acolyte.Audio.PlayDeathOnCliffLede();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.95f && this.acolyte.Stats.Life.Current <= 0f)
			{
				animator.speed = 0f;
			}
		}

		private Acolyte acolyte;

		private AcolyteAttackAnimations acolyteAttackAnimations;
	}
}
