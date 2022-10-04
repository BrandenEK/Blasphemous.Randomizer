using System;
using Gameplay.GameControllers.Enemies.Swimmer;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.UndergroundSwimmer
{
	public class UndergroundSwimmerJumpBehaviour : StateMachineBehaviour
	{
		public Swimmer Swimmer { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Swimmer == null)
			{
				this.Swimmer = animator.GetComponentInParent<Swimmer>();
			}
			this._fireProjectile = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (this._fireProjectile || stateInfo.normalizedTime <= 0.1f)
			{
				return;
			}
			this._fireProjectile = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
		}

		private bool _fireProjectile;
	}
}
