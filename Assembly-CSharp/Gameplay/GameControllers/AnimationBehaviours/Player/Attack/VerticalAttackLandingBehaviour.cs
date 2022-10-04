using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class VerticalAttackLandingBehaviour : StateMachineBehaviour
	{
		private Penitent Penitent { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Penitent == null)
			{
				this.Penitent = Core.Logic.Penitent;
			}
			this.Penitent.GrabLadder.EnableClimbLadderAbility(false);
			this.Penitent.VerticalAttack.SetAttackAreaDimensionsBySkill();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime <= 0.5f)
			{
				return;
			}
			this.Penitent.PlatformCharacterInput.ResetActions();
			this.Penitent.PlatformCharacterInput.ResetInputs();
			if (this.Penitent.PlatformCharacterInput.Rewired.GetButton(7))
			{
				this.Penitent.CancelEffect.PlayCancelEffect();
				this.Penitent.Parry.StopCast();
				this.Penitent.Animator.SetTrigger("DASH");
				this.Penitent.Animator.ResetTrigger("JUMP");
				this.Penitent.Animator.SetBool("DASHING", true);
				this.Penitent.Dash.Cast();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Penitent.GrabLadder.EnableClimbLadderAbility(true);
			this.Penitent.VerticalAttack.SetDefaultAttackAreaDimensions();
		}
	}
}
