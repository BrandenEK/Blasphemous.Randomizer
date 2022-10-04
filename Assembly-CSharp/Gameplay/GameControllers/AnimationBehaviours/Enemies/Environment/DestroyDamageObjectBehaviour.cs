using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Environment
{
	public class DestroyDamageObjectBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.gameObject.SetActive(false);
		}
	}
}
