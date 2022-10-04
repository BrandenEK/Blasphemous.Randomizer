using System;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class DefaultSwordSlashBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.AnimatorInyector == null)
			{
				this.AnimatorInyector = animator.GetComponent<SwordAnimatorInyector>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.AnimatorInyector.ResetParameters();
		}

		private SwordAnimatorInyector AnimatorInyector;
	}
}
