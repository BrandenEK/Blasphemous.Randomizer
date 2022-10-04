using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Fool.Audio
{
	public class FoolAudio : EntityAudio
	{
		public void PlayFootStep()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.PlayOneShotEvent("FoolFootStep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.PlayOneShotEvent("FoolDeath", EntityAudio.FxSoundCategory.Motion);
		}

		private const string FootStepEventKey = "FoolFootStep";

		private const string DeathEventKey = "FoolDeath";
	}
}
