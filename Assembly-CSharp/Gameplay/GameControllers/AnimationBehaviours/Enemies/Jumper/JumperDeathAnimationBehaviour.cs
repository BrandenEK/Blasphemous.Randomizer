using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Jumper
{
	public class JumperDeathAnimationBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.GetComponentInParent<Enemy>().DamageByContact = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Enemy componentInParent = animator.GetComponentInParent<Enemy>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
