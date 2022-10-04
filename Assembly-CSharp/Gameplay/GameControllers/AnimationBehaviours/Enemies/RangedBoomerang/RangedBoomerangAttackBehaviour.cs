using System;
using Gameplay.GameControllers.Enemies.RangedBoomerang;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.RangedBoomerang
{
	public class RangedBoomerangAttackBehaviour : StateMachineBehaviour
	{
		public RangedBoomerang RangedBoomerang { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.RangedBoomerang == null)
			{
				this.RangedBoomerang = animator.GetComponentInParent<RangedBoomerang>();
			}
			this.RangedBoomerang.IsAttacking = true;
		}
	}
}
