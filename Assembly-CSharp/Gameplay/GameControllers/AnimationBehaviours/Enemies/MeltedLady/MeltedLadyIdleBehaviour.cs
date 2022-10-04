using System;
using Gameplay.GameControllers.Enemies.MeltedLady;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.MeltedLady
{
	public class MeltedLadyIdleBehaviour : StateMachineBehaviour
	{
		public MeltedLady MeltedLady { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.MeltedLady == null)
			{
				this.MeltedLady = animator.GetComponentInParent<MeltedLady>();
			}
			if (this.MeltedLady.IsAttacking && this.MeltedLady.Behaviour.CurrentAttackAmount > 0)
			{
				this.MeltedLady.Behaviour.CurrentAttackAmount--;
				this.MeltedLady.AnimatorInyector.Attack();
			}
			else
			{
				this.MeltedLady.Behaviour.CurrentAttackAmount = this.MeltedLady.Behaviour.AttackAmount;
				this.MeltedLady.Behaviour.CanTeleport = true;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
		}
	}
}
