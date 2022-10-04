using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Traps
{
	public class FireExplosionBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.gameObject.SetActive(false);
		}
	}
}
