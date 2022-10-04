using System;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CauldronNun.Audio
{
	public class CauldronNunAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._callEventInstance.isValid())
			{
				return;
			}
			Core.Audio.ApplyDistanceParam(ref this._callEventInstance, this.minAudioDistance, this.maxAudioDistance, base.transform, Core.Logic.Penitent.transform);
			EntityAudio.SetPanning(this._callEventInstance, base.transform.position);
		}

		public void PlayCall()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			this.StopAll();
			Core.Audio.PlayEventWithCatalog(ref this._callEventInstance, "CauldronNunCall", default(Vector3));
		}

		public void StopAll()
		{
			base.StopEvent(ref this._callEventInstance);
		}

		public void PlayDeath()
		{
			this.StopAll();
			base.PlayOneShotEvent("CauldronNunDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.maxAudioDistance);
			Gizmos.DrawWireSphere(base.transform.position, this.minAudioDistance);
			Gizmos.color = Color.Lerp(Color.red, Color.blue, this._lastDistanceParam);
			Gizmos.DrawWireSphere(base.transform.position, this._lastDistance);
		}

		private const string DeathEventKey = "CauldronNunDeath";

		private const string CallEventKey = "CauldronNunCall";

		public float maxAudioDistance = 20f;

		public float minAudioDistance = 7f;

		private EventInstance _callEventInstance;

		private float _lastDistance;

		private float _lastDistanceParam;
	}
}
