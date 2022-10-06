using System;
using Gameplay.GameControllers.Enemies.Menina;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Menina
{
	public class MeninaDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._menina == null)
			{
				this._menina = animator.GetComponentInParent<Menina>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Object.Destroy(this._menina.gameObject);
		}

		private Menina _menina;
	}
}
