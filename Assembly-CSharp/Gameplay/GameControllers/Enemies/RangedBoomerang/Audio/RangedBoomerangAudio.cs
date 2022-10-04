using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang.Audio
{
	public class RangedBoomerangAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayLeftFootWalk()
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				return;
			}
			base.PlayOneShotEvent("RangedBoomerangFootStepLeft", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRightFootWalk()
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				return;
			}
			base.PlayOneShotEvent("RangedBoomerangFootStepRight", EntityAudio.FxSoundCategory.Motion);
		}

		public virtual void PlayThrow()
		{
			base.PlayEvent(ref this._attackEventInstance, "RangedBoomerangThrow", true);
		}

		public virtual void StopThrow()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public virtual void PlayGrab()
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				return;
			}
			base.PlayOneShotEvent("RangedBoomerangGrab", EntityAudio.FxSoundCategory.Attack);
		}

		public virtual void PlayDeath()
		{
			base.PlayOneShotEvent("RangedBoomerangDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void StopAll()
		{
		}

		private const string WalkRightFootEventKey = "RangedBoomerangFootStepRight";

		private const string WalkLeftFootEventKey = "RangedBoomerangFootStepLeft";

		private const string DeathEventKey = "RangedBoomerangDeath";

		private const string ThrowEventKey = "RangedBoomerangThrow";

		private const string GrabEventKey = "RangedBoomerangGrab";

		private EventInstance _attackEventInstance;
	}
}
