using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Crouch
{
	public class CrouchAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.CurrentOutputDamage = this.damageAmount;
			this._penitent.AnimatorInyector.RaiseAttackEvent();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.speed = ((this._penitent.PenitentAttack.AttackSpeed <= 1f) ? 1f : this._penitent.PenitentAttack.AttackSpeed);
			if (!this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = true;
			}
			if (!this._penitent.PlatformCharacterInput.IsAttacking)
			{
				this._penitent.PlatformCharacterInput.IsAttacking = true;
			}
			if (stateInfo.normalizedTime >= 0.35f && stateInfo.normalizedTime <= 0.7f)
			{
				if (!this._penitent.EntityAttack.IsWeaponBlowingUp)
				{
					this._penitent.EntityAttack.IsWeaponBlowingUp = true;
				}
			}
			else if (this._penitent.EntityAttack.IsWeaponBlowingUp)
			{
				this._penitent.EntityAttack.IsWeaponBlowingUp = false;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = !this._penitent.IsCrouchAttacking;
			}
			if (this._penitent.PlatformCharacterInput.IsAttacking)
			{
				this._penitent.PlatformCharacterInput.IsAttacking = false;
			}
			animator.speed = 1f;
			this._penitent.EntityAttack.IsWeaponBlowingUp = false;
		}

		private Penitent _penitent;

		[Tooltip("The damage amount done by the penitent in this attack")]
		public int damageAmount;
	}
}
