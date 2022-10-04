using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Hurt
{
	public class AcolyteHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			Hit lastHit = this._acolyte.EntityDamageArea.LastHit;
			if (lastHit.DamageType != DamageArea.DamageType.Heavy)
			{
				return;
			}
			if (!this._acolyte.EnemyFloorChecker().IsSideBlocked)
			{
				this._acolyte.HitDisplacement(lastHit.AttackingEntity.transform.position, lastHit.DamageType);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = true;
			}
			if (this._acolyte.EntityAttack.IsWeaponBlowingUp)
			{
				this._acolyte.EntityAttack.IsWeaponBlowingUp = false;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = !this._acolyte.Status.IsHurt;
			}
		}

		private Acolyte _acolyte;
	}
}
