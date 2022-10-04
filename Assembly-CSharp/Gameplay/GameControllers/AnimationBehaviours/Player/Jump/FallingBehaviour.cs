using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class FallingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.cliffLedeClimbingStarted = false;
			this._penitent.IsClimbingCliffLede = false;
			this._penitent.Physics.EnableColliders(true);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.startedJumpOff = false;
			this._penitent.IsJumpingOff = false;
			this._penitent.PlatformCharacterInput.CancelPlatformDropDown();
		}

		private Penitent _penitent;
	}
}
