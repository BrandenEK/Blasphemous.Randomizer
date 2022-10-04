using System;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class RangeAttackExplosionBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._rangeAttackExplosion == null)
			{
				this._rangeAttackExplosion = animator.GetComponent<RangeAttackExplosion>();
			}
			this.attack = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if ((double)stateInfo.length <= 0.5 || this.attack)
			{
				return;
			}
			this.attack = true;
			this._rangeAttackExplosion.Attack();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._rangeAttackExplosion.Dispose();
		}

		private RangeAttackExplosion _rangeAttackExplosion;

		private bool attack;
	}
}
