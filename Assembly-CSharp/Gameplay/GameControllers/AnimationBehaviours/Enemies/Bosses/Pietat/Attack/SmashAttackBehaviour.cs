using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Attack
{
	public class SmashAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			if (!this._pietyMonster.PietyBehaviour.StatusChange)
			{
				this._pietyMonster.PietyBehaviour.StatusChange = true;
			}
			this._pietyMonster.BodyBarrier.DisableCollider();
			this._pietyMonster.AnimatorInyector.ResetAttacks();
		}

		private PietyMonster _pietyMonster;
	}
}
