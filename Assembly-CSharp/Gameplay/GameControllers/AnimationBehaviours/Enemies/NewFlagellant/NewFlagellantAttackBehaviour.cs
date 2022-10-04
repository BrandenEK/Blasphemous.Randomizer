using System;
using Gameplay.GameControllers.Enemies.NewFlagellant;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.NewFlagellant
{
	public class NewFlagellantAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._NewFlagellant == null)
			{
				this._NewFlagellant = animator.GetComponentInParent<NewFlagellant>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._NewFlagellant == null)
			{
				return;
			}
			if (!this._NewFlagellant.IsAttacking)
			{
				this._NewFlagellant.IsAttacking = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._NewFlagellant == null)
			{
				return;
			}
			if (this._NewFlagellant.IsAttacking)
			{
				this._NewFlagellant.IsAttacking = !this._NewFlagellant.IsAttacking;
			}
			this._NewFlagellant.AnimatorInyector.AttackAnimationFinished();
		}

		private NewFlagellant _NewFlagellant;
	}
}
