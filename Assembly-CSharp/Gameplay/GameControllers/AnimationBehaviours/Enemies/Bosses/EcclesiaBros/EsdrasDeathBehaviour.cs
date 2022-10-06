using System;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.EcclesiaBros
{
	public class EsdrasDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Esdras componentInParent = animator.GetComponentInParent<Esdras>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
