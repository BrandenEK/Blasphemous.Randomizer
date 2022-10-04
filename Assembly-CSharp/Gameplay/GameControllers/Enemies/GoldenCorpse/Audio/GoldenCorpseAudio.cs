using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.Audio
{
	public class GoldenCorpseAudio : EntityAudio
	{
		public void PlayFootStep()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("GoldenCorpseWalk", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAwake()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("GoldenCorpseWakeUp", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayHit()
		{
			base.PlayOneShotEvent("GoldenCorpseHit", EntityAudio.FxSoundCategory.Damage);
		}

		private const string FootStepEventKey = "GoldenCorpseWalk";

		private const string DeathEventKey = "GoldenCorpseWakeUp";

		private const string HitEventKey = "GoldenCorpseHit";
	}
}
