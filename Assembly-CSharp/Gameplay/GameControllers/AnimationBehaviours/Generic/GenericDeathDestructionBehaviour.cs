using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Generic
{
	public class GenericDeathDestructionBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this.root = animator.transform.root.gameObject;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Object.Destroy(this.root);
		}

		public GameObject root;
	}
}
