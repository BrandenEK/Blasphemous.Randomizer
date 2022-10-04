using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.CrossCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.CrossCrawler
{
	public class CrossCrawlerAttackBehaviour : StateMachineBehaviour
	{
		public CrossCrawler CrossCrawler { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.CrossCrawler == null)
			{
				this.CrossCrawler = animator.GetComponentInParent<CrossCrawler>();
			}
			this.CrossCrawler.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.CrossCrawler.IsAttacking = false;
			this.CrossCrawler.Behaviour.LookAtTarget(Core.Logic.Penitent.transform.position);
		}
	}
}
