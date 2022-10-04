using System;
using Gameplay.GameControllers.Enemies.MeltedLady;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.MeltedLady
{
	public class InkLadyIdleBehaviour : StateMachineBehaviour
	{
		public InkLady InkLady { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.InkLady == null)
			{
				this.InkLady = animator.GetComponentInParent<InkLady>();
			}
			if (this.InkLady.IsAttacking && this.InkLady.Behaviour.CurrentAttackAmount > 0)
			{
				this.InkLady.Behaviour.CurrentAttackAmount--;
				this.InkLady.AnimatorInyector.Attack();
			}
			else
			{
				this.InkLady.Behaviour.CurrentAttackAmount = this.InkLady.Behaviour.AttackAmount;
				this.InkLady.Behaviour.CanTeleport = true;
			}
		}
	}
}
