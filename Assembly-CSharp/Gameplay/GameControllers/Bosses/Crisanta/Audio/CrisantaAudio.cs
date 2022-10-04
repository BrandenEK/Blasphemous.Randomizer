using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Bosses.Crisanta.Audio
{
	public class CrisantaAudio : EntityAudio
	{
		public void PlayDeath_AUDIO()
		{
		}

		public void PlayTeleportIn_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaTeleportIn", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTeleportOut_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaTeleportOut", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayParry_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaParry", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayKnee_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaKneeDown", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAirAttack_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaAirAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayRoll_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaRoll", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLanding_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayGuard_AUDIO()
		{
			base.PlayEvent(ref this._guardEventInstance, "CrisantaGuard", true);
		}

		public void StopGuard_AUDIO()
		{
			base.StopEvent(ref this._guardEventInstance);
		}

		public void PlaySwordAttack_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaSwordAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayGuardAttack_AUDIO()
		{
			base.PlayOneShotEvent("CrisantaGuardAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayGroundHit_AUDIO()
		{
		}

		public void PlayLightAttack_AUDIO()
		{
			this.PlayGuardAttack_AUDIO();
		}

		private const string Crisanta_DEATH = "CrisantaDeath";

		private const string Crisanta_NORMAL_ATTACK = "CrisantaNormalAttack";

		private const string Crisanta_GROUND_HIT = "CrisantaGroundHit";

		private const string Crisanta_TELEPORT_IN = "CrisantaTeleportIn";

		private const string Crisanta_TELEPORT_OUT = "CrisantaTeleportOut";

		private const string Crisanta_ROLL = "CrisantaRoll";

		private const string Crisanta_LANDING = "CrisantaLanding";

		private const string Crisanta_GUARD = "CrisantaGuard";

		private const string Crisanta_AIR_ATTACK = "CrisantaAirAttack";

		private const string Crisanta_SWORD_ATTACK = "CrisantaSwordAttack";

		private const string Crisanta_GUARD_ATTACK = "CrisantaGuardAttack";

		private const string Crisanta_TELEPORT_ATTACK = "CrisantaTeleportAttack";

		private const string Crisanta_PARRY = "CrisantaParry";

		private const string Crisanta_KNEE = "CrisantaKneeDown";

		private EventInstance _guardEventInstance;
	}
}
