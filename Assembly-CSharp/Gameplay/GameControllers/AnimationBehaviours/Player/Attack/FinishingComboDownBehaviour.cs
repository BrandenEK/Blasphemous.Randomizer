using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class FinishingComboDownBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.SetAttackAreaDimension();
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
