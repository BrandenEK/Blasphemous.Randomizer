using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours
{
	public class ChargeAttackSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.speed = ((this._penitent.PenitentAttack.AttackSpeed <= 1f) ? 1f : this._penitent.PenitentAttack.AttackSpeed);
			if (this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = !this._penitent.IsCrouchAttacking;
				animator.SetBool("CROUCH_ATTACKING", false);
			}
			if (!this._penitent.IsChargingAttack)
			{
				this._penitent.IsChargingAttack = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.speed = 1f;
		}

		private Penitent _penitent;
	}
}
