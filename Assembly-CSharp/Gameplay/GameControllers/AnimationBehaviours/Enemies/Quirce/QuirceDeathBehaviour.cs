using System;
using Gameplay.GameControllers.Bosses.Quirce;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Quirce
{
	public class QuirceDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Quirce componentInParent = animator.GetComponentInParent<Quirce>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
