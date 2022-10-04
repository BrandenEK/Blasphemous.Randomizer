using System;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellGhost
{
	public class BellGhostIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._bellGhost == null)
			{
				this._bellGhost = animator.GetComponentInParent<BellGhost>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._bellGhost)
			{
				return;
			}
			if (this._bellGhost.Behaviour.Asleep)
			{
				this._bellGhost.Audio.StopFloating();
				return;
			}
			if (this._bellGhost.IsAttacking)
			{
				this._bellGhost.Audio.StopFloating();
			}
			if (!this._bellGhost.Behaviour.IsAppear)
			{
				this._bellGhost.Audio.StopFloating();
			}
			if (this._bellGhost.Behaviour.IsAppear && !this._bellGhost.IsAttacking)
			{
				this._bellGhost.Audio.PlayFloating();
			}
		}

		private BellGhost _bellGhost;
	}
}
