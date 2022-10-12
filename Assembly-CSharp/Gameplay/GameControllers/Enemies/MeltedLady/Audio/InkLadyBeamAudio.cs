using System;
using FMOD.Studio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Audio
{
	public class InkLadyBeamAudio : MonoBehaviour
	{
		private void Awake()
		{
			this._audio = UnityEngine.Object.FindObjectOfType<InkLadyAudio>();
		}

		public void PlayBeamCharge()
		{
			this._audio.PlayBeamCharge(ref this._beamFireEventInstance);
		}

		public void PlayBeamFire()
		{
			this._audio.PlayBeamFire(ref this._beamFireEventInstance);
		}

		public void StopBeamFire()
		{
			this._audio.StopBeamFire(ref this._beamFireEventInstance);
		}

		private InkLadyAudio _audio;

		private EventInstance _beamFireEventInstance;
	}
}
