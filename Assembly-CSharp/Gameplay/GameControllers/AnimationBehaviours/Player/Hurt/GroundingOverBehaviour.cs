using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Hurt
{
	public class GroundingOverBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._damageCollider = this._penitent.DamageArea.DamageAreaCollider;
			}
			this._animationSoundEffect = false;
			if (!this._wallJump)
			{
				this._wallJump = this._penitent.GetComponentInChildren<WallJump>();
			}
			if (this._wallJump.enabled)
			{
				return;
			}
			this._wallJump.enabled = true;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			animator.SetBool(GroundingOverBehaviour.StickOnWall, false);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._animationSoundEffect)
			{
				this._penitent.Audio.PlayOverthrow();
				this._animationSoundEffect = true;
			}
			if (!this._penitent.Status.Dead)
			{
				return;
			}
			if (stateInfo.normalizedTime >= 0.9f)
			{
				animator.enabled = false;
				if (this._damageCollider.enabled)
				{
					this._damageCollider.enabled = false;
				}
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			Core.Logic.Penitent.PlatformCharacterInput.ResetInputs();
			Core.Logic.Penitent.PlatformCharacterInput.ResetHorizontalBlockers();
			if (this._penitent.IsFallingStunt)
			{
				this._penitent.IsFallingStunt = !this._penitent.IsFallingStunt;
			}
		}

		private Penitent _penitent;

		private bool _animationSoundEffect;

		private WallJump _wallJump;

		private Collider2D _damageCollider;

		private static readonly int StickOnWall = Animator.StringToHash("STICK_ON_WALL");
	}
}
