using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.Audio
{
	public abstract class CreepCrawlerAudio : EntityAudio
	{
		protected void OnEnable()
		{
			if (!this.Spawned)
			{
				this.Spawned = true;
			}
		}

		public abstract void Appear();

		public abstract void Disappear();

		public abstract void Hurt();

		public abstract void Death();

		public abstract void Walk();

		protected bool Spawned;
	}
}
