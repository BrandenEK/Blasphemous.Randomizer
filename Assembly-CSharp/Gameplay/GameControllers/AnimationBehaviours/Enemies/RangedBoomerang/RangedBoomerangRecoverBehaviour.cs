using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.RangedBoomerang;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.RangedBoomerang
{
	public class RangedBoomerangRecoverBehaviour : StateMachineBehaviour
	{
		public RangedBoomerang RangedBoomerang { get; set; }

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this.RangedBoomerang == null)
			{
				this.RangedBoomerang = animator.GetComponentInParent<RangedBoomerang>();
			}
			this.RangedBoomerang.IsAttacking = false;
			this.RangedBoomerang.Behaviour.LookAtTarget(Core.Logic.Penitent.transform.position);
		}
	}
}
