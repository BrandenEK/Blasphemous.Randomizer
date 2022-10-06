using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.Audio
{
	public class BellGhostAudio : EntityAudio
	{
		public void Hurt()
		{
			base.PlayOneShotEvent("BellGhostHurt", EntityAudio.FxSoundCategory.Motion);
		}

		public void HurtVariant()
		{
			this.StopShoot();
			base.PlayOneShotEvent("GhostHurt", EntityAudio.FxSoundCategory.Motion);
		}

		public void BrokenBell()
		{
			base.PlayOneShotEvent("BellGhostBrokenBell", EntityAudio.FxSoundCategory.Motion);
		}

		public void Death()
		{
			this.StopShoot();
			base.PlayOneShotEvent("BellGhostDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayShoot()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			base.PlayEvent(ref this._shootEventInstance, "GhostShoot", true);
		}

		public void StopShoot()
		{
			base.StopEvent(ref this._shootEventInstance);
		}

		public void PlayAttack()
		{
			if (base.AudioManager != null && !this._attackEventInstance.isValid())
			{
				this._attackEventInstance = base.AudioManager.CreateCatalogEvent("BellGhostAttack", default(Vector3));
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
			this._attackEventInstance.stop((!allowFade) ? 1 : 0);
			this._attackEventInstance.release();
			this._attackEventInstance = default(EventInstance);
		}

		public void Appear()
		{
			base.PlayOneShotEvent("BellGhostAppearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void Dissapear()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			base.PlayOneShotEvent("BellGhostDissapearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayFloating()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			if (!this._floatingEventInstance.isValid())
			{
				this._floatingEventInstance = base.AudioManager.CreateCatalogEvent("BellGhostFloating", default(Vector3));
				this._floatingEventInstance.start();
			}
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
				this._floatingEventInstance.stop(0);
				this._floatingEventInstance.release();
				this._floatingEventInstance = default(EventInstance);
			}
		}

		public void ChargeAttack()
		{
			if (!this.Owner.Status.IsVisibleOnCamera)
			{
				return;
			}
			if (!this._chargingEventInstance.isValid())
			{
				this._chargingEventInstance = base.AudioManager.CreateCatalogEvent("BellGhostCharge", default(Vector3));
				this._chargingEventInstance.start();
				base.SetPanning(this._chargingEventInstance);
			}
		}

		public void StopChargeAttack()
		{
			if (this._chargingEventInstance.isValid())
			{
				this._chargingEventInstance.stop(1);
				this._chargingEventInstance.release();
				this._chargingEventInstance = default(EventInstance);
			}
		}

		private void OnDestroy()
		{
			this.StopFloating();
			this.StopChargeAttack();
		}

		private const string HurtEventKey = "BellGhostHurt";

		private const string AttackEventKey = "BellGhostAttack";

		private const string AppearEventKey = "BellGhostAppearing";

		private const string BrokenBellEventKey = "BellGhostBrokenBell";

		private const string DeathEventKey = "BellGhostDeath";

		private const string DissapearEventKey = "BellGhostDissapearing";

		private const string FloatingEventKey = "BellGhostFloating";

		private const string ChargeEventKey = "BellGhostCharge";

		private const string ShootEventKey = "GhostShoot";

		private const string VariantHurtKey = "GhostHurt";

		private EventInstance _attackEventInstance;

		private EventInstance _chargingEventInstance;

		private EventInstance _shootEventInstance;

		private EventInstance _floatingEventInstance;
	}
}
