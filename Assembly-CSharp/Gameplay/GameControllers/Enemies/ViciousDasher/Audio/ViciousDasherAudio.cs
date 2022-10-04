using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.Audio
{
	public class ViciousDasherAudio : EntityAudio
	{
		public void PlayAttack()
		{
			base.PlayOneShotEvent("ViciousDasherAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayDash()
		{
			base.PlayOneShotEvent("ViciousDasherDash", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("ViciousDasherDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string AttackEventKey = "ViciousDasherAttack";

		private const string DashEventKey = "ViciousDasherDash";

		private const string DeathEventKey = "ViciousDasherDeath";
	}
}
