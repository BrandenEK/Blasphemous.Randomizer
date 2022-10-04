using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Spawn;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Player
{
	public class CherubRespawnAnimationBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._cherubRespawn == null)
			{
				this._cherubRespawn = animator.GetComponent<CherubRespawn>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._cherubRespawn.Dispose();
		}

		private CherubRespawn _cherubRespawn;
	}
}
