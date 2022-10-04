using System;
using Framework.FrameworkCore;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Disables TPO Stunt Fall.")]
	public class DisableStuntFall : FsmStateAction
	{
		public override void OnEnter()
		{
			this.oldMaxVSpeedFallStunt = Core.Logic.Penitent.AnimatorInyector.MaxVSpeedFallStunt;
			Core.Logic.Penitent.AnimatorInyector.MaxVSpeedFallStunt = float.PositiveInfinity;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			base.Finish();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			if (newLevel != oldLevel)
			{
				Core.Logic.Penitent.AnimatorInyector.MaxVSpeedFallStunt = this.oldMaxVSpeedFallStunt;
			}
		}

		private float oldMaxVSpeedFallStunt;
	}
}
