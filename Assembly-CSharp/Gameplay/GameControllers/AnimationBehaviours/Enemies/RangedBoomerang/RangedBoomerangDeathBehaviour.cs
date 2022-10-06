using System;
using Gameplay.GameControllers.Enemies.RangedBoomerang;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.RangedBoomerang
{
	public class RangedBoomerangDeathBehaviour : StateMachineBehaviour
	{
		public RangedBoomerang RangedBoomerang { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.RangedBoomerang == null)
			{
				this.RangedBoomerang = animator.GetComponentInParent<RangedBoomerang>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Object.Destroy(this.RangedBoomerang.gameObject);
		}
	}
}
