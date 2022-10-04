using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Traps
{
	public class SnakeLightningExplosionBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.transform.parent.gameObject.SetActive(false);
		}
	}
}
