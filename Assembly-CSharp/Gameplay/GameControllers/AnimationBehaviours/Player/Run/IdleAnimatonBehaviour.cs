using System;
using Framework.Managers;
using Gameplay.GameControllers.AnimationBehaviours.Player.Attack;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Run
{
	public class IdleAnimatonBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._startChargingAttackBehaviour == null)
			{
				this._startChargingAttackBehaviour = animator.GetBehaviour<StartChargingAttackBehaviour>();
			}
			this._penitent.Audio.StopLoadingChargedAttack();
			this._penitent.IsGrabbingLadder = false;
			this._penitent.Audio.AudioManager.StopSfx("PenitentLoadingChargedAttack", false);
			this._penitent.Dash.StandUpAfterDash = false;
			this._penitent.Dash.CrouchAfterDash = false;
			animator.SetBool(IdleAnimatonBehaviour.CrouchAttacking, false);
			if (this._penitent.IsFallingStunt)
			{
				this._penitent.IsFallingStunt = !this._penitent.IsFallingStunt;
			}
			if (this._penitent.IsChargingAttack)
			{
				this._penitent.IsChargingAttack = !this._penitent.IsChargingAttack;
			}
			this._penitent.Status.IsIdle = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._penitent)
			{
				this._penitent.Status.IsIdle = false;
			}
		}

		private Penitent _penitent;

		private StartChargingAttackBehaviour _startChargingAttackBehaviour;

		private static readonly int CrouchAttacking = Animator.StringToHash("CROUCH_ATTACKING");
	}
}
