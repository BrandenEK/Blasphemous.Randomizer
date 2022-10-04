using System;
using Gameplay.GameControllers.Enemies.Jumper;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Jumper
{
	public class JumperAscendingAnimationBehaviour : StateMachineBehaviour
	{
		public Jumper Jumper { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Jumper == null)
			{
				this.Jumper = animator.GetComponentInParent<Jumper>();
			}
			if (this.Jumper.Attack.TargetInAttackArea)
			{
				this.Jumper.Attack.CurrentWeaponAttack();
			}
		}
	}
}
