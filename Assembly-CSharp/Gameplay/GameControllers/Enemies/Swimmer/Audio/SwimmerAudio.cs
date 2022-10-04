using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Swimmer.Audio
{
	public class SwimmerAudio : EntityAudio
	{
		public Swimmer Swimmer { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Swimmer = (Swimmer)this.Owner;
			this.Swimmer.OnDeath += this.OnDeath;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Swimmer.Behaviour.IsSwimming)
			{
				this.PlaySwim();
			}
			else
			{
				this.StopSwim();
			}
		}

		private void PlaySwim()
		{
			base.PlayEvent(ref this._swimAudioInstance, "UndergroundSwimmerSwim", true);
			base.UpdateEvent(ref this._swimAudioInstance);
		}

		private void StopSwim()
		{
			base.StopEvent(ref this._swimAudioInstance);
		}

		public void PlayJump()
		{
			if (this.Swimmer && this.Swimmer.Behaviour.IsTriggerAttack)
			{
				base.PlayOneShotEvent("UndergroundSwimmerJump", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("UndergroundSwimmerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayLanding()
		{
			base.PlayOneShotEvent("UndergroundSwimmerLanding", EntityAudio.FxSoundCategory.Motion);
		}

		private void OnDeath()
		{
			this.StopSwim();
			this.Swimmer.OnDeath -= this.OnDeath;
		}

		private void OnDestroy()
		{
			this.StopSwim();
		}

		private const string JumpEventKey = "UndergroundSwimmerJump";

		private const string DeathEventKey = "UndergroundSwimmerDeath";

		private const string LandingEventKey = "UndergroundSwimmerLanding";

		private const string SwimEventKey = "UndergroundSwimmerSwim";

		private EventInstance _swimAudioInstance;
	}
}
