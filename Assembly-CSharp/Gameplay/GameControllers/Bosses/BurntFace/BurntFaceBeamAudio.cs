using System;
using FMOD.Studio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceBeamAudio : MonoBehaviour
	{
		private void Awake()
		{
			this._audio = UnityEngine.Object.FindObjectOfType<BurntFaceAudio>();
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

		private BurntFaceAudio _audio;

		private EventInstance _beamFireEventInstance;
	}
}
