using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.Audio
{
	public class PontiffHuskAudio : EntityAudio
	{
		public void Appear()
		{
			base.PlayOneShotEvent("PontiffHuskVanishIn", EntityAudio.FxSoundCategory.Motion);
		}

		public void ChargeAttack()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			if (!this._chargingEventInstance.isValid())
			{
				this._chargingEventInstance = base.AudioManager.CreateCatalogEvent("PontiffHuskMeleAttack", default(Vector3));
				this._chargingEventInstance.start();
				base.SetPanning(this._chargingEventInstance);
			}
		}

		public void Dissapear()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			base.PlayOneShotEvent("PontiffHuskVanishOut", EntityAudio.FxSoundCategory.Motion);
		}

		public void Death(bool cut)
		{
			this.StopShoot();
			if (cut)
			{
				base.PlayOneShotEvent("PontiffHuskMeleDeath", EntityAudio.FxSoundCategory.Motion);
			}
			else
			{
				base.PlayOneShotEvent("PontiffHuskRangedDeath", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayShoot()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			this.StopShoot();
			base.PlayEvent(ref this._shootEventInstance, "PontiffHuskRangedAttack", true);
		}

		public void StopShoot()
		{
			base.StopEvent(ref this._shootEventInstance);
		}

		public void PlayAttack()
		{
			if (base.AudioManager != null && !this._attackEventInstance.isValid())
			{
				this._attackEventInstance = base.AudioManager.CreateCatalogEvent("PontiffHuskMeleAttack", default(Vector3));
				this._attackEventInstance.start();
			}
		}

		public void UpdateAttackPanning()
		{
			if (this._attackEventInstance.isValid())
			{
				base.SetPanning(this._attackEventInstance);
			}
		}

		public void StopAttack(bool allowFade = true)
		{
			if (!this._attackEventInstance.isValid())
			{
				return;
			}
			this._attackEventInstance.stop((!allowFade) ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._attackEventInstance.release();
			this._attackEventInstance = default(EventInstance);
		}

		public void UpdateFloatingPanning()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			if (this._floatingEventInstance.isValid())
			{
				base.SetPanning(this._floatingEventInstance);
			}
		}

		public void StopFloating()
		{
			if (this._floatingEventInstance.isValid())
			{
				this._floatingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this._floatingEventInstance.release();
				this._floatingEventInstance = default(EventInstance);
			}
		}

		public void StopChargeAttack()
		{
			if (this._chargingEventInstance.isValid())
			{
				this._chargingEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this._chargingEventInstance.release();
				this._chargingEventInstance = default(EventInstance);
			}
		}

		private void OnDestroy()
		{
			this.StopFloating();
			this.StopChargeAttack();
		}

		private const string AttackRangedEventKey = "PontiffHuskRangedAttack";

		private const string AttackMeleEventKey = "PontiffHuskMeleAttack";

		private const string AppearEventKey = "PontiffHuskVanishIn";

		private const string DeathEventKey = "PontiffHuskRangedDeath";

		private const string CutDeathEventKey = "PontiffHuskMeleDeath";

		private const string DissapearEventKey = "PontiffHuskVanishOut";

		private EventInstance _attackEventInstance;

		private EventInstance _chargingEventInstance;

		private EventInstance _shootEventInstance;

		private EventInstance _floatingEventInstance;
	}
}
