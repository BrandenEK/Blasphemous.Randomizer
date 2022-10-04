using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.MasterAnguish.Audio
{
	public class MasterAnguishAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayMerge()
		{
			base.PlayOneShotEvent("MasterAnguishMerge", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDivide()
		{
			base.PlayOneShotEvent("MasterAnguishDivide", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPreDeath()
		{
			base.PlayOneShotEvent("MasterAnguishPreDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("MasterAnguishDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void StopAll()
		{
		}

		private const string MergeEventKey = "MasterAnguishMerge";

		private const string DivideEventKey = "MasterAnguishDivide";

		private const string PreDeath = "MasterAnguishPreDeath";

		private const string Death = "MasterAnguishDeath";
	}
}
