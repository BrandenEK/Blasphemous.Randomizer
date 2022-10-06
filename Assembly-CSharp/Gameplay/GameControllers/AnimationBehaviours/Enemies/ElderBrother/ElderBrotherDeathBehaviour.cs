using System;
using Gameplay.GameControllers.Bosses.ElderBrother;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ElderBrother
{
	public class ElderBrotherDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			ElderBrother componentInParent = animator.GetComponentInParent<ElderBrother>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
