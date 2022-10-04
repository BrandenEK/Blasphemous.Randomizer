using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Dash
{
	public class RunAfterDashBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._dashBehaviour == null)
			{
				this._dashBehaviour = animator.GetBehaviour<DashBehaviour>();
			}
			this.StopDash = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.PlatformCharacterInput.Rewired.GetButton(5))
			{
				animator.Play(RunAfterDashBehaviour.AttackRunningAnim);
			}
			if (stateInfo.normalizedTime >= 0.5f && !this.StopDash)
			{
				this.StopDash = true;
				this._penitent.Dash.StopCast();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.Dash.StandUpAfterDash)
			{
				this._penitent.Dash.StandUpAfterDash = false;
			}
			if (this.StopDash)
			{
				return;
			}
			this.StopDash = true;
			this._penitent.Dash.StopCast();
		}

		private Penitent _penitent;

		private DashBehaviour _dashBehaviour;

		public bool StopDash;

		private static readonly int AttackRunningAnim = Animator.StringToHash("Attack_Running");
	}
}
