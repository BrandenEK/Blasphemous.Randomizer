using System;
using Gameplay.GameControllers.Enemies.ShieldMaiden;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ShieldMaiden
{
	public class ShieldMaidenParryBehaviour : StateMachineBehaviour
	{
		public ShieldMaiden ShieldMaiden { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ShieldMaiden == null)
			{
				this.ShieldMaiden = animator.GetComponentInParent<ShieldMaiden>();
			}
			this.ShieldMaiden.Behaviour.ToggleShield(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
		}
	}
}
