using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Hurt
{
	public class GetUpStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = !this._acolyte.Status.IsHurt;
			}
			this._acolyte.EnableEnemyLayer(true);
		}

		private Acolyte _acolyte;
	}
}
