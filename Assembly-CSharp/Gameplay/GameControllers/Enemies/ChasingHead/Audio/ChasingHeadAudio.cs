using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.ChasingHead.Audio
{
	public class ChasingHeadAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Owner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing"))
			{
				if (this.Owner.SpriteRenderer.isVisible)
				{
					this.PlayFloating();
					this.UpdateFloating();
				}
				else
				{
					this.StopFloating();
				}
			}
			if (this.Owner.Status.Dead)
			{
				this.StopFloating();
			}
		}

		public void PlayFloating()
		{
			base.PlayEvent(ref this._explodingHeadFloating, this.FloatingEventKey, true);
		}

		public void StopFloating()
		{
			base.StopEvent(ref this._explodingHeadFloating);
		}

		public void UpdateFloating()
		{
			base.UpdateEvent(ref this._explodingHeadFloating);
		}

		public void PlayExplode()
		{
			base.PlayOneShotEvent(this.ExplodeEventKey, EntityAudio.FxSoundCategory.Motion);
		}

		private void OnDestroy()
		{
			this.StopFloating();
		}

		public string FloatingEventKey;

		public string ExplodeEventKey;

		private EventInstance _explodingHeadFloating;
	}
}
