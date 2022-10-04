using System;
using Gameplay.GameControllers.Enemies.ReekLeader;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ReekLeader
{
	public class ReekLeaderDeathBehaviour : StateMachineBehaviour
	{
		public ReekLeader ReekLeader { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ReekLeader == null)
			{
				this.ReekLeader = animator.GetComponentInParent<ReekLeader>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			UnityEngine.Object.Destroy(this.ReekLeader.gameObject);
		}
	}
}
