using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.PontiffGiant.Audio
{
	public class PontiffGiantAudio : EntityAudio
	{
		public void PlayPurpleSpell_AUDIO()
		{
			base.PlayOneShotEvent("PontiffGiantPurpleSpell", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayGreenSpell_AUDIO()
		{
			base.PlayOneShotEvent("PontiffGiantGreenSpell", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBlueSpell_AUDIO()
		{
			base.PlayOneShotEvent("PontiffGiantBlueSpell", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayDeath_AUDIO()
		{
			base.PlayOneShotEvent("PontiffGiantDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayOpenMask()
		{
			base.PlayOneShotEvent("PontiffGiantMaskOpen", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayCloseMask()
		{
			base.PlayOneShotEvent("PontiffGiantMaskClose", EntityAudio.FxSoundCategory.Motion);
		}

		private const string PontiffGiant_DEATH = "PontiffGiantDeath";

		private const string PontiffGiant_PURPLE_SPELL = "PontiffGiantPurpleSpell";

		private const string PontiffGiant_GREEN_SPELL = "PontiffGiantGreenSpell";

		private const string PontiffGiant_BLUE_SPELL = "PontiffGiantBlueSpell";

		private const string PontiffGiant_OPEN_MASK = "PontiffGiantMaskOpen";

		private const string PontiffGiant_CLOSE_MASK = "PontiffGiantMaskClose";
	}
}
