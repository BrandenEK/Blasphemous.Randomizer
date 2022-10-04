using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.GuardSlide;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Player
{
	public class GuardSlideImpactAnimationBehaviour : StateMachineBehaviour
	{
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			animator.transform.position = Core.Logic.Penitent.transform.position;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			GuardSlideImpact component = animator.GetComponent<GuardSlideImpact>();
			if (component != null)
			{
				component.Dispose();
			}
		}
	}
}
