using System;
using Gameplay.GameControllers.Enemies.Firethrower;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Firethrower
{
	public class FirethrowerTurnAroundBehaviour : StateMachineBehaviour
	{
		public Firethrower Firethrower { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Firethrower == null)
			{
				this.Firethrower = animator.GetComponentInParent<Firethrower>();
			}
			this.Firethrower.Behaviour.TurningAround = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Firethrower.Behaviour.TurningAround = false;
			this.Firethrower.SetOrientation(this.Firethrower.Status.Orientation, true, false);
		}
	}
}
