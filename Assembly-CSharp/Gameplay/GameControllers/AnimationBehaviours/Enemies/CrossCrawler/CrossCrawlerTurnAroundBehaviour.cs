using System;
using Gameplay.GameControllers.Enemies.CrossCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.CrossCrawler
{
	public class CrossCrawlerTurnAroundBehaviour : StateMachineBehaviour
	{
		public CrossCrawler CrossCrawler { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.CrossCrawler == null)
			{
				this.CrossCrawler = animator.GetComponentInParent<CrossCrawler>();
			}
			this.CrossCrawler.Behaviour.TurningAround = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.CrossCrawler.Behaviour.TurningAround = false;
			this.CrossCrawler.SetOrientation(this.CrossCrawler.Status.Orientation, true, false);
		}
	}
}
