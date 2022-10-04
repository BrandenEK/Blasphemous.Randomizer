using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHusk
{
	public class PontiffHuskHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._PontiffHuskRanged == null)
			{
				this._PontiffHuskRanged = animator.GetComponentInParent<PontiffHuskRanged>();
			}
			if (!this._PontiffHuskRanged.Status.IsHurt)
			{
				this._PontiffHuskRanged.Status.IsHurt = true;
			}
			this._PontiffHuskRanged.Audio.StopChargeAttack();
			this._PontiffHuskRanged.Audio.StopAttack(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._PontiffHuskRanged.Status.IsHurt)
			{
				this._PontiffHuskRanged.Status.IsHurt = !this._PontiffHuskRanged.Status.IsHurt;
			}
		}

		private PontiffHuskRanged _PontiffHuskRanged;
	}
}
