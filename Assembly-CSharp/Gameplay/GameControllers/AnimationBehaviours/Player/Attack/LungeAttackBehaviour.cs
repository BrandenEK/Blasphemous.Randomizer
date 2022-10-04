using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Animations;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class LungeAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._lungeAttack == null)
			{
				Penitent penitent = Core.Logic.Penitent;
				this._lungeAttack = penitent.GetComponentInChildren<LungeAttack>();
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.SetAttackAreaDimension();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._lungeAttack.StopCast();
			Core.Logic.Penitent.PenitentMoveAnimations.EnabledGhostTrail(AttackAnimationsEvents.Activation.False);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
		}

		public void SetAttackAreaDimension()
		{
			Core.Logic.Penitent.AttackArea.SetSize(this.AttackAreaDimensions.AttackAreaSize);
			Core.Logic.Penitent.AttackArea.SetOffset(this.AttackAreaDimensions.AttackAreaOffset);
		}

		private LungeAttack _lungeAttack;

		public VerticalAttack.AttackAreaDimensions AttackAreaDimensions;
	}
}
