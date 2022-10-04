using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Hurt
{
	public class AirHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.canClimbCliffLede = false;
			this._penitent.cliffLedeClimbingStarted = false;
			if (animator.GetBool("STICK_ON_WALL"))
			{
				this._penitent.GetComponentInChildren<WallJump>().enabled = false;
				this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.Status.Dead)
			{
				this._penitent.IsDeadInAir = true;
			}
		}

		private Penitent _penitent;
	}
}
