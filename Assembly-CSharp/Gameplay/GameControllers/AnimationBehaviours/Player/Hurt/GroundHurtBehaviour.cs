using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Hurt
{
	public class GroundHurtBehaviour : StateMachineBehaviour
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
			if (!this._penitent)
			{
				this._penitent = Core.Logic.Penitent;
			}
			Hit lastHit = this._penitent.DamageArea.LastHit;
			this._penitent.DamageArea.HitDisplacement(lastHit.AttackingEntity.transform.position);
			this._penitent.Status.Unattacable = true;
			this._penitent.IsJumpingOff = false;
			this._penitent.AnimatorInyector.FireJumpOffTrigger = false;
			this._animCompleted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.Stats.Life.Current <= 0f)
			{
				animator.Play(this._groundingOverAnimaHash);
			}
			if (stateInfo.normalizedTime >= 0.9f)
			{
				this._animCompleted = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._penitent.MotionLerper.IsLerping && !this.IsDemakeMode)
			{
				this._penitent.MotionLerper.StopLerping();
			}
			if (this._animCompleted)
			{
				Singleton<Core>.Instance.StartCoroutine(this.SetInvulnerabilityCoroutine());
			}
			else
			{
				this._penitent.Status.Unattacable = false;
			}
			if (this._penitent.PlatformCharacterInput.isJoystickDown)
			{
				animator.Play(this._crouchDownAnimaHash, 0, 0.9f);
			}
		}

		private IEnumerator SetInvulnerabilityCoroutine()
		{
			this._penitent.Status.Unattacable = true;
			yield return new WaitForSeconds(this._penitent.DamageArea.InvulnerabilityLapse);
			this._penitent.Status.Unattacable = false;
			yield break;
		}

		private Penitent _penitent;

		private bool _animCompleted;

		private readonly int _crouchDownAnimaHash = Animator.StringToHash("Crouch Down");

		private readonly int _groundingOverAnimaHash = Animator.StringToHash("Grounding Over");
	}
}
