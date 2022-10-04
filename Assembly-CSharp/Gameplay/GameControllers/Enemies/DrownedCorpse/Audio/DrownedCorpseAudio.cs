using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.Audio
{
	public class DrownedCorpseAudio : EntityAudio
	{
		public void PlayRun()
		{
			base.PlayOneShotEvent("DrownedCorpseRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayHidden()
		{
			base.PlayOneShotEvent("DrownedCorpseHidden", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayWakeup()
		{
			base.PlayOneShotEvent("DronwedCorpseWakeUp", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDamageHidden()
		{
			base.PlayOneShotEvent("DrownedCorpseDamageHidden", EntityAudio.FxSoundCategory.Damage);
		}

		private const string RunEventKey = "DrownedCorpseRun";

		private const string HiddenEventKey = "DrownedCorpseHidden";

		private const string WakeupEventKey = "DronwedCorpseWakeUp";

		private const string DamageHiddenEventKey = "DrownedCorpseDamageHidden";
	}
}
