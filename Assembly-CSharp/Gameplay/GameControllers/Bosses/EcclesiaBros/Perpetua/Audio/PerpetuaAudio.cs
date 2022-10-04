using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.Audio
{
	public class PerpetuaAudio : EntityAudio
	{
		public void PlayFly()
		{
			base.PlayOneShotEvent("PerpetuaFly", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDisappear()
		{
			base.PlayOneShotEvent("PerpetuaDisappear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAppear()
		{
			base.PlayOneShotEvent("PerpetuaAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySlideAttack()
		{
			base.PlayOneShotEvent("PerpetuaSlideAttack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayThunderSword()
		{
			base.PlayOneShotEvent("PerpetuaThunderSword", EntityAudio.FxSoundCategory.Motion);
		}

		public const string ThunderSwordEventKey = "PerpetuaThunderSword";

		public const string AppearEventKey = "PerpetuaAppear";

		public const string SlideAttackEventKey = "PerpetuaSlideAttack";

		public const string DisappearEventKey = "PerpetuaDisappear";

		public const string FlyEventKey = "PerpetuaFly";
	}
}
