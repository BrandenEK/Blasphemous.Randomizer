using System;
using Gameplay.GameControllers.Enemies.Fool;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Fool
{
	public class FoolTurnAroundBehaviour : StateMachineBehaviour
	{
		public Fool Fool { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Fool == null)
			{
				this.Fool = animator.GetComponentInParent<Fool>();
			}
			this.Fool.Behaviour.TurningAround = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Fool.SetOrientation(this.Fool.Status.Orientation, true, false);
			this.Fool.Behaviour.TurningAround = false;
		}
	}
}
