using System;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellGhost
{
	public class BellGhostHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._bellGhost == null)
			{
				this._bellGhost = animator.GetComponentInParent<BellGhost>();
			}
			if (!this._bellGhost.Status.IsHurt)
			{
				this._bellGhost.Status.IsHurt = true;
			}
			this._bellGhost.Audio.StopChargeAttack();
			this._bellGhost.Audio.StopAttack(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._bellGhost.Status.IsHurt)
			{
				this._bellGhost.Status.IsHurt = !this._bellGhost.Status.IsHurt;
			}
			this._bellGhost.Behaviour.IsBackToOrigin = false;
		}

		private BellGhost _bellGhost;
	}
}
