using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHusk
{
	public class PontiffHuskDashingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._pontiffHusk == null)
			{
				this._pontiffHusk = animator.GetComponentInParent<PontiffHuskMelee>();
			}
			this.time = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.time += Time.deltaTime;
			if (this.time > this.MaxDashTime)
			{
				this._pontiffHusk.Behaviour.AnimatorInyector.StopAttack();
				this._pontiffHusk.Behaviour.Disappear(1f);
				this.time = 0f;
			}
		}

		public float MaxDashTime = 2f;

		private float time;

		private PontiffHuskMelee _pontiffHusk;
	}
}
