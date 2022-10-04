using System;
using Gameplay.GameControllers.Enemies.Swimmer;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.UndergroundSwimmer
{
	public class UndergroundSwimmerAscendingBehaviour : StateMachineBehaviour
	{
		protected Swimmer Swimmer { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Swimmer == null)
			{
				this.Swimmer = animator.GetComponentInParent<Swimmer>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			float vspeed = this.Swimmer.Controller.PlatformCharacterPhysics.VSpeed;
			if (vspeed < -0.1f)
			{
				animator.Play("MaxHeight");
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
		}
	}
}
