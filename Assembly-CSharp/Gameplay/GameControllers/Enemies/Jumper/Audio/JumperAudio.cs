using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Jumper.Audio
{
	public class JumperAudio : EntityAudio
	{
		public void PlayJump()
		{
			base.PlayEvent(ref this._jumpEventInstance, "JumperJump", true);
		}

		public void StopJump()
		{
			base.StopEvent(ref this._jumpEventInstance);
		}

		public void PlayFall()
		{
			this.StopJump();
			base.PlayOneShotEvent("JumperFall", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			this.StopJump();
			base.PlayOneShotEvent("JumperDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string JumpEventKey = "JumperJump";

		private const string FallEventKey = "JumperFall";

		private const string DeathEventKey = "JumperDeath";

		private EventInstance _jumpEventInstance;
	}
}
