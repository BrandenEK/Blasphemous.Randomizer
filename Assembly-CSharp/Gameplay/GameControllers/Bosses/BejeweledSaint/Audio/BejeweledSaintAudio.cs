using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Audio
{
	public class BejeweledSaintAudio : EntityAudio
	{
		public void PlaySaintFall()
		{
			base.PlayOneShotEvent("SaintFall", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySaintRise()
		{
			base.PlayOneShotEvent("SaintRise", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayHeadDamage()
		{
			base.PlayOneShotEvent("SaintHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("SaintDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySmashMace()
		{
			base.PlayOneShotEvent("SaintAttack", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayLiftUpMace()
		{
			base.PlayOneShotEvent("SaintAttackRelease", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlaySlideAttack()
		{
			base.PlayOneShotEvent("SaintAttackSlide", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayHandStomp()
		{
			base.PlayOneShotEvent("SaintHandStomp", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayThunder()
		{
			base.PlayOneShotEvent("SaintThunder", EntityAudio.FxSoundCategory.Damage);
		}

		private const string SmashMaceEventKey = "SaintAttack";

		private const string LiftUpMaceEventKey = "SaintAttackRelease";

		private const string AttackSlideEventKey = "SaintAttackSlide";

		private const string HandStompEventKey = "SaintHandStomp";

		private const string ThunderEventKey = "SaintThunder";

		private const string SaintFallEventKey = "SaintFall";

		private const string SaintRiseEventKey = "SaintRise";

		private const string HitEventKey = "SaintHit";

		private const string DeathEventKey = "SaintDeath";
	}
}
