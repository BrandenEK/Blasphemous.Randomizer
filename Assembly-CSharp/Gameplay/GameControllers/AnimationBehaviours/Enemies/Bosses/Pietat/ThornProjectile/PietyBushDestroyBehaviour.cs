using System;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.ThornProjectile
{
	public class PietyBushDestroyBehaviour : StateMachineBehaviour
	{
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.95f)
			{
				Object.Destroy(animator.gameObject);
			}
		}
	}
}
