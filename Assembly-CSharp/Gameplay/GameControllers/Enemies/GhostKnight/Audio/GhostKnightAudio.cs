using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GhostKnight.Audio
{
	public class GhostKnightAudio : EntityAudio
	{
		public void Appear()
		{
			base.PlayOneShotEvent("GhostKnightAppearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void Disappear()
		{
			base.PlayOneShotEvent("GhostKnightDisappearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void Damage()
		{
			base.PlayOneShotEvent("GhostKnightHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void Death()
		{
			base.PlayOneShotEvent("GhostKnightDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void StartAttack()
		{
			base.PlayOneShotEvent("GhostKnightStartAttack", EntityAudio.FxSoundCategory.Damage);
		}

		public void Attack()
		{
			if (!this._attackEventInstance.isValid())
			{
				this._attackEventInstance = base.AudioManager.CreateCatalogEvent("GhostKnightAttack", default(Vector3));
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

		public void StopAttack()
		{
			if (!this._attackEventInstance.isValid())
			{
				return;
			}
			this._attackEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			this._attackEventInstance.release();
			this._attackEventInstance = default(EventInstance);
		}

		private const string Prefix = "GhostKnight";

		private const string AppearingEventKey = "GhostKnightAppearing";

		private const string DisappearingEventKey = "GhostKnightDisappearing";

		private const string StartAttackEventKey = "GhostKnightStartAttack";

		private const string AttackEventKey = "GhostKnightAttack";

		private const string HitEventKey = "GhostKnightHit";

		private const string DeathEventKey = "GhostKnightDeath";

		private EventInstance _attackEventInstance;
	}
}
