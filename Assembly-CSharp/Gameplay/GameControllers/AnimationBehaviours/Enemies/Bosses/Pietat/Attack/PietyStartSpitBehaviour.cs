using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Attack
{
	public class PietyStartSpitBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._piety == null)
			{
				this._piety = animator.GetComponentInParent<PietyMonster>();
			}
			this._piety.PietyBehaviour.SpitAttack.RefillSpitPositions();
		}

		private PietyMonster _piety;
	}
}
