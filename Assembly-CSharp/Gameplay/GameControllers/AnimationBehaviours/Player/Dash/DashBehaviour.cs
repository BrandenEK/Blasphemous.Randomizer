using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Dash
{
	public class DashBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.PenitentMoveAnimations.PlayDash();
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.ResetSwordSlashTriggers();
			this._cancelToParry = false;
			this._penitent.Dash.CrouchAfterDash = true;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime > 0.9f && this._penitent.Dash.IsUpperBlocked && !this._addExtraDash)
			{
				this.AddExtraDash();
			}
			if (this._penitent.Dash.IsUpperBlocked)
			{
				return;
			}
			if (this._penitent.PlatformCharacterInput.Rewired.GetButtonTimedPressDown(5, 0.1f) && stateInfo.normalizedTime < 1f && this.CastLungeAttack())
			{
				return;
			}
			if (this._penitent.PlatformCharacterInput.Rewired.GetButtonDown(38))
			{
				this._cancelToParry = true;
				this._penitent.Dash.StopCast();
				this._penitent.CancelEffect.PlayCancelEffect();
				this._penitent.DashDustGenerator.GetStopDashDust(0.1f);
				this._penitent.Parry.Cast();
				this._penitent.Dash.CrouchAfterDash = false;
				this._penitent.Animator.Play(this.ParryAnim);
			}
			bool button = this._penitent.PlatformCharacterInput.Rewired.GetButton(6);
			if (this._penitent.PlatformCharacterInput.Rewired.GetButtonUp(5) && !button && stateInfo.normalizedTime >= 0.1f)
			{
				this._penitent.Dash.StopCast();
				this._penitent.DashDustGenerator.GetStopDashDust(0.2f);
				this._penitent.Dash.CrouchAfterDash = false;
				bool flag = this._penitent.PlatformCharacterInput.Rewired.GetAxis(4) >= 0.75f;
				animator.Play((!flag) ? this._attackRunningAnimHash : this._upwardAttackAnimHash);
			}
			if (button && stateInfo.normalizedTime > 0.1f)
			{
				this._penitent.AnimatorInyector.IsJumpWhileDashing = true;
				this._penitent.Dash.StopCast();
				this._penitent.Dash.CrouchAfterDash = false;
				Core.Input.SetBlocker("PLAYER_LOGIC", false);
			}
			if (stateInfo.normalizedTime > 0.5f && stateInfo.normalizedTime < 1f && this._penitent.PlatformCharacterInput.Rewired.GetAxis(4) < -0.5f)
			{
				this.Crouch();
			}
			else if (stateInfo.normalizedTime > 0.5f && stateInfo.normalizedTime < 1f && Math.Abs(this._penitent.PlatformCharacterInput.Rewired.GetAxisRaw(0)) >= 0.1f)
			{
				if (!this._penitent.Dash.StandUpAfterDash)
				{
					this._penitent.Dash.StandUpAfterDash = true;
				}
				if (this._penitent.Status.IsGrounded)
				{
					this._penitent.DashDustGenerator.GetStopDashDust(0.1f);
				}
				this._penitent.Dash.CrouchAfterDash = false;
				animator.Play(this._runningAfterDashAnimHash);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._penitent.Status.Unattacable)
			{
				this._penitent.Status.Unattacable = false;
			}
			this._penitent.PlatformCharacterInput.ResetInputs();
			if (!this._cancelToParry)
			{
				Core.Input.SetBlocker("PLAYER_LOGIC", false);
			}
			this._addExtraDash = false;
			this._penitent.Audio.StopDashSound();
		}

		private void AddExtraDash()
		{
			if (this._addExtraDash)
			{
				return;
			}
			this._addExtraDash = true;
			Vector3 vector = (this._penitent.Status.Orientation != EntityOrientation.Right) ? (-this._penitent.transform.right) : this._penitent.transform.right;
			Vector3 position = this._penitent.transform.position;
			position.x += vector.x * 2f;
			this._penitent.transform.DOMoveX(position.x, 0.3f, false).SetEase(Ease.OutSine).OnUpdate(delegate
			{
				Core.Input.SetBlocker("PLAYER_LOGIC", true);
				if (!this._penitent.Dash.IsUpperBlocked || this._penitent.HasFlag("FRONT_BLOCKED"))
				{
					Core.Input.SetBlocker("PLAYER_LOGIC", false);
					DOTween.Kill(this._penitent.transform, false);
				}
			}).OnComplete(delegate
			{
				Core.Input.SetBlocker("PLAYER_LOGIC", false);
			});
		}

		private bool CastLungeAttack()
		{
			LungeAttack componentInChildren = this._penitent.GetComponentInChildren<LungeAttack>();
			if (!componentInChildren.IsAvailable)
			{
				return false;
			}
			this._penitent.Dash.CrouchAfterDash = false;
			this._penitent.Dash.StopCast();
			componentInChildren.Cast();
			componentInChildren.PlayLungeAnimByLevelReached();
			return true;
		}

		private void Crouch()
		{
			this._penitent.Dash.StopCast();
			this._penitent.Dash.StandUpAfterDash = false;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			if (this._penitent.Status.IsGrounded)
			{
				this._penitent.DashDustGenerator.GetStopDashDust(0f);
			}
			this._penitent.Animator.Play(this.CrouchDownAnim, 0, 0.5f);
		}

		private void ResetSwordSlashTriggers()
		{
			if (!this._penitent)
			{
				return;
			}
			SwordAnimatorInyector componentInChildren = this._penitent.GetComponentInChildren<SwordAnimatorInyector>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetParameters();
			}
		}

		private readonly int _attackRunningAnimHash = Animator.StringToHash("Attack_Running");

		private readonly int _upwardAttackAnimHash = Animator.StringToHash("GroundUpwardAttack");

		private readonly int _runningAfterDashAnimHash = Animator.StringToHash("Start_Run_After_Dash");

		protected readonly int LungAttackAnim = Animator.StringToHash("LungeAttack");

		protected readonly int CrouchDownAnim = Animator.StringToHash("Crouch Down");

		protected readonly int ParryAnim = Animator.StringToHash("ParryChance");

		private bool _cancelToParry;

		private bool _addExtraDash;

		private Penitent _penitent;
	}
}
