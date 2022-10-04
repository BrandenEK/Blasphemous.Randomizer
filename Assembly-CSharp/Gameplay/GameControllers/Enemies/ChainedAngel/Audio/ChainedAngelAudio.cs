using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.Audio
{
	public class ChainedAngelAudio : EntityAudio
	{
		public void PlayFlap()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("AngelFly", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayAttack()
		{
			base.PlayOneShotEvent("CageAngelAttack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("CageAngelDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public const string FlyEventKey = "AngelFly";

		public const string AttackEventKey = "CageAngelAttack";

		public const string DeathEventKey = "CageAngelDeath";
	}
}
