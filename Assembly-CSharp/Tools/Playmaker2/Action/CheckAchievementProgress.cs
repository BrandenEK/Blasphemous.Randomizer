using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Checks the percentage of progress (from 0f to 100f) of an Achievement.")]
	public class CheckAchievementProgress : FsmStateAction
	{
		public override void OnEnter()
		{
			this.achievementProgress.Value = Core.AchievementsManager.CheckAchievementProgress(this.achievementId.Value);
			base.Finish();
		}

		[RequiredField]
		public FsmString achievementId;

		[UIHint(UIHint.Variable)]
		public FsmFloat achievementProgress;
	}
}
