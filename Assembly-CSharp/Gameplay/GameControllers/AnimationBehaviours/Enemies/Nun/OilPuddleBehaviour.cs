using System;
using Gameplay.GameControllers.Enemies.Nun.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Nun
{
	public class OilPuddleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._oilPuddle == null)
			{
				this._oilPuddle = animator.GetComponentInParent<OilPuddle>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._oilPuddle.Dissappear();
		}

		private OilPuddle _oilPuddle;
	}
}
