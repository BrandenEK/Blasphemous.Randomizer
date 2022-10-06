using System;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.EcclesiaBros
{
	public class PerpetuaDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Perpetua componentInParent = animator.GetComponentInParent<Perpetua>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
