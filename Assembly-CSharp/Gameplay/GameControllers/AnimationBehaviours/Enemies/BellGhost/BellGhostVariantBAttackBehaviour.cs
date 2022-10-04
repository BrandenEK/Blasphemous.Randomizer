using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellGhost
{
	public class BellGhostVariantBAttackBehaviour : StateMachineBehaviour
	{
		public BellGhost BellGhost { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.BellGhost == null)
			{
				this.BellGhost = animator.GetComponentInParent<BellGhost>();
			}
			this.BellGhost.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.BellGhost.IsAttacking = false;
			this.BellGhost.Behaviour.LookAtTarget(Core.Logic.Penitent.transform.position);
		}
	}
}
