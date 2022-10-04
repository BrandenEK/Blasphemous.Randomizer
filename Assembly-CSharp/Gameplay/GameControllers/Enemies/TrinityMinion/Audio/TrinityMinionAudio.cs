using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.TrinityMinion.Audio
{
	public class TrinityMinionAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayFlap()
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				return;
			}
			base.PlayOneShotEvent("TrinityMinionFly", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("TrinityMinionDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void StopAll()
		{
		}

		private const string DeathEventKey = "TrinityMinionDeath";

		private const string FlyEventKey = "TrinityMinionFly";

		private EventInstance _attackEventInstance;
	}
}
