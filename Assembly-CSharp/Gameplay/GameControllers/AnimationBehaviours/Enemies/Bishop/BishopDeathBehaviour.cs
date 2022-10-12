using System;
using Gameplay.GameControllers.Enemies.Bishop;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bishop
{
	public class BishopDeathBehaviour : StateMachineBehaviour
	{
		public Bishop Bishop { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Bishop == null)
			{
				this.Bishop = animator.GetComponentInParent<Bishop>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			UnityEngine.Object.Destroy(this.Bishop.gameObject);
		}
	}
}
