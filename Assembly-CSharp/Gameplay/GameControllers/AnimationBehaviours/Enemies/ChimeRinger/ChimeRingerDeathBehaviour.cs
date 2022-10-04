using System;
using Gameplay.GameControllers.Enemies.ChimeRinger;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.ChimeRinger
{
	public class ChimeRingerDeathBehaviour : StateMachineBehaviour
	{
		public ChimeRinger ChimeRinger { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.ChimeRinger == null)
			{
				this.ChimeRinger = animator.GetComponentInParent<ChimeRinger>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			UnityEngine.Object.Destroy(this.ChimeRinger.gameObject);
		}
	}
}
