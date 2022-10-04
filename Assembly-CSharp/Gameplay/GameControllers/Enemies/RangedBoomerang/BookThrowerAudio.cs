using System;
using FMOD.Studio;
using Gameplay.GameControllers.Enemies.RangedBoomerang.Audio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang
{
	public class BookThrowerAudio : RangedBoomerangAudio
	{
		public override void PlayThrow()
		{
			base.PlayEvent(ref this._attackEventInstance, "BookThrow", true);
		}

		public override void StopThrow()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public override void PlayGrab()
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				return;
			}
			base.PlayOneShotEvent("BookGrab", EntityAudio.FxSoundCategory.Attack);
		}

		public override void PlayDeath()
		{
			base.PlayOneShotEvent("BookThrowerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string DeathEventKey = "BookThrowerDeath";

		private const string ThrowEventKey = "BookThrow";

		private const string GrabEventKey = "BookGrab";

		private EventInstance _attackEventInstance;
	}
}
