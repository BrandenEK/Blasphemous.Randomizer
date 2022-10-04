using System;
using Gameplay.GameControllers.Enemies.Firethrower;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Firethrower
{
	public class FirethrowerAttackEndBehaviour : StateMachineBehaviour
	{
		public Firethrower Firethrower { get; set; }

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.Firethrower == null)
			{
				this.Firethrower = animator.GetComponentInParent<Firethrower>();
			}
			this.Firethrower.Behaviour.OnAttackAnimationFinished();
		}
	}
}
