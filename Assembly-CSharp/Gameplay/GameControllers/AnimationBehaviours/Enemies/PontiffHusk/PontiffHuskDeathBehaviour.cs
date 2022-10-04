using System;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.PontiffHusk
{
	public class PontiffHuskDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._pontiffHuskRanged == null)
			{
				this._pontiffHuskRanged = animator.GetComponentInParent<PontiffHuskRanged>();
			}
			if (this._pontiffHuskMelee == null)
			{
				this._pontiffHuskMelee = animator.GetComponentInParent<PontiffHuskMelee>();
			}
			this._destroy = false;
			if (this._pontiffHuskRanged != null)
			{
				PontiffHuskRanged pontiffHuskRanged = this._pontiffHuskRanged;
				if (pontiffHuskRanged.MotionLerper.IsLerping)
				{
					pontiffHuskRanged.MotionLerper.StopLerping();
				}
				pontiffHuskRanged.Audio.StopFloating();
				pontiffHuskRanged.Audio.StopChargeAttack();
				pontiffHuskRanged.Audio.StopAttack(true);
			}
			else
			{
				PontiffHuskMelee pontiffHuskMelee = this._pontiffHuskMelee;
				if (pontiffHuskMelee.AttackArea != null)
				{
					pontiffHuskMelee.AttackArea.WeaponCollider.enabled = false;
				}
				if (pontiffHuskMelee.MotionLerper.IsLerping)
				{
					pontiffHuskMelee.MotionLerper.StopLerping();
				}
				pontiffHuskMelee.Audio.StopFloating();
				pontiffHuskMelee.Audio.StopChargeAttack();
				pontiffHuskMelee.Audio.StopAttack(true);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.95f && !this._destroy)
			{
				this._destroy = true;
				if (this._pontiffHuskRanged != null)
				{
					this._pontiffHuskRanged.gameObject.SetActive(false);
				}
				if (this._pontiffHuskMelee != null)
				{
					this._pontiffHuskMelee.gameObject.SetActive(false);
				}
			}
		}

		private PontiffHuskRanged _pontiffHuskRanged;

		private PontiffHuskMelee _pontiffHuskMelee;

		private bool _destroy;
	}
}
