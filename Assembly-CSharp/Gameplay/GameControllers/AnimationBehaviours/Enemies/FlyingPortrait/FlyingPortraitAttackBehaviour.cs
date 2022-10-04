using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.FlyingPortrait
{
	public class FlyingPortraitAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._flyingPortrait == null)
			{
				this._flyingPortrait = animator.GetComponentInParent<Enemy>();
			}
			this._flyingPortrait.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._flyingPortrait.IsAttacking = false;
		}

		private Enemy _flyingPortrait;
	}
}
