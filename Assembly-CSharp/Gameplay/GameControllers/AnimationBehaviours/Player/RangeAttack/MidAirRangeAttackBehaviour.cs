using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.RangeAttack
{
	public class MidAirRangeAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._rangeAttack = this._penitent.GetComponentInChildren<RangeAttack>();
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			Vector3 velocity = this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity;
			this._accumulatedVelocity = new Vector3(velocity.x, 0f, velocity.z);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Gravity = Vector3.zero;
			this._penitent.PenitentAttack.IsRangeAttacking = true;
			this._penitent.AnimatorInyector.ResetStuntByFall();
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity = Vector3.zero;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Gravity = new Vector3(0f, -9.8f, 0f);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity = this._accumulatedVelocity;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			this._penitent.PenitentAttack.IsRangeAttacking = false;
			if (this._rangeAttack != null)
			{
				this._rangeAttack.StopCastRangeAttack();
			}
		}

		private Penitent _penitent;

		private RangeAttack _rangeAttack;

		private Vector3 _accumulatedVelocity;
	}
}
