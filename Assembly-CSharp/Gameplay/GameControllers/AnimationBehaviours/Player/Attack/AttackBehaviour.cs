using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class AttackBehaviour : StateMachineBehaviour
	{
		private bool IsDemakeMode
		{
			get
			{
				return Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			}
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._penitentAttackArea = this._penitent.PenitentAttack.CurrentPenitentWeapon.AttackAreas[0];
			}
			this._penitent.CurrentOutputDamage = this.damageAmount;
			if (this._swordSparkSpawner == null)
			{
				this._swordSparkSpawner = Core.Logic.Penitent.SwordSparkSpawner;
			}
			if (this.swordSpark != null)
			{
				this._swordSparkSpawner.CurrentSwordSparkSpawningType = this.swordSpark.sparkType;
			}
			this._isAttackFired = false;
			PenitentSword penitentSword = (PenitentSword)this._penitent.PenitentAttack.CurrentPenitentWeapon;
			this._swordAnimatorInyector = penitentSword.SlashAnimator;
			this._penitent.OnAttackBehaviour_OnEnter(this);
			this._penitent.Audio.StopLoadingChargedAttack();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.Status.IsGrounded && !animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_3"))
			{
				float axis = this._penitent.PlatformCharacterInput.Rewired.GetAxis(0);
				this.SetOrientation(axis, animator);
			}
			if (!this._isAttackFired && this._penitent.PlatformCharacterInput.Attack)
			{
				this._isAttackFired = true;
			}
			if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Running")) && stateInfo.normalizedTime >= this.desiredPlayBackTime && this._isAttackFired && !this._penitentAttackArea.IsTargetHit && !this._penitent.PlatformCharacterInput.isJoystickUp)
			{
				animator.Play(this._attackAnim, 0, this.desiredBackwardTime);
				this._swordAnimatorInyector.PlayAttackDesiredTime(this._penitent.PenitentAttack.CurrentLevel, this.desiredBackwardTime, this._penitent.PenitentAttack.AttackColor, "Basic1_Lv");
			}
			if (this._penitent.Status.IsGrounded && this._penitent.PlatformCharacterInput.isJoystickDown && !this._penitent.PenitentAttack.IsRunningCombo && !this.IsDemakeMode)
			{
				this._penitent.Audio.AudioManager.StopSfx("PenitentAttack1", false);
				this._penitent.Audio.AudioManager.StopSfx("PenitentAttack2", false);
				animator.Play(this._crouchDownAnim);
			}
		}

		private void SetOrientation(float horAxis, Animator animator)
		{
			EntityOrientation orientation = this._penitent.Status.Orientation;
			if (horAxis < -0.2f && orientation != EntityOrientation.Left)
			{
				this._penitent.SetOrientation(EntityOrientation.Left, true, false);
				this._penitent.PenitentAttack.ResetCombo();
				if (this._penitent.PlatformCharacterInput.Attack)
				{
					animator.Play(this._attackAnim);
				}
			}
			else if (horAxis > 0.2f && orientation != EntityOrientation.Right)
			{
				this._penitent.SetOrientation(EntityOrientation.Right, true, false);
				this._penitent.PenitentAttack.ResetCombo();
				if (this._penitent.PlatformCharacterInput.Attack)
				{
					animator.Play(this._attackAnim);
				}
			}
		}

		private Penitent _penitent;

		private readonly int _attackAnim = Animator.StringToHash("Attack");

		private readonly int _crouchDownAnim = Animator.StringToHash("Crouch Down");

		private AttackArea _penitentAttackArea;

		[Tooltip("The damage amount done by the penitent in this attack")]
		public int damageAmount;

		[Range(0f, 1f)]
		public float desiredBackwardTime = 0.2f;

		[Range(0f, 1f)]
		public float desiredPlayBackTime = 0.45f;

		private bool _isAttackFired;

		[Tooltip("The sword spark will be generated in this animation")]
		public SwordSpark swordSpark;

		private SwordSparkSpawner _swordSparkSpawner;

		private SwordAnimatorInyector _swordAnimatorInyector;
	}
}
