using System;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellCarrier.Animator
{
	public class BellCarrierAnimatorInyector : EnemyAnimatorInyector
	{
		public Animator Animator
		{
			get
			{
				return base.EntityAnimator;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this._colorFlash = base.GetComponent<ColorFlash>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._cameraManager = Core.Logic.CameraManager;
			this._bellCarrier = base.GetComponentInParent<BellCarrier>();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool targetInLine = this._bellCarrier.BellCarrierBehaviour.TargetInLine;
			base.EntityAnimator.SetBool("TARGET_IN_LINE", targetInLine);
		}

		public void Idle()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
		}

		public void Awaken()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("AWAKE");
		}

		public void Chasing()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CHASING", true);
		}

		public void Stop()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CHASING", false);
			base.EntityAnimator.ResetTrigger("TURN_AROUND");
		}

		public void PlayStopAnimation()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("StopRun");
		}

		public void TurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN_AROUND");
		}

		public void ResetTurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("TURN_AROUND");
		}

		public bool IsInWallCrashAnim
		{
			get
			{
				return base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("WallCrash");
			}
		}

		public void PlayWallCrushAnimation()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play(this._wallCrashAnimHashName, 0, 0f);
		}

		public void PlayBellHidden()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("IdleToHidden");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void TriggerColorFlash()
		{
			if (this._colorFlash == null)
			{
				return;
			}
			this._colorFlash.TriggerColorFlash();
		}

		public void DropBellCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			if (this.OwnerEntity.SpriteRenderer.isVisible)
			{
				Core.Logic.CameraShakeManager.ShakeUsingPreset("SimpleHit", false);
			}
		}

		public void WallCrushCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			if (this.OwnerEntity.SpriteRenderer.isVisible)
			{
				Core.Logic.CameraShakeManager.ShakeUsingPreset("SimpleHit", true);
			}
		}

		public void HeavyStepCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			if (this.OwnerEntity.SpriteRenderer.isVisible)
			{
				this._cameraManager.ProCamera2DShake.ShakeUsingPreset("LadderLanding");
			}
		}

		public void SetDamageSoundType(BellCarrierAnimatorInyector.DamageSoundType soundType)
		{
			this.CurrentDamageSoundType = soundType;
		}

		public void PlayRun()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayRun();
		}

		public void PlayRunStop()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayRunStop();
		}

		public void PlayDropBell()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.DropBell();
		}

		public void StartToRun()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayStartToRun();
		}

		public void PlayTurnAround()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayTurnAround();
		}

		public void PlayTurnAroundRun()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayTurnAroundRun();
		}

		public void PlayFrontHit()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayBellCarrierFrontHit();
		}

		public void PlayWallCrush()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayBellCarrierWallCrush();
		}

		public void PlayDeath()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayDeath();
		}

		public void PlayWakeUp()
		{
			if (this._bellCarrier == null)
			{
				return;
			}
			this._bellCarrier.Audio.PlayWakeUp();
		}

		private readonly int _wallCrashAnimHashName = Animator.StringToHash("WallCrash");

		private BellCarrier _bellCarrier;

		private CameraManager _cameraManager;

		private ColorFlash _colorFlash;

		public BellCarrierAnimatorInyector.DamageSoundType CurrentDamageSoundType;

		public enum DamageSoundType
		{
			BeforeTurnAround,
			AfterTurnAround
		}
	}
}
