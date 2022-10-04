using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Nun;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Nun
{
	public class NunAttackBehaviour : StateMachineBehaviour
	{
		public Nun Nun { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Nun == null)
			{
				this.Nun = animator.GetComponentInParent<Nun>();
			}
			this.Nun.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Nun.IsAttacking = false;
			this.Nun.Behaviour.LookAtTarget(Core.Logic.Penitent.transform.position);
			this.Nun.Audio.StopAttack();
		}
	}
}
