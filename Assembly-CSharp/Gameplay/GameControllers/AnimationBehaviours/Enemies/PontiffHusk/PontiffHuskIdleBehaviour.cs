using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHusk
{
	public class PontiffHuskIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._PontiffHuskRanged == null)
			{
				this._PontiffHuskRanged = animator.GetComponentInParent<PontiffHuskRanged>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._PontiffHuskRanged)
			{
				return;
			}
			if (this._PontiffHuskRanged.Behaviour.Asleep)
			{
				this._PontiffHuskRanged.Audio.StopFloating();
				return;
			}
			if (this._PontiffHuskRanged.IsAttacking)
			{
				this._PontiffHuskRanged.Audio.StopFloating();
			}
			if (!this._PontiffHuskRanged.Behaviour.IsAppear)
			{
				this._PontiffHuskRanged.Audio.StopFloating();
			}
			if (!this._PontiffHuskRanged.Behaviour.IsAppear || !this._PontiffHuskRanged.IsAttacking)
			{
			}
		}

		private PontiffHuskRanged _PontiffHuskRanged;
	}
}
