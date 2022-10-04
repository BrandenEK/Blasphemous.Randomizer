using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.InputSystem;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class PlayerIdleMode : Trait
	{
		private bool IsDemakeMode
		{
			get
			{
				return Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._playerInput = Core.Logic.Penitent.PlatformCharacterInput;
			this._currentPlayerMode = PlayerIdleMode.PlayerMode.Play;
			base.EntityOwner.OnDamaged += this.OnDamaged;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.IsDemakeMode)
			{
				return;
			}
			PlayerIdleMode.PlayerMode currentPlayerMode = this._currentPlayerMode;
			if (currentPlayerMode != PlayerIdleMode.PlayerMode.Play)
			{
				if (currentPlayerMode == PlayerIdleMode.PlayerMode.Idle)
				{
					if (this.InputReceived)
					{
						this.SetToPlayMode(null);
					}
				}
			}
			else if (base.EntityOwner.Status.IsIdle && !Core.Input.InputBlocked)
			{
				this._currentIdleTime += Time.deltaTime;
				if (this._currentIdleTime >= this.timeToIdleMode)
				{
					this.SetToIdleMode(null);
				}
			}
			else
			{
				this.ResetIdleTimer();
			}
		}

		private void SetToIdleMode(Action callBack = null)
		{
			this._currentPlayerMode = PlayerIdleMode.PlayerMode.Idle;
			base.EntityOwner.Animator.SetBool("IS_IDLE_MODE", true);
			Core.Logic.Penitent.Audio.PlayIdleModeBlood();
			if (callBack != null)
			{
				callBack();
			}
		}

		private void SetToPlayMode(Action callBack = null)
		{
			this._currentPlayerMode = PlayerIdleMode.PlayerMode.Play;
			base.EntityOwner.Animator.SetBool("IS_IDLE_MODE", false);
			Core.Logic.Penitent.Audio.StopIdleModeBlood();
			this.ResetIdleTimer();
			if (callBack != null)
			{
				callBack();
			}
		}

		private void ResetIdleTimer()
		{
			if (this._currentIdleTime > 0f)
			{
				this._currentIdleTime = 0f;
			}
		}

		private bool InputReceived
		{
			get
			{
				bool flag = Math.Abs(this._playerInput.FHorAxis) > float.Epsilon;
				bool flag2 = Math.Abs(this._playerInput.FVerAxis) > float.Epsilon;
				return this._playerInput.Rewired.GetAnyButton() || flag || flag2;
			}
		}

		private void SetAnimation(int animation)
		{
			Animator animator = base.EntityOwner.Animator;
			if (animator)
			{
				animator.Play(animation);
			}
		}

		private void OnDamaged()
		{
			this.SetToPlayMode(null);
		}

		private void OnDestroy()
		{
			base.EntityOwner.OnDamaged -= this.OnDamaged;
		}

		[Range(0f, 60f)]
		public float timeToIdleMode = 15f;

		private float _currentIdleTime;

		private PlatformCharacterInput _playerInput;

		private PlayerIdleMode.PlayerMode _currentPlayerMode;

		private enum PlayerMode
		{
			Play,
			Idle
		}
	}
}
