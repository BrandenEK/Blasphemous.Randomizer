using System;
using Gameplay.GameControllers.Bosses.BejeweledSaint.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.BejeweledBoss
{
	public class BsSmashHandBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._smashHand == null)
			{
				this._smashHand = animator.GetComponent<BejeweledSmashHand>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._smashHand.IsRaised = false;
			this._smashHand.Disappear();
		}

		private BejeweledSmashHand _smashHand;
	}
}
