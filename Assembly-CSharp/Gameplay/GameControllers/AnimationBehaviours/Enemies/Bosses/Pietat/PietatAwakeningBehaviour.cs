using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat
{
	public class PietatAwakeningBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._pietat == null)
			{
				this._pietat = animator.GetComponent<Enemy>();
			}
			animator.speed = 0f;
		}

		private Enemy _pietat;
	}
}
