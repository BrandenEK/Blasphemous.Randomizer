using System;
using FMODUnity;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.WallEnemy.Audio
{
	public abstract class WallEnemyAudio : EntityAudio
	{
		public abstract void PlayAttack();

		public abstract void StopAttack();

		public abstract void PlayDamage();

		public abstract void PlayDeath();

		public abstract void PlayWoosh();

		[EventRef]
		public string DamageEvent;
	}
}
