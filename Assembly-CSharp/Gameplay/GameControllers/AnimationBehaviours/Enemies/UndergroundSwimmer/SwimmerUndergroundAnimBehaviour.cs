using System;
using Gameplay.GameControllers.Enemies.Swimmer;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.UndergroundSwimmer
{
	public class SwimmerUndergroundAnimBehaviour : StateMachineBehaviour
	{
		public Swimmer Swimmer { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Swimmer == null)
			{
				this.Swimmer = animator.GetComponentInParent<Swimmer>();
			}
			this.Swimmer.DamageAreaCollider.enabled = false;
			this.Swimmer.Behaviour.IsTriggerAttack = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.Swimmer)
			{
				this.Swimmer.DamageAreaCollider.enabled = true;
			}
		}
	}
}
