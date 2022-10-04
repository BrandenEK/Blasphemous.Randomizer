using System;
using Gameplay.GameControllers.Enemies.WaxCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WaxCrawler
{
	public class WaxCrawlerHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._waxCrawler == null)
			{
				this._waxCrawler = animator.GetComponentInParent<WaxCrawler>();
			}
			this._waxCrawler.Status.IsHurt = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._waxCrawler.Status.IsHurt = false;
		}

		private WaxCrawler _waxCrawler;
	}
}
