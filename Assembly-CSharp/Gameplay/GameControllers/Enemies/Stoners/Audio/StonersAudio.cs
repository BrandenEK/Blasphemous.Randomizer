using System;
using System.Collections.Generic;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Stoners.Audio
{
	public class StonersAudio : EntityAudio
	{
		protected override void OnWake()
		{
			base.OnWake();
			this.EventInstances = new List<EventInstance>();
		}

		public void Raise()
		{
			base.PlayOneShotEvent("StonersRaise", EntityAudio.FxSoundCategory.Motion);
		}

		public void ThrowRock()
		{
			base.PlayOneShotEvent("StonersThrowRock", EntityAudio.FxSoundCategory.Attack);
		}

		public void PassRock()
		{
			base.PlayOneShotEvent("StonersPassRock", EntityAudio.FxSoundCategory.Attack);
		}

		public void Damage()
		{
			base.PlayOneShotEvent("StonersDamage", EntityAudio.FxSoundCategory.Damage);
		}

		public void Death()
		{
			base.PlayOneShotEvent("StonersDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public const string RaiseEventKey = "StonersRaise";

		public const string ThrowRockEventKey = "StonersThrowRock";

		public const string PassRockEventKey = "StonersPassRock";

		public const string RockHit = "StonersRockHit";

		public const string DamageEventKey = "StonersDamage";

		public const string DeathEventKey = "StonersDeath";
	}
}
