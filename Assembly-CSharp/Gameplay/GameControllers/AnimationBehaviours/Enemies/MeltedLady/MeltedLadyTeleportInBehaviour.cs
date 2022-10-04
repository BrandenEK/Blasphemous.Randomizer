using System;
using Gameplay.GameControllers.Enemies.MeltedLady;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.MeltedLady
{
	public class MeltedLadyTeleportInBehaviour : StateMachineBehaviour
	{
		public FloatingLady MeltedLady { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.MeltedLady == null)
			{
				this.MeltedLady = animator.GetComponentInParent<FloatingLady>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.MeltedLady.IsAttacking = !this.MeltedLady.Behaviour.IsInOrigin;
			this.MeltedLady.DamageArea.DamageAreaCollider.enabled = true;
			this.MeltedLady.DamageByContact = true;
			if (!this.MeltedLady.Status.IsHurt)
			{
				return;
			}
			this.MeltedLady.Status.IsHurt = false;
			this.MeltedLady.IsAttacking = true;
			this.MeltedLady.Behaviour.ResetAttackCounter();
		}
	}
}
