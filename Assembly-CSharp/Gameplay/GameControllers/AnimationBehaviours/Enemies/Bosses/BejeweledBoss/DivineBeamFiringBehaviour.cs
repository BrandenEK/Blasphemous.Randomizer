using System;
using Gameplay.GameControllers.Bosses.BejeweledSaint.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.BejeweledBoss
{
	public class DivineBeamFiringBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._divineBeam == null)
			{
				this._divineBeam = animator.GetComponent<BejeweledDivineBeam>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._divineBeam.Dispose();
		}

		private BejeweledDivineBeam _divineBeam;
	}
}
