using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.Audio
{
	public class PatrollingFlyingEnemyAudio : EntityAudio
	{
		public void PlayFlap()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			Core.Audio.PlaySfx(this.FlyEventKey, 0f);
		}

		public void PlayDeath()
		{
			Core.Audio.PlaySfx(this.DeathEventKey, 0f);
		}

		[EventRef]
		public string DeathEventKey;

		[EventRef]
		public string FlyEventKey;

		private EventInstance _attackEventInstance;
	}
}
