using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.HeadThrower.Audio
{
	public class HeadThrowerAudio : EntityAudio
	{
		public void PlayDeath()
		{
			if (this.Owner != null)
			{
				base.PlayOneShotEvent("HeadThrowerDeath", EntityAudio.FxSoundCategory.Damage);
			}
		}

		private const string DeathEventKey = "HeadThrowerDeath";
	}
}
