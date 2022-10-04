using System;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.Audio
{
	public class WaxCrawlerAudio : CreepCrawlerAudio
	{
		public override void Appear()
		{
			if (this.Owner.Status.IsVisibleOnCamera)
			{
				base.PlayOneShotEvent("WaxCrawlerAppearing", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public override void Disappear()
		{
			if (this.Spawned)
			{
				this.Spawned = false;
				return;
			}
			if (this.Owner.Status.IsVisibleOnCamera)
			{
				base.PlayOneShotEvent("WaxCrawlerDisappearing", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public override void Hurt()
		{
			base.PlayOneShotEvent("WaxCrawlerHurt", EntityAudio.FxSoundCategory.Motion);
		}

		public override void Death()
		{
			base.PlayOneShotEvent("WaxCrawlerDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public override void Walk()
		{
			base.PlayOneShotEvent("WaxCrawlerWalk", EntityAudio.FxSoundCategory.Motion);
		}

		private const string EntityPrefix = "WaxCrawler";

		private const string AppearingEventKey = "WaxCrawlerAppearing";

		private const string WalkEventKey = "WaxCrawlerWalk";

		private const string DisapearingEventKey = "WaxCrawlerDisappearing";

		private const string HurtEventKey = "WaxCrawlerHurt";

		private const string DeathEventKey = "WaxCrawlerDeath";
	}
}
