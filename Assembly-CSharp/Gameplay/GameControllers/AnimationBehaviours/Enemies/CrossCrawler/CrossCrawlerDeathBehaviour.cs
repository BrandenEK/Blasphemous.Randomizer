using System;
using Gameplay.GameControllers.Enemies.CrossCrawler;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.CrossCrawler
{
	public class CrossCrawlerDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			CrossCrawler componentInParent = animator.GetComponentInParent<CrossCrawler>();
			if (componentInParent != null)
			{
				Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
