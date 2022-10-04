using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class FinishingComboUpBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			Core.Logic.Penitent.PenitentAttack.IsAttacking = false;
			this.SetAttackAreaDimension();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
		}

		public void SetAttackAreaDimension()
		{
			Core.Logic.Penitent.AttackArea.SetSize(this.AttackAreaDimensions.AttackAreaSize);
			Core.Logic.Penitent.AttackArea.SetOffset(this.AttackAreaDimensions.AttackAreaOffset);
		}

		public VerticalAttack.AttackAreaDimensions AttackAreaDimensions;
	}
}
