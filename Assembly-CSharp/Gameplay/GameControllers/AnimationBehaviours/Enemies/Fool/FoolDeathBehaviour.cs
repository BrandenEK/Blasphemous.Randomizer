using System;
using Gameplay.GameControllers.Enemies.Fool;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Fool
{
	public class FoolDeathBehaviour : StateMachineBehaviour
	{
		public Fool Fool { get; set; }

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.Fool == null)
			{
				this.Fool = animator.GetComponentInParent<Fool>();
			}
			Object.Destroy(this.Fool.gameObject);
		}
	}
}
