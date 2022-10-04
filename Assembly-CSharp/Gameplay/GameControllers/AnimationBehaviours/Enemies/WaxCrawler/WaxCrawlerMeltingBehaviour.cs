using System;
using Gameplay.GameControllers.Enemies.WaxCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.WaxCrawler
{
	public class WaxCrawlerMeltingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._waxCrawler == null)
			{
				this._waxCrawler = animator.GetComponentInParent<WaxCrawler>();
			}
			this._goToOrigin = false;
			this._waxCrawler.Behaviour.Melting = true;
			bool flag = animator.GetCurrentAnimatorStateInfo(0).IsName("Appear");
			this._waxCrawler.DamageArea.DamageAreaCollider.enabled = flag;
			this._waxCrawler.CanBeAttacked = flag;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.95f && animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear") && !this._goToOrigin)
			{
				this._waxCrawler.Behaviour.Asleep();
				this._goToOrigin = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._waxCrawler == null)
			{
				return;
			}
			this._waxCrawler.Behaviour.Melting = false;
		}

		private bool _goToOrigin;

		private WaxCrawler _waxCrawler;
	}
}
