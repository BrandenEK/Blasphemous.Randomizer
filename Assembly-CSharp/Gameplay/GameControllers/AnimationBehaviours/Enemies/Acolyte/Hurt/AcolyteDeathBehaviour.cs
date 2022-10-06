using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Hurt
{
	public class AcolyteDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			animator.speed = 1f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime < 0.95f)
			{
				return;
			}
			EntityShadow componentInChildren = this._acolyte.GetComponentInChildren<EntityShadow>();
			if (componentInChildren != null)
			{
				componentInChildren.RemoveBlobShadow();
			}
			Object.Destroy(this._acolyte.gameObject);
		}

		private Acolyte _acolyte;
	}
}
