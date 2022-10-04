using System;
using Gameplay.GameControllers.Bosses.Crisanta;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Crisanta
{
	public class CrisantaMeleeAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Crisanta componentInParent = animator.GetComponentInParent<Crisanta>();
			componentInParent.Behaviour.OnExitMeleeAttack();
		}
	}
}
