using System;
using Gameplay.GameControllers.Enemies.WaxCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WaxCrawler
{
	public class WaxCrawlerDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._waxCrawler == null)
			{
				this._waxCrawler = animator.GetComponentInParent<WaxCrawler>();
			}
			this._waxCrawler.Status.IsHurt = true;
			this._isDestroy = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime >= 0.9f && !this._isDestroy)
			{
				this._isDestroy = true;
				Object.Destroy(this._waxCrawler.gameObject);
			}
		}

		private bool _isDestroy;

		private WaxCrawler _waxCrawler;
	}
}
