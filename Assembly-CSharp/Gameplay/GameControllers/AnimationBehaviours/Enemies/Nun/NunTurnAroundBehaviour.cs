using System;
using Gameplay.GameControllers.Enemies.Nun;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Nun
{
	public class NunTurnAroundBehaviour : StateMachineBehaviour
	{
		public Nun Nun { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Nun == null)
			{
				this.Nun = animator.GetComponentInParent<Nun>();
			}
			this.Nun.Behaviour.TurningAround = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Nun.Behaviour.TurningAround = false;
			this.Nun.SetOrientation(this.Nun.Status.Orientation, true, false);
			this.Nun.Audio.StopTurnAround();
		}
	}
}
