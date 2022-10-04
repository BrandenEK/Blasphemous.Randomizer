using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.ShieldMaiden.Audio
{
	public class ShieldMaidenAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
		}

		private void Shield_OnShieldHitDetected()
		{
			this.PlayHitShield();
		}

		public void PlayHitShield()
		{
			base.PlayOneShotEvent("ShieldMaidenHitShield", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "ShieldMaidenAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void PlayDeath()
		{
			this.StopAttack();
			base.PlayOneShotEvent("ShieldMaidenDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayFootstep()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("ShieldMaidenFootstep", EntityAudio.FxSoundCategory.Motion);
			}
		}

		private const string FootstepEventKey = "ShieldMaidenFootstep";

		private const string DeathEventKey = "ShieldMaidenDeath";

		private const string AttackEventKey = "ShieldMaidenAttack";

		private const string HitShieldEventKey = "ShieldMaidenHitShield";

		private EventInstance _attackEventInstance;
	}
}
