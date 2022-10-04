using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.WallEnemy.Audio
{
	public class BasicWallEnemyAudio : WallEnemyAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.Owner.OnDeath += this.OnOwnerDeath;
		}

		private void OnOwnerDeath()
		{
			this.StopAttack();
		}

		public override void PlayAttack()
		{
			this.StopAttack();
			base.PlayEvent(ref this._attackAudioInstance, "WallEnemyAttack", true);
		}

		public override void PlayWoosh()
		{
			base.PlayOneShotEvent("WallEnemyWoosh", EntityAudio.FxSoundCategory.Damage);
		}

		public override void StopAttack()
		{
			base.StopEvent(ref this._attackAudioInstance);
		}

		public override void PlayDamage()
		{
			base.PlayOneShotEvent("WallEnemyDamage", EntityAudio.FxSoundCategory.Damage);
		}

		public override void PlayDeath()
		{
			base.PlayOneShotEvent("WallEnemyDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private void OnDestroy()
		{
			this.Owner.OnDeath -= this.OnOwnerDeath;
		}

		private const string AttackEventKey = "WallEnemyAttack";

		private const string WooshEventKey = "WallEnemyWoosh";

		private const string DamageEventKey = "WallEnemyDamage";

		private const string DeathEventKey = "WallEnemyDeath";

		private EventInstance _attackAudioInstance;
	}
}
