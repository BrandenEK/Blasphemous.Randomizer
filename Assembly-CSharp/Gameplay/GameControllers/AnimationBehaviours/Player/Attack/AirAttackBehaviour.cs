using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class AirAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.CurrentOutputDamage = this.damageAmount;
			animator.SetBool("AIR_ATTACKING", true);
			this._penitent.OnAirAttackBehaviour_OnEnter(this);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.6f)
			{
				this._penitent.EntityAttack.IsWeaponBlowingUp = true;
			}
			else
			{
				this._penitent.EntityAttack.IsWeaponBlowingUp = false;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.EntityAttack.IsWeaponBlowingUp = false;
			this._penitent.CurrentOutputDamage = 0;
			animator.SetBool("AIR_ATTACKING", false);
		}

		private Penitent _penitent;

		[Tooltip("The damage amount done by the penitent in this attack")]
		public int damageAmount;
	}
}
