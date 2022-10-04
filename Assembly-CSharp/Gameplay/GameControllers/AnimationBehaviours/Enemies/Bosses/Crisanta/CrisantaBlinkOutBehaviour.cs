using System;
using Gameplay.GameControllers.Bosses.Crisanta;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Crisanta
{
	public class CrisantaBlinkOutBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			CrisantaBehaviour componentInParent = animator.GetComponentInParent<CrisantaBehaviour>();
			componentInParent.OnBlinkOut();
		}
	}
}
