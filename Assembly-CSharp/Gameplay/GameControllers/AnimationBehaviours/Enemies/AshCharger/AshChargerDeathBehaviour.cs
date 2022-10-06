using System;
using Gameplay.GameControllers.Enemies.AshCharger;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.AshCharger
{
	public class AshChargerDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			AshCharger componentInParent = animator.GetComponentInParent<AshCharger>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
