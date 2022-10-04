using System;
using Gameplay.GameControllers.Enemies.Nun;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Nun
{
	public class NunDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Nun componentInParent = animator.GetComponentInParent<Nun>();
			if (componentInParent != null)
			{
				UnityEngine.Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
