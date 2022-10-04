using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.Menina
{
	public class LionheadAudio : MeninaAudio
	{
		public override void PlayDeath()
		{
			this.StopAttack();
			base.PlayOneShotEvent("LeonDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public override void PlayAttack()
		{
			base.PlayEvent(ref this._attackSoundInstance, "LeonAttack", true);
		}

		public override void StopAttack()
		{
			base.StopEvent(ref this._attackSoundInstance);
		}

		public override void PlayRightLeg()
		{
			base.PlayRightLeg();
		}

		public override void PlayLeftLeg()
		{
			base.PlayLeftLeg();
		}

		public override void PlayIdle()
		{
			base.PlayIdle();
		}

		private const string AttackEventKey = "LeonAttack";

		private const string DeathEventKey = "LeonDeath";
	}
}
