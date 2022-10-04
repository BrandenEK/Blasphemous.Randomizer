using System;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChimeRinger.Audio
{
	public class ChimeRingerAudio : EntityAudio
	{
		protected Penitent Penitent { get; set; }

		protected override void OnUpdate()
		{
		}

		public void PlayCall()
		{
			this.StopAll();
			Core.Audio.PlayEventWithCatalog(ref this._callEventInstance, "ChimeRingerCall", base.transform.position);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("ChimeRingerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void StopAll()
		{
			base.StopEvent(ref this._callEventInstance);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.maxAudioDistance);
			Gizmos.DrawWireSphere(base.transform.position, this.minAudioDistance);
			Gizmos.color = Color.Lerp(Color.red, Color.blue, this._lastDistanceParam);
			Gizmos.DrawWireSphere(base.transform.position, this._lastDistance);
		}

		private const string DeathEventKey = "ChimeRingerDeath";

		private const string CallEventKey = "ChimeRingerCall";

		public float maxAudioDistance = 20f;

		public float minAudioDistance = 7f;

		private EventInstance _callEventInstance;

		private float _lastDistance;

		private float _lastDistanceParam;
	}
}
