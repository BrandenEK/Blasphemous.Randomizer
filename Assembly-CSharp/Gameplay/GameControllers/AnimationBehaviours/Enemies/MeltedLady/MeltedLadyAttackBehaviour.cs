using System;
using Gameplay.GameControllers.Enemies.MeltedLady;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.MeltedLady
{
	public class MeltedLadyAttackBehaviour : StateMachineBehaviour
	{
		public FloatingLady MeltedLady { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.MeltedLady == null)
			{
				this.MeltedLady = animator.GetComponentInParent<FloatingLady>();
			}
			this.MeltedLady.Behaviour.LookAtTarget(this.MeltedLady.Target.transform.position);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this.MeltedLady.Behaviour.TeleportCooldownLapse = 0f;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.MeltedLady.Behaviour.LookAtTarget(this.MeltedLady.Target.transform.position);
		}
	}
}
