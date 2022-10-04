using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Adds a percentage of progress (from 0f to 100f) to an Achievement, granting it if reaches 100f.")]
	public class AddAchievementProgress : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.AchievementsManager.AddAchievementProgress(this.achievementId.Value, this.achievementProgress.Value);
			base.Finish();
		}

		[RequiredField]
		public FsmString achievementId;

		[RequiredField]
		public FsmFloat achievementProgress;
	}
}
