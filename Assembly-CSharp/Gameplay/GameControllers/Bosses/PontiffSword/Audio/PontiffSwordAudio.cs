using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.PontiffSword.Audio
{
	public class PontiffSwordAudio : EntityAudio
	{
		public void PlayAppear_AUDIO()
		{
			base.PlayOneShotEvent("PontiffSwordAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayVanish_AUDIO()
		{
			base.PlayOneShotEvent("PontiffSwordVanish", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPlunge_AUDIO()
		{
			base.PlayOneShotEvent("PontiffSwordPlunge", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySlash_AUDIO()
		{
			base.PlayOneShotEvent("PontiffSwordSlash", EntityAudio.FxSoundCategory.Attack);
		}

		private const string PontiffSword_APPEAR = "PontiffSwordAppear";

		private const string PontiffSword_VANISH = "PontiffSwordVanish";

		private const string PontiffSword_PLUNGE = "PontiffSwordPlunge";

		private const string PontiffSword_SLASH = "PontiffSwordSlash";
	}
}
