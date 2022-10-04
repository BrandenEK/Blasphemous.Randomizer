using System;
using Gameplay.GameControllers.Enemies.GoldenCorpse;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.GoldenCorpse
{
	public class GoldenCorpseAwakenBehaviour : StateMachineBehaviour
	{
		public GoldenCorpse GoldenCorpse { get; set; }

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.GoldenCorpse == null)
			{
				this.GoldenCorpse = animator.GetComponentInParent<GoldenCorpse>();
			}
			this.GoldenCorpse.Behaviour.OnAwakeAnimationFinished();
		}
	}
}
