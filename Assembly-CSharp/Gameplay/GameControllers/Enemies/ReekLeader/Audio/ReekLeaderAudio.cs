using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ReekLeader.Audio
{
	public class ReekLeaderAudio : EntityAudio
	{
		public void PlayIdle()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			if (this._idleEventInstance.isValid())
			{
				return;
			}
			this._idleEventInstance = base.AudioManager.CreateCatalogEvent("LeperLeaderIdle", default(Vector3));
			this._idleEventInstance.start();
		}

		public void StopIdle()
		{
			if (!this._idleEventInstance.isValid())
			{
				return;
			}
			this._idleEventInstance.stop(0);
			this._idleEventInstance.release();
			this._idleEventInstance = default(EventInstance);
		}

		public void PlayCall()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			if (this._callEventInstance.isValid())
			{
				return;
			}
			this._callEventInstance = base.AudioManager.CreateCatalogEvent("LeperLeaderCall", default(Vector3));
			this._callEventInstance.start();
		}

		public void StopCall()
		{
			if (!this._callEventInstance.isValid())
			{
				return;
			}
			this._callEventInstance.stop(0);
			this._callEventInstance.release();
			this._callEventInstance = default(EventInstance);
		}

		public void PlayDeath()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("LeperLeaderDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string IdleEventKey = "LeperLeaderIdle";

		private const string CallEventKey = "LeperLeaderCall";

		private const string DeathEventKey = "LeperLeaderDeath";

		private EventInstance _idleEventInstance;

		private EventInstance _callEventInstance;
	}
}
