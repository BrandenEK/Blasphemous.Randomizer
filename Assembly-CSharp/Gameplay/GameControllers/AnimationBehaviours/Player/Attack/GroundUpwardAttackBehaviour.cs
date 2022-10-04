using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class GroundUpwardAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._defaultAttackAreaOffset = new Vector2(this._penitent.AttackArea.WeaponCollider.offset.x, this._penitent.AttackArea.WeaponCollider.offset.y);
				this._defaultAttackAreaSize = new Vector2(this._penitent.AttackArea.WeaponCollider.bounds.size.x, this._penitent.AttackArea.WeaponCollider.bounds.size.y);
				this._penitentSword = (PenitentSword)this._penitent.PenitentAttack.CurrentPenitentWeapon;
				this._swordAnimatorInyector = this._penitentSword.SlashAnimator;
			}
			this._isAttackFired = false;
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._penitent.AttackArea.SetOffset(this.AttackAreaOffset);
			this._penitent.AttackArea.SetSize(this.AttackAreaSize);
			if (!this._isAttackFired && this._penitent.PlatformCharacterInput.Attack)
			{
				this._isAttackFired = true;
			}
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("GroundUpwardAttack") && stateInfo.normalizedTime >= this.DesiredPlayBackTime && this._isAttackFired && !this._penitent.AttackArea.IsTargetHit && this._penitent.PlatformCharacterInput.isJoystickUp)
			{
				animator.Play("GroundUpwardAttack", 0, this.DesiredBackwardTime);
				this._swordAnimatorInyector.PlayAttackDesiredTime(this._penitent.PenitentAttack.CurrentLevel, this.DesiredBackwardTime, this._penitent.PenitentAttack.AttackColor, "BasicUpward_Lv");
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._penitent.AttackArea.SetOffset(this._defaultAttackAreaOffset);
			this._penitent.AttackArea.SetSize(this._defaultAttackAreaSize);
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
		}

		private Penitent _penitent;

		private bool _climbingDisabled;

		public Vector2 AttackAreaOffset;

		public Vector2 AttackAreaSize;

		private Vector2 _defaultAttackAreaOffset;

		private Vector2 _defaultAttackAreaSize;

		private bool _isAttackFired;

		[Range(0f, 1f)]
		public float DesiredPlayBackTime = 0.45f;

		[Range(0f, 1f)]
		public float DesiredBackwardTime = 0.2f;

		private SwordAnimatorInyector _swordAnimatorInyector;

		private PenitentSword _penitentSword;
	}
}
