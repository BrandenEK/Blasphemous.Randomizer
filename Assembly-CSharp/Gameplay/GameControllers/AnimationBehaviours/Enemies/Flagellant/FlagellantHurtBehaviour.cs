using System;
using Gameplay.GameControllers.Enemies.Flagellant.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant
{
	public class FlagellantHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Entity>();
				this._behaviour = this._flagellant.GetComponentInChildren<FlagellantBehaviour>();
			}
			if (!this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = true;
			}
			if (this._flagellant.Status.Dead)
			{
				animator.Play(this._overThrow);
			}
			Hit lastHit = this._flagellant.EntityDamageArea.LastHit;
			if (lastHit.DamageType == DamageArea.DamageType.Heavy)
			{
				this._flagellant.HitDisplacement(lastHit.AttackingEntity.transform.position, lastHit.DamageType);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._behaviour.StopMovement();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				return;
			}
			if (this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = !this._flagellant.Status.IsHurt;
			}
		}

		private Entity _flagellant;

		private FlagellantBehaviour _behaviour;

		private readonly int _overThrow = Animator.StringToHash("Overthrow");
	}
}
