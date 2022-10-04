using System;
using Framework.FrameworkCore;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Disable Penitent Prayers.")]
	public class DisablePenitentPrayers : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.PrayerCast.enabled = false;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			base.Finish();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			if (newLevel != oldLevel)
			{
				Core.Logic.Penitent.PrayerCast.enabled = true;
			}
		}
	}
}
