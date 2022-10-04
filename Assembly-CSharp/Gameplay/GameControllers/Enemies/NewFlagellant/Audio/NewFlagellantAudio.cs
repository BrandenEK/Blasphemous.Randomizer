using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.Audio
{
	public class NewFlagellantAudio : EntityAudio
	{
		public void PlayFootStep()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("FlagellantFootStep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRunning()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("FlagellantRunning", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLandingSound()
		{
			base.PlayOneShotEvent("FlagellantLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBasicAttack()
		{
			base.PlayOneShotEvent("FlagellantBasicAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAttack()
		{
			base.PlayOneShotEvent("FlagellantAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAttackHit()
		{
			base.PlayOneShotEvent("FlagellantAttackHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySelfLash()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantSelfLash");
		}

		public void PlayBloodDecal()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantBloodDecal");
		}

		public void PlayDeath()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantDeath");
		}

		public void PlaySlashHit()
		{
			this.PlayAttackHit();
		}

		private const string AttackEventKey = "FlagellantAttack";

		private const string DashEventKey = "FlagellantDash";

		private const string DeathEventKey = "FlagellantDeath";

		public const string FootStepEventKey = "FlagellantFootStep";

		public const string RunnigEventKey = "FlagellantRunning";

		public const string LandingEventKey = "FlagellantLanding";

		public const string BasicAttackEventKey = "FlagellantBasicAttack";

		public const string BasicAttackHitEventKey = "FlagellantAttackHit";

		public const string SelfLashEventKey = "FlagellantSelfLash";

		public const string BloodDecalEventKey = "FlagellantBloodDecal";

		private EventInstance _basicAttackEventInstance;
	}
}
