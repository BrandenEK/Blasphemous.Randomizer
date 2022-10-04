using System;
using Gameplay.GameControllers.Enemies.WaxCrawler.Audio;
using Gameplay.GameControllers.Entities.Audio;

namespace Gameplay.GameControllers.Enemies.MudCrawler.Audio
{
	public class MudCrawlerAudio : CreepCrawlerAudio
	{
		public override void Appear()
		{
			if (this.Owner.Status.IsVisibleOnCamera)
			{
				base.PlayOneShotEvent("MudCrawlerAppearing", EntityAudio.FxSoundCategory.Motion);
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
				base.PlayOneShotEvent("MudCrawlerDisappearing", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public override void Hurt()
		{
		}

		public override void Death()
		{
			base.PlayOneShotEvent("MudCrawlerDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public override void Walk()
		{
			base.PlayOneShotEvent("MudCrawlerWalk", EntityAudio.FxSoundCategory.Motion);
		}

		private const string EntityPrefix = "MudCrawler";

		private const string AppearingEventKey = "MudCrawlerAppearing";

		private const string WalkEventKey = "MudCrawlerWalk";

		private const string DisapearingEventKey = "MudCrawlerDisappearing";

		private const string DeathEventKey = "MudCrawlerDeath";
	}
}
