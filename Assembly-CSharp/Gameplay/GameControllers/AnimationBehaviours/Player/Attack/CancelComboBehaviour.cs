using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class CancelComboBehaviour : StateMachineBehaviour
	{
		private Penitent Penitent { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Penitent == null)
			{
				this.Penitent = Core.Logic.Penitent;
			}
			this.RequestParry();
			this.RequestDash();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this.RequestParry();
			this.RequestDash();
		}

		private void RequestParry()
		{
			if (this.Penitent.PlatformCharacterInput.Rewired.GetButtonDown(38))
			{
				this.Penitent.CancelEffect.PlayCancelEffect();
				this.Penitent.Parry.StopCast();
				this.Penitent.Animator.SetBool("PARRY", true);
				this.Penitent.Animator.Play(this._parryAnimation);
			}
		}

		private void RequestDash()
		{
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

		private readonly int _parryAnimation = Animator.StringToHash("ParryChance");
	}
}
