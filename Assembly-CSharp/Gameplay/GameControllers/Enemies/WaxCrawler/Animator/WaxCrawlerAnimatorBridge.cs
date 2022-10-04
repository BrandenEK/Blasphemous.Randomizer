using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.Animator
{
	public class WaxCrawlerAnimatorBridge : MonoBehaviour
	{
		private void Start()
		{
			if (this.WaxCrawler == null)
			{
				Debug.LogError("You need a waxcrawler entity");
			}
		}

		public void PlayWalk()
		{
			if (this.WaxCrawler == null)
			{
				return;
			}
			this.WaxCrawler.Audio.Walk();
		}

		public void PlayAppear()
		{
			if (this.WaxCrawler == null)
			{
				return;
			}
			this.WaxCrawler.Audio.Appear();
		}

		public void PlayDisappeaar()
		{
			if (this.WaxCrawler == null)
			{
				return;
			}
			this.WaxCrawler.Audio.Disappear();
		}

		public WaxCrawler WaxCrawler;
	}
}
