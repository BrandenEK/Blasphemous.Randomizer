using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Movement
{
	public class PietyIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			if (this._pietyMonster.PietyBehaviour.StatusChange)
			{
				this._pietyMonster.PietyBehaviour.StatusChange = false;
			}
		}

		private PietyMonster _pietyMonster;
	}
}
