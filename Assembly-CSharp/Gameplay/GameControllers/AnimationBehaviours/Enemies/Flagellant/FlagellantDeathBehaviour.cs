using System;
using Gameplay.GameControllers.Enemies.Flagellant;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant
{
	public class FlagellantDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.flagellant == null)
			{
				this.flagellant = animator.GetComponentInParent<Flagellant>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.5f)
			{
				EntityShadow componentInChildren = this.flagellant.GetComponentInChildren<EntityShadow>();
				if (componentInChildren != null)
				{
					componentInChildren.RemoveBlobShadow();
				}
			}
			if (stateInfo.normalizedTime >= 0.95f)
			{
				UnityEngine.Object.Destroy(this.flagellant.gameObject);
			}
		}

		private Flagellant flagellant;
	}
}
