using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossSpawnedAreaAttackEndBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.areaAttack == null)
			{
				this.areaAttack = animator.GetComponentInParent<BossSpawnedAreaAttack>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.areaAttack != null)
			{
				this.areaAttack.OnEndAnimationFinished();
			}
		}

		private BossSpawnedAreaAttack areaAttack;
	}
}
