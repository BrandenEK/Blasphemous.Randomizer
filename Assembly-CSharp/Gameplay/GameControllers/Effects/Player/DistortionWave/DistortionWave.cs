using System;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.DistortionWave
{
	public class DistortionWave : MonoBehaviour
	{
		private void Start()
		{
			if (this.DistortionParticleSystem == null)
			{
				Debug.LogError("A particle system is needed!");
			}
		}

		public void PreWarm()
		{
			if (this.DistortionParticleSystem == null)
			{
				return;
			}
			if (this.DistortionParticleSystem.isEmitting)
			{
				this.DistortionParticleSystem.Stop();
			}
			ParticleSystem.MainModule main = this.DistortionParticleSystem.main;
			main.loop = true;
			main.prewarm = true;
		}

		public void Play()
		{
			if (this.DistortionParticleSystem == null)
			{
				return;
			}
			this.DistortionParticleSystem.Play();
			ParticleSystem.MainModule main = this.DistortionParticleSystem.main;
			main.loop = false;
			main.prewarm = false;
		}

		public ParticleSystem DistortionParticleSystem;
	}
}
