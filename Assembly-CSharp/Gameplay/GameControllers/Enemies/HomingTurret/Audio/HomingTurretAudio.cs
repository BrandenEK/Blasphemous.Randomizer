using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.Audio
{
	public class HomingTurretAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.OwnerSprite = this.Owner.SpriteRenderer;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.StopIdleSoundWhenInvisible();
		}

		public void PlayWakeUp()
		{
			base.PlayOneShotEvent("HomingTurretWakeUp", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayIdle()
		{
			this.StopIdle();
			this._isPlayingIdle = true;
			this._onMute = false;
			base.PlayEvent(ref this._idleEventInstance, "HomingTurretIdle", true);
		}

		public void StopIdle()
		{
			this._isPlayingIdle = false;
			base.StopEvent(ref this._idleEventInstance);
		}

		public void PlayChargeProjectile()
		{
			if (this.OwnerSprite.isVisible)
			{
				base.PlayOneShotEvent("ProjectileCharge", EntityAudio.FxSoundCategory.Attack);
			}
		}

		public void PlayShotProjectile()
		{
			base.PlayOneShotEvent("HomingTurretShot", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("HomingTurretDeath", EntityAudio.FxSoundCategory.Motion);
		}

		private void StopIdleSoundWhenInvisible()
		{
			if (this.Owner.Status.Dead)
			{
				return;
			}
			if (!this.OwnerSprite.isVisible && this._isPlayingIdle)
			{
				this._onMute = true;
				this.StopIdle();
			}
			else if (this.OwnerSprite.isVisible && this._onMute)
			{
				this.PlayIdle();
			}
		}

		private void OnDisable()
		{
			this.StopIdle();
		}

		private const string WakeupEventKey = "HomingTurretWakeUp";

		private const string IdleEventKey = "HomingTurretIdle";

		private const string ShotProjectileEventKey = "HomingTurretShot";

		private const string DeathEventKey = "HomingTurretDeath";

		private const string ChargeProjectileKey = "ProjectileCharge";

		private EventInstance _idleEventInstance;

		private SpriteRenderer OwnerSprite;

		[SerializeField]
		private float maxTimeInvisibleBeforeMute = 2f;

		private float _currentInvisibleTime;

		private bool _isPlayingIdle;

		private bool _onMute;
	}
}
