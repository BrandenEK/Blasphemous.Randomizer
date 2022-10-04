using System;
using Gameplay.GameControllers.Bosses.Crisanta;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Crisanta
{
	public class CrisantaGuardBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Crisanta componentInParent = animator.GetComponentInParent<Crisanta>();
			componentInParent.Behaviour.OnEnterGuard();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Crisanta componentInParent = animator.GetComponentInParent<Crisanta>();
			componentInParent.Behaviour.OnExitGuard();
		}
	}
}
